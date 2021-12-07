using System.Text;

namespace Wikipedia_XML_Generator.Utils
{
    public static class StringConverter
    {
        public static byte[] StringToUTF8(string s)
        {
            return new UTF8Encoding(true).GetBytes(s);
        }

        public static string UTF8ToString(byte[] b)
        {
            return new UTF8Encoding(true).GetString(b);
        }
    }
}
