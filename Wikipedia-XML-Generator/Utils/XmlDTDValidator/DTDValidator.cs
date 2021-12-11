using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml;

namespace Wikipedia_XML_Generator.Utils.XmlDTDValidator
{
    public class DTDValidator : ValidationAttribute, IValidator
    {
        public DTDValidator() { }

        public bool Validate(string dtd)
        {
            try
            {
                new XmlDocument().LoadXml(string.Format("<!DOCTYPE doc [{0}]><root/>", dtd));
                return true;
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

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(!Validate(value as string))
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }
    }
}
