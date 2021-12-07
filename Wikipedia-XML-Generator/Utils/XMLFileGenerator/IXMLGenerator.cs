using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Wikipedia_XML_Generator.Utils.XMLFileGenerator
{
    public interface IXMLGenerator
    {
        public Task<XmlDocument> GetXMLFromWikiTextAsync(String url);
    }
}
