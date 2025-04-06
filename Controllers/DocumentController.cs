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

            // 1️⃣ Guardar documento en SQL Server
            var document = new Document
            {
                Title = model.Title,
                Content = model.Content
            };

            _appDbContext.Documents.Add(document);
            await _appDbContext.SaveChangesAsync();

            // 2️⃣ Fragmentar el contenido
            var chunkSize = 300;
            var chunks = Enumerable.Range(0, (model.Content.Length + chunkSize - 1) / chunkSize)
                .Select(i => model.Content.Substring(i * chunkSize, Math.Min(chunkSize, model.Content.Length - i * chunkSize)))
                .ToList();

            // 3️⃣ Preparar payload para ChromaDB (v2)
            var chromaPayload = new
            {
                documents = chunks.Select((chunk, index) => new
                {
                    id = $"doc_{document.Id}_chunk_{index}",
                    text = chunk,
                    metadata = new { documentId = document.Id, title = model.Title }
                }).ToList()
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(
                "http://localhost:8000/api/v2/tenants/my_tenant/databases/history_db/collections/history_documents/documents",
                new StringContent(JsonSerializer.Serialize(chromaPayload), Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                // ⚠️ Puedes agregar manejo de error más robusto aquí
                ModelState.AddModelError("", "Error al enviar a ChromaDB.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
