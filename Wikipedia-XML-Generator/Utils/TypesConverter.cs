using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Wikipedia_XML_Generator.Utils
{
    public static class TypesConverter
    {
        public async static Task<byte[]> StringToUTF8(string s)
        {
            try
            {
                return new UTF8Encoding(true).GetBytes(s);
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return null;
            }
        }


        public async static Task<string> UTF8ToString(byte[] b)
        {
            try
            {
                return new UTF8Encoding(true).GetString(b);
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return null;
            }
        }

        public async static Task<Stream> XmlToStream(XmlDocument doc)
        {
            try
            {
                var stream = new MemoryStream();

                XmlWriterSettings wsettings = new XmlWriterSettings();
                wsettings.Async = true;
                wsettings.Encoding = Encoding.UTF8;
                wsettings.Indent = true;

                using (var writer = XmlWriter.Create(stream, wsettings))
                {
                    doc.Save(writer);
                    await writer.FlushAsync();
                }

                stream.Position = 0;
                return stream;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return null;
            }
        }

        public async static Task<Stream> StringToSteam(String text)
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(text);
                MemoryStream stream = new MemoryStream(byteArray);
                return stream;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return null;
            }
        }
    }
}
