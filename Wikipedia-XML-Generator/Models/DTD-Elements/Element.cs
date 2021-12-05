using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wikipedia_XML_Generator.Models.DTD_Elements
{
    public class Element
    {
        private String name;
        private List<List<Tuple<String, char>>> enumeration;
        private String content;

        public Element(String name, List<List<Tuple<String, char>>> enumeration = null, String content = null)
        {
            this.name = name;
            this.enumeration = enumeration;
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
            for(int i = 0; i < this.enumeration.Count(); i++)
            {
                if(this.enumeration[i].Count() == 1)
                {
                    if(this.enumeration[i][0].Item1 == "#PCDATA")
                    {
                        return true;
                    }
                }
                else
                {
                    if(this.enumeration[i].Contains(new Tuple<String, char>("#PCDATA", '-')) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public String Name { get; private set; }
        public List<List<Tuple<String, char>>> Enumeration { get; private set; }
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
