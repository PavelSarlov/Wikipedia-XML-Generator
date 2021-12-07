﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikipedia_XML_Generator.Models.DTD_Elements;
using Wikipedia_XML_Generator.Models.Enums;
using Attribute = Wikipedia_XML_Generator.Models.DTD_Elements.Attribute;

namespace Wikipedia_XML_Generator.Utils.DTDReader
{
    public class DTDFileReader : IDTDFileReader
    {
        private String DTDtext;
        private List<String> elementsLines;
        private List<String> attibutesLines;

        public DTDFileReader(String filepath)
        {
            this.Status = FileManager.Read(filepath, out this.DTDtext);
            List<String> lines = this.DTDtext.Split("\r\n").ToList();

            foreach (var l in lines)
            {
                if (l.StartsWith("<!ELEMENT"))
                {
                    elementsLines.Add(l);
                }
                else if (l.StartsWith("<!ATTLIST"))
                {
                    attibutesLines.Add(l);
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

        public List<Element> GetElements()
        {
            List<Element> elements = new List<Element>();
            foreach (var item in elementsLines)
            {
                String name;
                Dictionary<String, char> childrenQuantifies = new Dictionary<string, char>();
                List<String> parts = this.DTDtext.Remove(0, 1).Remove(this.DTDtext.Length - 1, 1).Split("(").ToList();
                name = parts[0].Split(" ")[1];
                foreach (var element in parts[1].Remove(parts[1].Length - 1, 1).Replace(" ", "").Split(","))
                {
                    char quantify = ' ';
                    if (element[element.Length - 1] == '*' || element[element.Length - 1] == '+' || element[element.Length - 1] == '?')
                    {
                        quantify = element[element.Length - 1];
                        element.Remove(element.Length - 1, 1);
                    }
                    childrenQuantifies[element] = quantify;
                }
                elements.Add(new Element(name, childrenQuantifies));
            }
            return elements;
        }

        public List<Attribute> GetAttributes()
        {
            List<Attribute> attributes = new List<Attribute>();
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
                attributes.Add(new Attribute(name, elementName, type, valuesType, enumerations, value));
            }
            return attributes;
        }

        public int GetStatus()
        {
            return this.Status;
        }

        public int Status { get; set; }
    }
}