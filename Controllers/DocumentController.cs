using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLP_Prototype_MVC_8._0.Models;
using NLP_Prototype_MVC_8._0.Models.ViewModels;

namespace NLP_Prototype_MVC_8._0.Controllers
{
    public class DocumentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public DocumentController(AppDbContext appDbContext, IHttpClientFactory httpClientFactory)
        {
            _appDbContext = appDbContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _appDbContext.Documents.OrderByDescending(d => d.UploadedAt).ToListAsync());
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(DocumentUploadViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1️⃣ Guardar el documento en SQL Server
            var document = new Document
            {
                Title = model.Title,
                Content = model.Content
            };

            _appDbContext.Documents.Add(document);
            await _appDbContext.SaveChangesAsync();

            // 2️⃣ Fragmentar contenido
            int chunkSize = 300;
            var chunks = Enumerable.Range(0, (model.Content.Length + chunkSize - 1) / chunkSize)
                .Select(i => model.Content.Substring(i * chunkSize, Math.Min(chunkSize, model.Content.Length - i * chunkSize)))
                .ToList();

            // 3️⃣ Obtener embeddings
            var embeddings = new List<List<float>>();
            foreach (var chunk in chunks)
            {
                var embedding = await GetEmbeddingFromService(chunk);
                embeddings.Add(embedding);
            }

            // 4️⃣ Construir payload con estructura compatible con ChromaDB v2
            var chromaPayload = new
            {
                embeddings = embeddings,
                metadatas = chunks.Select(_ => new { documentId = document.Id, title = model.Title }).ToList(),
                documents = chunks,
                uris = chunks.Select(_ => (string?)null).ToList(),
                ids = chunks.Select((_, index) => $"doc_{document.Id}_chunk_{index}").ToList()
            };

            // 5️⃣ Definir endpoint correcto
            string tenant = "default_tenant";
            string database = "history_db";
            string collectionId = "52595b81-e8e9-4c51-bd75-ce5cf36cc83b"; // 👈 Reemplaza esto con el ID real de la colección

            var url = $"http://localhost:8000/api/v2/tenants/{tenant}/databases/{database}/collections/{collectionId}/add";

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(
                url,
                new StringContent(JsonSerializer.Serialize(chromaPayload), Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Error al enviar los fragmentos a ChromaDB.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<float>> GetEmbeddingFromService(string text)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(
                "http://localhost:8001/embedding",
                new StringContent(JsonSerializer.Serialize(new { text }), Encoding.UTF8, "application/json")
            );

            var body = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(body);
            var embeddingArray = parsed.RootElement.GetProperty("embedding")
                .EnumerateArray().Select(x => x.GetSingle()).ToList();

            return embeddingArray;
        }
    }
}
