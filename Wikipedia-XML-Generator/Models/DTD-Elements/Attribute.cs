using System;
using System.Collections.Generic;
using Wikipedia_XML_Generator.Models.Enums;

namespace Wikipedia_XML_Generator.Models.DTD_Elements
{
    public class Attribute
    {
        private List<String> enumerations;
        private String value;

        public Attribute(String name, String elementName, AttributeTypes type, AttributeValuesType valuesType = AttributeValuesType.REQUIRED, List<String> enumerations = null, String value = null)
        {
            this.Name = name;
            this.ElementName = elementName;
            this.Type = type;
            this.enumerations = enumerations;
            this.ValuesType = valuesType;
            this.Value = value;
        }

        private bool validateValue(String value)
        {
            if (this.Type == AttributeTypes.ENUMERATION)
            {
                return this.enumerations.Contains(value);
            }
            return true;
        }

        public String Name { get; private set; }
        public String ElementName { get; private set; }
        public AttributeTypes Type { get; private set; }
        public AttributeValuesType ValuesType { get; private set; }
        public String Value
        {
            get { return this.value; }
            set
            {
                if (this.validateValue(value) == true)
                {
                    this.value = value;
                }
                else
                {
                    this.value = null;
                }
            }
        }
    }
}
