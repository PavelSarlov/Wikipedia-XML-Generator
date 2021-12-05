using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikipedia_XML_Generator.Models.Enums;

namespace Wikipedia_XML_Generator.Models.DTD_Elements
{
    public class Attribute
    {
        private String name;
        private String elementName;
        private AttributeTypes type;
        private AttributeValuesType valuesType;
        private List<String> enumerations;
        private String value;

        public Attribute(String name, String elementName, AttributeTypes type, AttributeValuesType valuesType = AttributeValuesType.REQUIRED, List<String> enumerations = null, String value = null)
        {
            this.name = name;
            this.elementName = elementName;
            this.type = type;
            this.enumerations = enumerations;
            this.valuesType = valuesType;
            if(this.validateValue(value) == true)
            {
                this.value = value;
            }
        }

        private bool validateValue(String value)
        {
            if(this.type == AttributeTypes.ENUMERATION)
            {
                return this.enumerations.Contains(value);
            }
            return true;
        }

        public String Name { get; private set; }
        public String ElementName { get; private set; }
        public AttributeTypes Type { get; private set;}
        public AttributeValuesType ValueTypes { get; private set; }
        public String Values 
        {
            get { return this.value; }
            set {
                if (this.validateValue(value) == true)
                {
                    this.value = value;
                }
            } 
        }
    }
}
