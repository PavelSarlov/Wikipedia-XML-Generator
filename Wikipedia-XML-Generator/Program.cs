using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Wikipedia_XML_Generator.Models.DTD_Elements;
using Wikipedia_XML_Generator.Utils.DTDReader;
using Wikipedia_XML_Generator.Utils.XMLFileGenerator;

namespace Wikipedia_XML_Generator
{
    public class Program
    {
        public async static Task Main(string[] args)
        {

            DTDFileReader a = new DTDFileReader("./../../../../Desktop/a.dtd.txt");
            Dictionary<String, Element> b = a.GetElements();
            XMLGenerator c = new XMLGenerator("./../../../../Desktop/a.dtd.txt");
            XmlDocument d = await c.GetXMLFromWikiTextAsync("https://en.wikipedia.org/wiki/Bulgaria");
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
