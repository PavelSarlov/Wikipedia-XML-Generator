﻿using Microsoft.AspNetCore.Http;
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
                byte[] buf = TypesConverter.StringToUTF8(value).Result;
                File.WriteAllBytes(filepath, buf);
                return buf.Length;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return -1;
            }
        }

        public static int Read(string filepath, out string value)
        {
            try
            {
                value = File.ReadAllText(filepath);
                return TypesConverter.StringToUTF8(value).Result.Length;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                value = string.Empty;
                return -1;
            }
        }

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

        public async static Task<int> WriteAsync(string filepath, string value)
        {
            try
            {
                byte[] buf = await TypesConverter.StringToUTF8(value);
                await File.WriteAllBytesAsync(filepath, buf);
                return buf.Length;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
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
                Logger.LogAsync(Console.Out, e.Message);
                return string.Empty;
            }
        }

        public async static Task<string> ReadAsync(IFormFile file)
        {
            try
            {
                string value = String.Empty;
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    value = await reader.ReadToEndAsync();
                }
                return value;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return string.Empty;
            }
        }
    }
}
