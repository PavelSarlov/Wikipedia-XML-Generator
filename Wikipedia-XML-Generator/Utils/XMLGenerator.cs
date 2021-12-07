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

        /*private bool validateXMLFile(out Dictionary<String, Tuple<int, List<String>>> elements)
        {
            List<Element> DTDElements = this.reader.getElements();

        }*/

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
