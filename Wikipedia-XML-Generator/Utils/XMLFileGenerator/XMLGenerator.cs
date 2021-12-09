using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Wikipedia_XML_Generator.Models.DTD_Elements;
using Wikipedia_XML_Generator.Utils.DTDReader;
using Attribute = Wikipedia_XML_Generator.Models.DTD_Elements.Attribute;

namespace Wikipedia_XML_Generator.Utils.XMLFileGenerator
{
    public class XMLGenerator : IXMLGenerator
    {
        private IDTDFileReader _reader;

        public XMLGenerator(Stream file)
        {
            this._reader = new DTDReader.XMLFileGenerator(file);
        }

        public XMLGenerator(IFormFile file)
        {
            this._reader = new DTDReader.XMLFileGenerator(file);
        }

        private void AddAttributesToNode(ref XmlElement el, Dictionary<String, List<Attribute>> DTDAttributes)
        {
            XmlDocument doc = new XmlDocument();
            if (DTDAttributes.ContainsKey(el.Name))
            {
                foreach (var item in DTDAttributes[el.Name])
                {
                    XmlAttribute attr = doc.CreateAttribute(item.Name);
                    if (item.Value != null)
                    {
                        attr.Value = item.Value;
                    }
                    else
                    {
                        attr.Value = "N/A";
                    }
                    el.Attributes.SetNamedItem(attr);
                }
            }
        }

        private async Task<XmlDocument> GenerateXMLFileAsync(Dictionary<String, Element> DTDElements, Dictionary<String, List<Attribute>> DTDAttributes)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement el = doc.CreateElement(string.Empty, this._reader.GetRoot(), string.Empty);
                this.AddAttributesToNode(ref el, DTDAttributes);
                doc.AppendChild(el);
                Queue<XmlNode> nextElements = new Queue<XmlNode>();
                nextElements.Enqueue(el);

                while (nextElements.Count() != 0)
                {
                    XmlNode node = nextElements.Dequeue();

                    foreach (var e in DTDElements[node.Name].ChildrenOccurrences)
                    {
                        if (e.Key == "#PCDATA")
                        {
                            continue;
                        }
                        XmlElement newEl = doc.CreateElement(string.Empty, e.Key, string.Empty);
                        this.AddAttributesToNode(ref newEl, DTDAttributes);
                        node.AppendChild(newEl);
                        nextElements.Enqueue(newEl);
                    }

                    foreach (var e in DTDElements[node.Name].ChildrenInGroupOccurrences)
                    {
                        if (e.Key[0] == "#PCDATA")
                        {
                            continue;
                        }
                        XmlElement newEl = doc.CreateElement(string.Empty, e.Key[0], string.Empty);
                        this.AddAttributesToNode(ref newEl, DTDAttributes);
                        node.AppendChild(newEl);
                        nextElements.Enqueue(newEl);
                    }
                }
                return doc;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return null;
            }
        }

        private bool ValidateXMLFileToDTD(Dictionary<String, Element> DTDElements, Dictionary<String, List<Attribute>> DTDAttributes, ref XmlDocument xmlFile)
        {
            try
            {
                foreach (var e in DTDElements)
                {
                    var currentElement = xmlFile.GetElementsByTagName(e.Value.Name);
                    if (currentElement.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        var children = currentElement[0].SelectNodes("child::node()");
                        if (DTDAttributes.ContainsKey(e.Value.Name))
                        {
                            foreach (var item in DTDAttributes[e.Value.Name])
                            {
                                XmlAttribute attr = xmlFile.CreateAttribute(item.Name);
                                if (item.Value != null)
                                {
                                    attr.Value = item.Value;
                                }
                                else
                                {
                                    attr.Value = "N/A";
                                }
                                currentElement[0].Attributes.SetNamedItem(attr);
                            }
                        }
                        foreach (XmlNode n in children)
                        {
                            if (n.NodeType == XmlNodeType.Text)
                            {
                                e.Value.AddChild("#PCDATA");
                                continue;
                            }
                            if (e.Value.HasAChild(n.Name))
                            {
                                e.Value.AddChild(n.Name);
                            }
                            else
                            {
                                n.RemoveAll();
                                currentElement[0].RemoveChild(n);
                            }
                        }
                    }
                    if (!e.Value.IsChildrenCountValid())
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return false;
            }
        }

        public async Task<XmlDocument> GetXMLFromWikiTextAsync(String url)
        {
            var wikiXML = await WikiScrapper.GetXml(url);
            if (this._reader.GetStatus() == -1 || wikiXML == null)
            {
                return null;
            }
            Dictionary<String, Element> DTDElements = this._reader.GetElements();
            Dictionary<String, List<Attribute>> DTDAttributes = this._reader.GetAttributes();
            bool isValid = ValidateXMLFileToDTD(DTDElements, DTDAttributes, ref wikiXML);
            if (isValid == true)
            {
                return wikiXML;
            }
            return await this.GenerateXMLFileAsync(DTDElements, DTDAttributes);
        }
    }
}
