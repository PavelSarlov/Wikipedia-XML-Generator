using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Wikipedia_XML_Generator.Utils.XmlDTDValidator
{
    public class DTDValidator : ValidationAttribute, IValidator
    {
        public DTDValidator() { }

        public bool Validate(string dtd)
        {
            try
            {
                var lines = Regex.Replace(dtd.Trim(), "[\r\n]", "").Split('<', '>').Where(x => x != string.Empty);
                var result = false;

                foreach (var l in lines)
                {
                    var line = Regex.Replace(l.Trim(), " +", " ");

                    result = line.Split(' ')[0] switch
                    {
                        "!ELEMENT" => IsValidElement(line),
                        "!ATTRLIST" => true,
                        _ => false
                    };
                }

                return result;
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return false;
            }
        }

        public bool Validate(Stream dtdStream)
        {
            try
            {
                using (var reader = new StreamReader(dtdStream))
                {
                    return Validate(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
                return false;
            }
        }

        private bool IsValidElement(string line)
        {
            var el = @"((?<!xml)[_[:alpha:]][\w-]*|#PCDATA)";
            var quant = @"([*?+]?)";
            var pattern = string.Format(@"!ELEMENT {0} (ANY|EMPTY|\(({0}{1}|\(?({0}{1}\|?)+(\){1})?,?)+\){1})", el, quant);

            return Regex.IsMatch(line, pattern);
        }
    }
}
