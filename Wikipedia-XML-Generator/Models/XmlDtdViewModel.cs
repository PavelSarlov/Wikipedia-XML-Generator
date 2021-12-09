using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wikipedia_XML_Generator.Models.Enums;
using Wikipedia_XML_Generator.Utils.XmlDTDValidator;

namespace Wikipedia_XML_Generator.Models
{
    [BindProperties]
    public class XmlDtdViewModel
    {
        public string XML { get; set; } = null;

        public string DTD { get; set; } = null;

        [Required(ErrorMessage = "You need to enter a Wikipedia page url.")]
        public string WikiPage { get; set; } = null;

        public IFormFile FileDTD { get; set; } = null;
    }
}
