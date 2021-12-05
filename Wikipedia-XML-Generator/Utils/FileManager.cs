using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace Wikipedia_XML_Generator.Utils
{
    public static class FileManager
    {
        public static int Write(string filepath, string value)
        {
            try
            {
                byte[] buf = StringToUTF8(value);
                File.WriteAllBytes(filepath, buf);
                return buf.Length;
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        public static int Read(string filepath, out string value)
        {
            try
            {
                byte[] buf = File.ReadAllBytes(filepath);
                value = UTF8ToString(buf);
                return buf.Length;
            }
            catch (Exception e)
            {
                value = "";
                return -1;
            }
        }

        private static byte[] StringToUTF8(string s)
        {
            return new UTF8Encoding(true).GetBytes(s);
        }

        private static string UTF8ToString(byte[] b)
        {
            return new UTF8Encoding(true).GetString(b);
        }
    }
}
