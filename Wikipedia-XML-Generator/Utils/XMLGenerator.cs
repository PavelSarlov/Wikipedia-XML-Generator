using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Wikipedia_XML_Generator.Models.DTD_Elements;

namespace Wikipedia_XML_Generator.Utils
{
    public class XMLGenerator
    {
        private DTDReader reader;

        public  XMLGenerator(String filepath)
        {
            this.reader = new DTDReader(filepath);
        }

        private Dictionary<String, String> getRelations(Dictionary<String, Tuple<int, List<String>>> elements)
        {
            Dictionary<String, String> relations = new Dictionary<String, String>();
            Stack<Tuple<String, int>> parents = new Stack<Tuple<string, int>>();
            foreach(var e in elements)
            {
                if(parents.Count() == 0)
                {
                    relations[e.Key] = null;
                    parents.Push(new Tuple<string, int>(e.Key, e.Value.Item1));
                    continue;
                }
                while(parents.Peek().Item2 >= e.Value.Item1)
                {
                    parents.Pop();
                }
                if(parents.Peek().Item2 < e.Value.Item1)
                {
                    relations[e.Key] = parents.Peek().Item1;
                }
                parents.Push(new Tuple<string, int>(e.Key, e.Value.Item1));
            }
            return relations;
        }

        private Dictionary<String, List<String>> getHierarchy(Dictionary<String, Tuple<int, List<String>>> elements)
        {
            Dictionary<String, List<String>> hierarchy = new Dictionary<string, List<string>>();
            foreach(var e in elements)
            {
                hierarchy[e.Key] = null;
            }



            return hierarchy;
        }

        //private bool validateXMLFile(out Dictionary<String, Tuple<int, List<String>>> elements)
        //{
        //    List<Element> DTDElements = this.reader.getElements();

        //}

        public async Task<StringBuilder> getXMLFromWikiTextAsync(String url)
        {
            Dictionary<String, Tuple<int, List<String>>> data = await WikiScrapper.GetContent(url);
            if (this.reader.Status == -1 || data == null)
            {
                return null;
            }
            //validateXMLFile(out data);
            StringBuilder sb = new StringBuilder();
            Stack<Tuple<String, int>> closingTabs = new Stack<Tuple<string, int>>();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf - 8\"?>");
            foreach(var item in data)
            {
                while(closingTabs.Count != 0 && closingTabs.Peek().Item2>=item.Value.Item1)
                {
                    sb.AppendLine("</" + closingTabs.Peek().Item1 + ">");
                    closingTabs.Pop();
                }
                sb.AppendLine("<" + item.Key + ">");
                for(int i = 0; i < item.Value.Item2.Count(); i++)
                {
                    sb.AppendLine(item.Value.Item2[i]);
                }
                closingTabs.Push(new Tuple<string, int>(item.Key, item.Value.Item1));
            }
            while (closingTabs.Count != 0)
            {
                sb.AppendLine("</" + closingTabs.Peek().Item1 + ">");
                closingTabs.Pop();
            }
            return sb;
        }
    }
}
