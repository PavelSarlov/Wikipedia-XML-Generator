﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Wikipedia_XML_Generator.Models
{
    [BindProperties]
    public class XmlDtdViewModel
    {
        public string XML { get; set; } = null;
        public string DTD { get; set; } = null;
        public string WikiPage { get; set; } = null;
        public IFormFile FileDTD { get; set; } = null;
    }
}