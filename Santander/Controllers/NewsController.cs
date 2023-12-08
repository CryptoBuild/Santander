using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Santander.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Santander.Controllers.NewsViewModel;

namespace Santander
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : Controller
    {
        private readonly ILogger<NewsController> _logger;
        public NewsController(ILogger<NewsController> logger)
        {
            _logger = logger;
        }
        public static NewsViewModel _NewsViewModel { get; set; }

        [NonAction]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("GetTopnStoriesFromBestStories")]

        // GET: GetTopnStoriesFromBestStories/5
        public IEnumerable<NewsViewModelItem> GetTopnStoriesFromBestStories(int n)
        {
            if (_NewsViewModel == null)
            {
                _NewsViewModel = new NewsViewModel();
            }
            return _NewsViewModel.NewsItemList.GetRange(0, Math.Min(n, _NewsViewModel.NewsItemList.Count() )).ToArray();
        }


    }
}
