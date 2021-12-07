using System;
using System.IO;
using System.Threading.Tasks;

namespace Wikipedia_XML_Generator.Utils
{
    public static class FileManager
    {
        public static int Write(string filepath, string value)
        {
            try
            {
                byte[] buf = StringConverter.StringToUTF8(value);
                File.WriteAllBytes(filepath, buf);
                return buf.Length;
            }
            catch (Exception e)
            {
                Logger.Log(Console.Out, e.Message);
                return -1;
            }
        }

        public static int Read(string filepath, out string value)
        {
            try
            {
                value = File.ReadAllText(filepath);
                return StringConverter.StringToUTF8(value).Length;
            }
            catch (Exception e)
            {
                Logger.Log(Console.Out, e.Message);
                value = string.Empty;
                return -1;
            }
        }

        public async static Task<int> WriteAsync(string filepath, string value)
        {
            try
            {
                byte[] buf = StringConverter.StringToUTF8(value);
                await File.WriteAllBytesAsync(filepath, buf);
                return buf.Length;
            }
            catch (Exception e)
            {
                Logger.Log(Console.Out, e.Message);
                return -1;
            }
        }

        public async static Task<string> ReadAsync(string filepath)
        {
            try
            {
                var value = await File.ReadAllTextAsync(filepath);
                return value;
            }
            catch (Exception e)
            {
                Logger.Log(Console.Out, e.Message);
                return string.Empty;
            }
        }
    }
}
