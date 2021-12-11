using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Wikipedia_XML_Generator.Utils.XmlDTDValidator;

namespace Wikipedia_XML_Generator.Models
{
    [BindProperties]
    public class XmlDtdViewModel
    {
        public string XML { get; set; } = null;

        [Required(ErrorMessage = "You need to enter a valid DTD")]
        [DTDValidator(ErrorMessage = "Invalid DTD")]
        public string DTD { get; set; } = null;

        [Required(ErrorMessage = "You need to enter a Wikipedia page url")]
        [RegularExpression(@"http[s]:\/\/\w{2,}\.wikipedia\.org\/wiki\/.+", ErrorMessage = "Invalid wikipedia page")]
        public string WikiPage { get; set; } = null;

        public IFormFile FileDTD { get; set; } = null;
    }
}
