using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Wikipedia_XML_Generator.Models;
using Wikipedia_XML_Generator.Utils;
using Wikipedia_XML_Generator.Utils.XMLFileGenerator;
using System.Xml;
using Wikipedia_XML_Generator.Utils.XMLFileGenerator;
using System.Web;
using Wikipedia_XML_Generator.Models.Enums;

namespace Wikipedia_XML_Generator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            return View(new XmlDtdViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(XmlDtdViewModel model)
        {
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    return await UploadDTDAsync(model);
                }

                if (Request.Form.TryGetValue("btnSubmit", out var value))
                {
                    return await (this.GetType().GetMethod(value).Invoke(this, new object[] { model }) as Task<IActionResult>);
                }
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return View(model);
        }

        [NonAction]
        public async Task<IActionResult> UploadDTDAsync(XmlDtdViewModel model)
        {
            try
            {
                model.DTD = await FileManager.ReadAsync(model.FileDTD);
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return View("Index", model);
        }

        [NonAction]
        public async Task<IActionResult> GenerateAsync(XmlDtdViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();

                    Stream file = await TypesConverter.StringToSteam(model.DTD);
                    IXMLGenerator _generator = new XMLGenerator(file);
                    doc = await _generator.GetXMLFromWikiTextAsync(model.WikiPage);
                    model.XML = FileManager.ReadAsync(TypesConverter.XmlToStream(doc).Result).Result;
                }
                catch (Exception e)
                {
                    Logger.LogAsync(Console.Out, e.Message);
                }
            }

            return View("Index", model);
        }

        [NonAction]
        public async Task<IActionResult> DownloadAsync(XmlDtdViewModel model)
        {
            try
            {
                var content = await TypesConverter.StringToUTF8(model.XML);
                string contentType = "text/xml";
                return new FileContentResult(content != null ? content : new byte[0], contentType) { FileDownloadName = "wiki.xml" };
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
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
