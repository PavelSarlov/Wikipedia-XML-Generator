using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Wikipedia_XML_Generator.Models;
using Wikipedia_XML_Generator.Utils;
using Wikipedia_XML_Generator.Utils.XMLFileGenerator;
using System.Web;

namespace Wikipedia_XML_Generator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Index(XmlDtdViewModel model)
        {
            try
            {
                if (Request.Method == "POST")
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        return await UploadDTD(model);
                    }

                    if (Request.Form.TryGetValue("btnSubmit", out var value))
                    {
                        return await (this.GetType().GetMethod(value).Invoke(this, new object[] { model }) as Task<IActionResult>);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return View(model);
        }

        [NonAction]
        public async Task<IActionResult> UploadDTD(XmlDtdViewModel model)
        {
            try
            {
                model.DTD = await FileManager.ReadAsync(model.FileDTD); ;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return View("Index", model);
        }

        [NonAction]
        public async Task<IActionResult> Generate(XmlDtdViewModel model)
        {
            try
            {
                using (var dtdStream = new MemoryStream())
                {
                    if (await FileManager.WriteAsync(dtdStream, model.DTD) == -1) throw new IOException();
                    dtdStream.Position = 0;
                    
                    var xmlGenerator = new XMLGenerator(dtdStream);
                    var xml = await xmlGenerator.GetXMLFromWikiTextAsync(HttpUtility.UrlDecode(model.WikiPage));
                    using (var xmlStream = await TypesConverter.XmlToStream(xml))
                    {
                        model.XML = await FileManager.ReadAsync(xmlStream);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return View("Index", model);
        }

        [NonAction]
        public async Task<IActionResult> Download(XmlDtdViewModel model)
        {
            try
            {
                var content = await TypesConverter.StringToUTF8(model.XML);
                string contentType = "text/xml";
                return new FileContentResult(content, contentType) { FileDownloadName = "wiki.xml" };
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
