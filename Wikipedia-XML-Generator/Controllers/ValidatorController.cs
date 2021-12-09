using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Wikipedia_XML_Generator.Models;
using Wikipedia_XML_Generator.Utils;
using Wikipedia_XML_Generator.Utils.XmlDTDValidator;

namespace Wikipedia_XML_Generator.Controllers
{
    public class ValidatorController : Controller
    {
        public ValidatorController()
        { }

        [HttpPost]
        public async Task<IActionResult> Validate([FromBody] XmlDtdViewModel model)
        {
            try
            {
                var validator = new XmlDTDValidator(model.DTD);
                return Ok(validator.Validate(model.XML));
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return BadRequest();
            }
        }
    }
}
