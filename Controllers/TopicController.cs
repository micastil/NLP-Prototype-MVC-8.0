using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NLP_Prototype_MVC_8._0.Models;
using NLP_Prototype_MVC_8._0.Models.ViewModels;

namespace NLP_Prototype_MVC_8._0.Controllers
{
    public class TopicController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public TopicController(AppDbContext appDbContext, IHttpClientFactory httpClientFactory)
        {
            _appDbContext = appDbContext;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _appDbContext.Topics.OrderBy(d => d.Title).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Topic model) 
        {
            if (!ModelState.IsValid)
                return View(model);

            Topic newtopic = new Topic { 
                Title = model.Title
            };

            _appDbContext.Topics.Add(newtopic);
            int _result = await _appDbContext.SaveChangesAsync();

            if ( _result == 0) 
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id) 
        {
            var _topic = await _appDbContext.Topics.Where(i => i.Id == id).FirstOrDefaultAsync();

            if (_topic == null) 
            {
                return NotFound();
            }

            TopicDetailViewModel model = new TopicDetailViewModel 
            {
                Title = _topic.Title,
                Questions = _topic.Questions,
                Documents = _topic.Documents
            };

            return View(model);
        }
    }
}
