using System;
using System.Collections.Generic;
using Wikipedia_XML_Generator.Models.DTD_Elements;
using Attribute = Wikipedia_XML_Generator.Models.DTD_Elements.Attribute;

namespace Wikipedia_XML_Generator.Utils.DTDReader
{
    public interface IDTDFileReader
    {
        public Dictionary<String, Element> GetElements();

        public Dictionary<String, List<Attribute>> GetAttributes();
    }
}