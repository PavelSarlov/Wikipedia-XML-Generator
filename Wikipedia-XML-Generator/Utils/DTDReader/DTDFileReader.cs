using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Wikipedia_XML_Generator.Models.DTD_Elements;
using Wikipedia_XML_Generator.Models.Enums;
using Attribute = Wikipedia_XML_Generator.Models.DTD_Elements.Attribute;

namespace Wikipedia_XML_Generator.Utils.DTDReader
{
    public class XMLFileGenerator : IDTDFileReader
    {
        private string DTDtext;
        private List<string> elementsLines;
        private List<string> attibutesLines;

        public XMLFileGenerator(Stream file)
        {
            this.Status = FileManager.Read(file, out this.DTDtext);
            this.FieldSetter();
        }

        public XMLFileGenerator(IFormFile file)
        {
            this.Status = FileManager.Read(file, out this.DTDtext);
            this.FieldSetter();
        }

        private void FieldSetter()
        {
            List<string> lines = this.DTDtext.Split("\r\n").ToList();
            this.elementsLines = new List<string>();
            this.attibutesLines = new List<string>();
            this.Root = lines[0].Split("[")[0].Trim().Split(" ").Last().ToUpper();
            foreach (var l in lines)
            {
                string line = l.Remove(0, l.LastIndexOf("\t") + 1);
                if (line.StartsWith("<!ELEMENT"))
                {
                    this.elementsLines.Add(line);
                }
                else if (line.StartsWith("<!ATTLIST"))
                {
                    this.attibutesLines.Add(line);
                }
            }
        }

        private List<string> GetWordsInAttributeLine(string line)
        {
            List<string> parts = line.Trim('>').Split("\"").ToList();
            List<string> words = new List<string>();
            switch (parts.Count())
            {
                case 1:
                    words = parts[0].Split(" ").ToList();
                    break;
                case 2:
                    words = parts[0].Split(" ").ToList();
                    words.Add(parts[1]);
                    break;
            }
            return words;
        }

        private void SetAttributeType(string word, out AttributeTypes type)
        {
            type = word switch
            {
                "CDATA" => AttributeTypes.CDATA,
                "ID" => AttributeTypes.ID,
                "IDREF" => AttributeTypes.IDREF,
                "IDREFS" => AttributeTypes.IDREFS,
                "NMTOKEN" => AttributeTypes.NMTOKEN,
                "NMTOKENS" => AttributeTypes.NMTOKENS,
                "ENTITY" => AttributeTypes.ENTITY,
                "ENTITIES" => AttributeTypes.ENTITIES,
                "NOTATION" => AttributeTypes.NOTATION,
                _ => AttributeTypes.ENUMERATION,
            };
        }

        private void SetAttributeValuesType(string word, out AttributeValuesType valuesType)
        {
            valuesType = word.Remove(0, 1) switch
            {
                "REQUIRED" => AttributeValuesType.REQUIRED,
                "IMPLIED" => AttributeValuesType.IMPLIED,
                "FIXED" => AttributeValuesType.FIXED,
                _ => AttributeValuesType.VALUE
            };
        }

        public Dictionary<string, Element> GetElements()
        {
            Dictionary<string, Element> elements = new Dictionary<string, Element>();
            foreach (var item in this.elementsLines)
            {
                string name;
                Dictionary<string, char> childrenQuantifies = new Dictionary<string, char>();
                string line = item.Trim('>');
                List<string> parts = line.Split(' ', 3).ToList();
                name = parts[1].ToUpper();

                if (!(parts[2].Contains("ANY") || parts[2].Contains("EMPTY")))
                {
                    foreach (var element in Regex.Replace(parts[2], "[ \\(\\)]", "").ToUpper().Split(","))
                    {
                        char quantify = ' ';
                        if (element.Last() == '*' || element.Last() == '+' || element.Last() == '?')
                        {
                            quantify = element.Last();
                            element.Remove(element.Length - 1, 1);
                        }
                        childrenQuantifies[element] = quantify;
                    }
                }
                else
                {
                    childrenQuantifies[parts[2]] = ' ';
                }
                
                elements[name] = new Element(name, childrenQuantifies);
            }
            return elements;
        }

        public Dictionary<string, List<Attribute>> GetAttributes()
        {
            Dictionary<string, List<Attribute>> attributes = new Dictionary<string, List<Attribute>>();
            foreach (var item in attibutesLines)
            {
                List<string> words = GetWordsInAttributeLine(item);

                string elementName = words[1], name = words[2];
                AttributeTypes type;
                SetAttributeType(words[3], out type);
                List<string> enumerations = null;
                if (type == AttributeTypes.ENUMERATION)
                {
                    enumerations = words[3].Trim('(',')').Split('|').ToList();
                }

                AttributeValuesType valuesType = AttributeValuesType.REQUIRED;
                string value = null;
                if (words.Count() > 4)
                {
                    SetAttributeValuesType(words[4], out valuesType);
                    if (valuesType == AttributeValuesType.VALUE)
                    {
                        value = words[4];
                    }
                    else if (valuesType == AttributeValuesType.FIXED)
                    {
                        if (words.Count() > 5)
                        {
                            value = words[5];
                        }
                        else
                        {
                            value = "N/A";
                        }
                    }
                }
                attributes[elementName].Add(new Attribute(name, elementName, type, valuesType, enumerations, value));
            }
            return attributes;
        }

        public int GetStatus()
        {
            return this.Status;
        }

        public string GetRoot()
        {
            return this.Root;
        }

        public int Status { get; set; }
        public string Root { get; set; }
    }
}