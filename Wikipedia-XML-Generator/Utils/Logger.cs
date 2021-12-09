using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Wikipedia_XML_Generator.Utils
{
    public static class Logger
    {
        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss.ff]");
        }

        private static byte[] FormatMessage(string msg, string caller, string path)
        {
            var str = string.Format("{0} {1}::{2} => {3}\n", GetTimestamp(), path, caller, msg);
            return TypesConverter.StringToUTF8(str).Result;
        }

        public static void Log(Stream str, string msg, [CallerMemberName] string caller = "", [CallerFilePath] string path = "")
        {
            var buf = FormatMessage(msg, caller, path);
            str.Write(buf, 0, buf.Length);
        }

        public static void Log(TextWriter tw, string msg, [CallerMemberName] string caller = "", [CallerFilePath] string path = "")
        {
            tw.WriteLine(TypesConverter.UTF8ToString(FormatMessage(msg, caller, path)).Result);
        }

        public async static Task LogAsync(Stream str, string msg, [CallerMemberName] string caller = "", [CallerFilePath] string path = "")
        {
            var buf = FormatMessage(msg, caller, path);
            await str.WriteAsync(buf, 0, buf.Length);
        }

        public async static Task LogAsync(TextWriter tw, string msg, [CallerMemberName] string caller = "", [CallerFilePath] string path = "")
        {
            await tw.WriteLineAsync(TypesConverter.UTF8ToString(FormatMessage(msg, caller, path)).Result);
        }
    }
}
