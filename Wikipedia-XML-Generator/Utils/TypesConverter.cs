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

        public async static Task<string> XmlToString(XmlDocument doc)
        {
            try
            {
                var result = new StringBuilder();
                using (var stream = new MemoryStream())
                {
                    doc.Save(stream);
                    stream.Position = 0;

                    XmlWriterSettings wsettings = new XmlWriterSettings();
                    wsettings.Async = true;
                    wsettings.Encoding = Encoding.UTF8;
                    wsettings.Indent = true;

                    XmlReaderSettings rsettings = new XmlReaderSettings();
                    rsettings.Async = true;

                    using (var writer = XmlWriter.Create(result, wsettings))
                    {
                        writer.WriteStartDocument();

                        using (var reader = XmlReader.Create(stream, rsettings))
                        {
                            while (await reader.ReadAsync())
                            {
                                await writer.WriteNodeAsync(reader, true);
                            }
                        }
                    }
                }

                return result.ToString();
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
                doc.Save(stream);
                stream.Position = 0;
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
