using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikipedia_XML_Generator.Models.DTD_Elements;
using Wikipedia_XML_Generator.Models.Enums;
using Attribute = Wikipedia_XML_Generator.Models.DTD_Elements.Attribute;

namespace Wikipedia_XML_Generator.Utils.DTDReader
{
    public class XMLFileGenerator : IDTDFileReader
    {
        private String DTDtext;
        private List<String> elementsLines;
        private List<String> attibutesLines;

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
            List<String> lines = this.DTDtext.Split("\r\n").ToList();
            this.elementsLines = new List<String>();
            this.attibutesLines = new List<String>();
            this.Root = lines[0].Split("(")[0].Split(" ").Last().ToUpper();
            foreach (var l in lines)
            {
                String line = l.Remove(0, l.LastIndexOf("\t") + 1);
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

        private List<String> GetWordsInAttributeLine(String line)
        {
            List<String> parts = line.Remove(0, 1).Remove(line.Length - 1, 1).Split("\"").ToList();
            List<String> words = new List<String>();
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

        private void SetAttributeType(String word, out AttributeTypes type)
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

        private void SetAttributeValuesType(String word, out AttributeValuesType valuesType)
        {
            valuesType = word.Remove(0, 1) switch
            {
                "REQUIRED" => AttributeValuesType.REQUIRED,
                "IMPLIED" => AttributeValuesType.IMPLIED,
                "FIXED" => AttributeValuesType.FIXED,
                _ => AttributeValuesType.VALUE
            };
        }

        public Dictionary<String, Element> GetElements()
        {
            Dictionary<String, Element> elements = new Dictionary<String, Element>();
            foreach (var item in this.elementsLines)
            {
                String name;
                Dictionary<String, char> childrenQuantifies = new Dictionary<string, char>();
                String line = item.Remove(0, 1);
                line.Remove(line.Length - 1, 1);
                List<String> parts = line.Split("(").ToList();
                name = parts[0].Split(" ")[1];
                foreach (var element in parts[1].Remove(parts[1].Length - 1, 1).Replace(" ", "").Replace(")", "").Split(","))
                {
                    char quantify = ' ';
                    if (element[element.Length - 1] == '*' || element[element.Length - 1] == '+' || element[element.Length - 1] == '?')
                    {
                        quantify = element[element.Length - 1];
                        element.Remove(element.Length - 1, 1);
                    }
                    childrenQuantifies[element] = quantify;
                }
                elements[name] = new Element(name, childrenQuantifies);
            }
            return elements;
        }

        public Dictionary<String, List<Attribute>> GetAttributes()
        {
            Dictionary<String, List<Attribute>> attributes = new Dictionary<String, List<Attribute>>();
            foreach (var item in attibutesLines)
            {
                List<String> words = GetWordsInAttributeLine(item);

                String name = words[1], elementName = words[2];
                AttributeTypes type;
                SetAttributeType(words[3], out type);
                List<String> enumerations = null;
                if (type == AttributeTypes.ENUMERATION)
                {
                    enumerations = words[3].Remove(0, 1).Remove(words[3].Length - 1, 1).Split('|').ToList();
                }

                AttributeValuesType valuesType = AttributeValuesType.REQUIRED;
                String value = null;
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

        public String GetRoot()
        {
            return this.Root;
        }

        public int Status { get; set; }
        public String Root { get; set; }
    }
}