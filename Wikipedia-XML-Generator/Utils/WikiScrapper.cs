using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Wikipedia_XML_Generator.Utils
{
    public static class WikiScrapper
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<Dictionary<string, Tuple<int, List<string>>>> GetContent(string url)
        {
            if(!Regex.IsMatch(url, "http[s]:\\/\\/[\\w\\-]*\\.wikipedia\\.org\\/wiki\\/[\\w]+"))
            {
                return null;
            }

            string domain = url.Substring(0, url.LastIndexOf("wiki/"));
            string page = url.Substring(url.LastIndexOf('/') + 1, url.Length - url.LastIndexOf('/') - 1);

            try
            {
                var response = await client.GetStringAsync(domain + "w/api.php?action=query&format=json&titles=" + page + "&prop=extracts&explaintext=");
                var json = JObject.Parse(response);
                var sections = new Dictionary<string, Tuple<int, List<string>>>();

                var currentTitle = json.SelectToken("$.query.pages.*.title").ToString();
                sections.Add(currentTitle, new Tuple<int, List<string>>(1, new List<string>()));

                Regex rg = new Regex("(==+)([\\w ]+)(==+)");

                foreach (var line in json.SelectToken("$.query.pages.*.extract").ToString().Split('\n'))
                {
                    if (line == "") continue;

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
                return null;
            }
        }
    }
}
