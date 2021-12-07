using System.IO;

namespace Wikipedia_XML_Generator.Utils.XmlDTDValidator
{
    public interface IValidator
    {
        public bool Validate(string s);

        public bool Validate(Stream s);
    }
}
