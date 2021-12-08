using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wikipedia_XML_Generator.Models;
using Wikipedia_XML_Generator.Utils;

namespace Wikipedia_XML_Generator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(XmlDtdViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Generate(XmlDtdViewModel model)
        {
            if (Request.Form.ContainsKey("btnGenerate"))
            {
                var doc = WikiScrapper.GetXml(model.WikiPage).Result;
                model.XML = TypesConverter.XmlToString(doc).Result;
            }

            return View("Index", model);
        }

        public async Task<IActionResult> Download(XmlDtdViewModel model)
        {
            try
            {
                var fs = await TypesConverter.StringToUTF8(model.XML);
                string contentType = "application/xml";
                return new FileContentResult(fs, contentType);
            }
            catch(Exception e)
            {
                await Logger.LogAsync(Console.Out, e.Message);
                return BadRequest();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
