using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wikipedia_XML_Generator.Models.DTD_Elements
{
    public class Element
    {
        private String name;
        private String content;
        Dictionary<String, int> childrenOccurrences;
        Dictionary<String, char> childrenQuantifies;

        public Element(String name, Dictionary<String, char> childrenQuantifies, String content = null)
        {
            this.name = name;
            this.childrenQuantifies = childrenQuantifies;
            this.popuateOccurrencesDictionary();
            if(this.validateContent(content) == true)
            {
                this.content = content;
            }
            else
            {
                this.content = null;
            }
        }

        private bool validateContent(String content)
        {
            if(this.childrenQuantifies.ContainsKey("#PCDATA"))
            {
                return true;
            }
            return false;
        }

        private void popuateOccurrencesDictionary()
        {
            foreach(var item in this.childrenQuantifies)
            {
                this.childrenOccurrences[item.Key] = 0;
            }
        }

        public bool isAChild(String elementName)
        {
            return this.childrenQuantifies.ContainsKey(elementName);
        }

        public bool addChild(String elementName)
        {
            if (isAChild(elementName))
            {
                this.childrenOccurrences[elementName]++;
                return true;
            }
            return false;
        }

        public bool isChildrenCountValid()
        {
            foreach(var item in this.childrenQuantifies)
            {
                if(item.Value == '+' && this.childrenOccurrences[item.Key]<1)
                {
                    return false;
                }
                else if (item.Value == '?' && this.childrenOccurrences[item.Key] > 1)
                {
                    return false;
                }
                if (item.Value == ' ' && this.childrenOccurrences[item.Key] != 1)
                {
                    return false;
                }
            }
            return true;
        }

        public String Name { get; private set; }
        public String Content
        {
            get { return this.content; }
            set
            {
                if (this.validateContent(value) == true)
                {
                    this.content = value;
                }
                else
                {
                    this.content = null;
                }
            }
        }
    }
}
