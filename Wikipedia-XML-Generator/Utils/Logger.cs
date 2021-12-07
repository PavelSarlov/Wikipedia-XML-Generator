using System.IO;
using System.Threading.Tasks;

namespace Wikipedia_XML_Generator.Utils
{
    public static class Logger
    {
        public static void Log(Stream str, string msg)
        {
            var buf = StringConverter.StringToUTF8(msg + '\n');
            str.Write(buf, 0, buf.Length);
        }

        public static void Log(TextWriter tw, string msg)
        {
            foreach (var ch in msg + '\n')
            {
                tw.Write(ch);
            }
        }

        public async static Task LogAsync(Stream str, string msg)
        {
            var buf = StringConverter.StringToUTF8(msg + '\n');
            await str.WriteAsync(buf, 0, buf.Length);
        }

        public async static Task LogAsync(TextWriter tw, string msg)
        {
            foreach (var ch in msg + '\n')
            {
                await tw.WriteAsync(ch);
            }
        }
    }
}
