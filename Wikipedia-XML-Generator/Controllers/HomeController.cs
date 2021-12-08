﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using Wikipedia_XML_Generator.Models;
using Wikipedia_XML_Generator.Utils;
using Wikipedia_XML_Generator.Utils.XMLFileGenerator;

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
        public IActionResult Index(XmlDtdViewModel model)
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
                        return this.GetType().GetMethod(value).Invoke(this, new object[] { model }) as IActionResult;
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
        public IActionResult Generate(XmlDtdViewModel model)
        {
            try
            {
                using (var dtdStream = new MemoryStream())
                {
                    if (FileManager.Write(dtdStream, model.DTD) == -1) throw new IOException();
                    dtdStream.Position = 0;
                       
                    var xmlGenerator = new XMLGenerator(dtdStream);
                    using (var xmlStream = TypesConverter.XmlToStream(xmlGenerator.GetXMLFromWikiTextAsync(model.WikiPage).Result).Result)
                    {
                        model.XML = FileManager.ReadAsync(xmlStream).Result;
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
