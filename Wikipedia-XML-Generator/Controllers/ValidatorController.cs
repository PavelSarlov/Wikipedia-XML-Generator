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
        public async Task<JsonResult> ValidateXMLAsync([FromBody] XmlDtdViewModel model)
        {
            try
            {
                var validator = new XMLValidator(model.DTD.ToUpper());
                return Json(validator.Validate(model.XML));
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return Json(false);
            }
        }

        [HttpPost]
        public async Task<JsonResult> ValidateDTDAsync([FromBody] XmlDtdViewModel model)
        {
            try
            {
                var validator = new DTDValidator();
                return Json(validator.Validate(model.DTD));
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return Json(false);
            }
        }
    }
}
