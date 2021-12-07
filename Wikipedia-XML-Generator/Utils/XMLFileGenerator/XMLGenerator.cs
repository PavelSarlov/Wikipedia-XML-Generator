using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wikipedia_XML_Generator.Utils.DTDReader;
using System.Xml;
using Wikipedia_XML_Generator.Models.DTD_Elements;

namespace Wikipedia_XML_Generator.Utils.XMLFileGenerator
{
    public class XMLGenerator
    {
        private IDTDFileReader _reader;

        public  XMLGenerator(String filepath)
        {
            this._reader = new DTDFileReader(filepath);
        }

        private bool ValidateXMLFileToDTD(ref XmlDocument xmlFile)
        {
            List<Element> DTDElements = this._reader.GetElements();
            foreach(var e in DTDElements)
            {
                var currentElement = xmlFile.GetElementsByTagName(e.Name);
                if (currentElement == null)
                {
                    return false;
                }
                else
                {
                    var children = currentElement[0].SelectNodes("child::node()");
                    foreach(XmlNode n in children)
                    {
                        if(e.HasAChild(n.Name))
                        {
                            e.AddChild(n.Name);
                        }
                        else
                        {
                            n.RemoveAll();
                            currentElement[0].RemoveChild(n);
                        }
                    }
                }
                if(!e.IsChildrenCountValid())
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<XmlDocument> GetXMLFromWikiTextAsync(String url)
        {
            var wikiXML = await WikiScrapper.GetXml(url);
            if (this._reader.GetStatus() == -1 || wikiXML == null)
            {
                return null;
            }
            bool isValid = ValidateXMLFileToDTD(ref wikiXML);
            if(isValid == true)
            {
                return wikiXML;
            }
            return new XmlDocument();
        }
    }
}
