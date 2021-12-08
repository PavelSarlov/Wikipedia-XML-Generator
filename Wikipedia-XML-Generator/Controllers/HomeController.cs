using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Wikipedia_XML_Generator.Models;
using Wikipedia_XML_Generator.Utils;
using Wikipedia_XML_Generator.Utils.XMLFileGenerator;
using System.Xml;

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
        public async Task<IActionResult> IndexAsync(XmlDtdViewModel model)
        {
            try
            {
                if (Request.Method == "POST")
                {
                    if (Request.Form.Files.Count > 0)
                    {
                        return UploadDTD(model);
                    }

                    if (Request.Form.TryGetValue("btnSubmit", out var value))
                    {
                        await this.GenerateAsync(model);
                        //return this.GetType().GetMethod(value).Invoke(this, new object[] { model }) as IActionResult;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return View(model);
        }

        public IActionResult UploadDTD(XmlDtdViewModel model)
        {
            try
            {
                FileManager.Read(model.FileDTD, out string dtd);
                model.DTD = dtd;
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
            try
            {
                XmlDocument doc = new XmlDocument();
                if (model.FileDTD != null)
                {
                    IXMLGenerator _generator = new XMLGenerator(model.FileDTD);
                    doc = await _generator.GetXMLFromWikiTextAsync(model.WikiPage);
                }
                else if (model.DTD != null)
                {
                    Stream file = await TypesConverter.StringToSteam(model.DTD);
                    IXMLGenerator _generator = new XMLGenerator(file);
                    doc = await _generator.GetXMLFromWikiTextAsync(model.WikiPage);
                }
                model.XML = FileManager.ReadAsync(TypesConverter.XmlToStream(doc).Result).Result;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return View("Index", model);
        }

        [NonAction]
        public IActionResult Download(XmlDtdViewModel model)
        {
            try
            {
                var content = TypesConverter.StringToUTF8(model.XML).Result;
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
