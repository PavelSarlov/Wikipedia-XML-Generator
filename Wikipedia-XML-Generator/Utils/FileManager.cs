using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Wikipedia_XML_Generator.Utils
{
    public static class FileManager
    {
        #region Writers
        public static int Write(Stream file, string value)
        {
            if (value is null) return 0;

            try
            {
                var bytes = TypesConverter.StringToUTF8(value).Result;
                file.Write(bytes);
                return bytes.Length;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return -1;
            }
        }

        public async static Task<int> WriteAsync(Stream file, string value)
        {
            if (value is null) return 0;

            try
            {
                var bytes = await TypesConverter.StringToUTF8(value);
                await file.WriteAsync(bytes);
                return bytes.Length;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return -1;
            }
        }
        #endregion

        #region Readers
        public static int Read(IFormFile file, out string value)
        {
            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    value = reader.ReadToEnd();
                }
                return TypesConverter.StringToUTF8(value).Result.Length;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                value = string.Empty;
                return -1;
            }
        }

        public static int Read(Stream file, out string value)
        {
            try
            {
                using (var reader = new StreamReader(file))
                {
                    value = reader.ReadToEnd();
                }
                return TypesConverter.StringToUTF8(value).Result.Length;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                value = string.Empty;
                return -1;
            }
        }

        public async static Task<string> ReadAsync(IFormFile file)
        {
            string value = String.Empty;

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    value = await reader.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return value;
        }

        public async static Task<string> ReadAsync(Stream file)
        {
            string value = string.Empty;

            try
            {
                using (var reader = new StreamReader(file))
                {
                    value = await reader.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return value;
        }
        #endregion
    }
}
