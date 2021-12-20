using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Wikipedia_XML_Generator.Utils.XmlDTDValidator
{
    public class XMLValidator : IValidator
    {
        private string dtd = null;

        public XMLValidator(string dtd)
        {
            this.dtd = dtd;
        }

        public XMLValidator(Stream dtd)
        {
            if (dtd is not null && dtd.CanRead)
            {
                using (var reader = new StreamReader(dtd))
                {
                    this.dtd = reader.ReadToEnd();
                }
            }
        }

        public bool Validate(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                return Validate(doc);
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return false;
        }

        public bool Validate(Stream str)
        {
            try
            {
                var settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Parse;
                settings.ValidationType = ValidationType.DTD;
                settings.ValidationEventHandler += ValidationHandler;
                settings.XmlResolver = new XmlUrlResolver();

                using (var reader = XmlReader.Create(str, settings))
                {
                    while (reader.Read()) { }
                }
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return Result;
        }

        public bool Validate(XmlDocument doc)
        {
            try
            {
                if (doc.DocumentType is null)
                {
                    var markup = doc.CreateDocumentType(doc.DocumentElement.Name, null, null, this.dtd);
                    doc.InsertBefore(markup, doc.DocumentElement);
                }

                using (var stream = new MemoryStream())
                {
                    doc.Save(stream);
                    stream.Position = 0;
                    return Validate(stream);
                }
            }
            catch (Exception e)
            {
                Logger.LogAsync(Console.Out, e.Message);
            }

            return false;
        }

        private void ValidationHandler(object sender, ValidationEventArgs args)
        {
            this.Result = false;
        }

        public bool Result { get; private set; } = true;
    }
}
