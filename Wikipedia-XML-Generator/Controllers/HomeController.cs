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
        public IActionResult Index_Post(XmlDtdViewModel model)
        {
            if (Request.Form.ContainsKey("btnGenerate"))
            {
                model.XML = "this is a test";
            }
            if (Request.Form.Files.Count > 0)
            {
                FileManager.Read(Request.Form.Files.First(), out string dtd);
                model.DTD = dtd;
            }

            return RedirectToAction("Index", model);
        }

        public IActionResult Download(XmlDtdViewModel model)
        {
            var fs = StringConverter.StringToUTF8(model.XML);
            string contentType = "application/xml";
            return new FileContentResult(fs, contentType);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
