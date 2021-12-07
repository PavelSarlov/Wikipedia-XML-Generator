using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wikipedia_XML_Generator.Models.DTD_Elements
{
    public class Element
    {
        private String content;
        private Dictionary<String, Tuple<int, char>> childrenOccurrences;
        private Dictionary<List<String>, Tuple<int, char>> childrenInGroupOccurrences;

        public Element(String name, Dictionary<String, char> childrenQuantifies = null, String content = null)
        {
            this.Name = name;
            if (childrenQuantifies != null)
            {
                this.PopulateOccurrencesDictionary(childrenQuantifies);
            }
            else
            {
                this.childrenOccurrences = null;
                this.childrenInGroupOccurrences = null;
            }
            this.Content = content;
        }

        private bool ValidateContent(String content)
        {
            if(this.childrenOccurrences.ContainsKey("#PCDATA"))
            {
                return true;
            }
            return false;
        }

        private void PopulateOccurrencesDictionary(Dictionary<String, char> childrenQuantifies)
        {
            foreach(var item in childrenQuantifies)
            {
                if(item.Key[0]!='(')
                {
                    this.childrenOccurrences[item.Key] = new Tuple<int, char>(0, item.Value);
                }
                else
                {
                    List<String> elements = item.Key.Remove(0,1).Remove(item.Key.Length-1, 1).Split('|').ToList<String>();
                    this.childrenInGroupOccurrences[elements] = new Tuple<int, char>(0, item.Value);
                }
            }
        }

        public bool AddChild(String elementName)
        {
            if (this.childrenOccurrences.ContainsKey(elementName))
            {
                var valTuple = this.childrenOccurrences[elementName];
                this.childrenOccurrences[elementName] = new Tuple<int, char>(valTuple.Item1 + 1, valTuple.Item2);
                return true;
            }
            foreach (var item in this.childrenInGroupOccurrences)
            {
                if (item.Key.Contains(elementName))
                {
                    this.childrenInGroupOccurrences[item.Key] = new Tuple<int, char>(item.Value.Item1 + 1, item.Value.Item2);
                    return true;
                }
            }
            return false;
        }

        public bool IsChildrenCountValid()
        {
            foreach(var item in this.childrenOccurrences)
            {
                if (item.Value.Item2 == '+' && item.Value.Item1 < 1)
                {
                    return false;
                }
                else if (item.Value.Item2 == '?' && item.Value.Item1 > 1)
                {
                    return false;
                }
                if (item.Value.Item2 == ' ' && item.Value.Item1 != 1)
                {
                    return false;
                }
            }
            foreach (var item in this.childrenInGroupOccurrences)
            {
                if (item.Value.Item2 == '+' && item.Value.Item1 < 1)
                {
                    return false;
                }
                else if (item.Value.Item2 == '?' && item.Value.Item1 > 1)
                {
                    return false;
                }
                if (item.Value.Item2 == ' ' && item.Value.Item1 != 1)
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasAChild(String name)
        {
            if(this.childrenOccurrences.ContainsKey(name))
            {
                return true;
            }
            foreach(var item in this.childrenInGroupOccurrences)
            {
                if(item.Key.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        public String Name { get; private set; }
        public String Content
        {
            get { return this.content; }
            set
            {
                if (this.ValidateContent(value) == true)
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
