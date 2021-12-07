using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace Wikipedia_XML_Generator.Utils
{
    public static class WikiScrapper
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string urlPattern = "http[s]:\\/\\/[\\w\\-]*\\.wikipedia\\.org\\/wiki\\/[\\w]+";
        private static readonly string apiQuery = "w/api.php?action=query&format=json&prop=extracts&explaintext=&titles=";

        public static async Task<Dictionary<string, Tuple<int, List<string>>>> GetContent(string url)
        {
            if(!Regex.IsMatch(url, urlPattern))
            {
                return null;
            }

            string domain = url.Substring(0, url.LastIndexOf("wiki/"));
            string page = url.Substring(url.LastIndexOf('/') + 1, url.Length - url.LastIndexOf('/') - 1);

            try
            {
                var response = await client.GetStringAsync(domain + apiQuery + page + "");
                var json = JObject.Parse(response);
                var sections = new Dictionary<string, Tuple<int, List<string>>>();

                var currentTitle = json.SelectToken("$.query.pages.*.title").ToString();
                sections.Add(currentTitle, new Tuple<int, List<string>>(1, new List<string>()));

                Regex rg = new Regex("(==+)([\\w ]+)(==+)");

                foreach (var line in json.SelectToken("$.query.pages.*.extract").ToString().Split('\n'))
                {
                    if (line == string.Empty) continue;

                    var match = rg.Match(line);

                    if (match.Success)
                    {
                        currentTitle = match.Groups[2].Value.Trim().Replace(' ', '_');
                        sections.Add(currentTitle, new Tuple<int, List<string>>(match.Groups[1].Length, new List<string>()));
                    }
                    else
                    {
                        sections[currentTitle].Item2.Add(line);
                    }
                }

                return sections;
            }
            catch (Exception e)
            {
                await Logger.LogAsync(Console.Out, e.Message);
                return null;
            }
        }

        public async static Task<XmlDocument> GetXml(string url)
        {
            var contents = await GetContent(url);

            if (contents is null) return null;

            try
            {
                var doc = new XmlDocument();
                doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                var asc = new Stack<XmlNode>();
                asc.Push(doc);

                foreach (var section in contents)
                {
                    while (asc.Count - 1 >= section.Value.Item1)
                    {
                        asc.Pop();
                    }

                    var el = doc.CreateElement(string.Empty, section.Key, string.Empty);
                    if (section.Value.Item2.Count > 0)
                    {
                        el.AppendChild(doc.CreateTextNode(string.Join('\n', section.Value.Item2)));
                    }
                    asc.Peek().AppendChild(el);
                    asc.Push(el);
                }

                return doc;
            }
            catch (Exception e)
            {
                await Logger.LogAsync(Console.Out, e.Message);
                return null;
            }
        }
    }
}
