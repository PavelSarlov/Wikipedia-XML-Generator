using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikipedia_XML_Generator.Models.DTD_Elements;
using Attribute = Wikipedia_XML_Generator.Models.DTD_Elements.Attribute;

namespace Wikipedia_XML_Generator.Utils.DTDReader
{
    public interface IDTDFileReader
    {
        public List<Element> GetElements();

        public List<Attribute> GetAttributes();

        public int GetStatus();
    }
}