using System.Xml;
using System.Xml.Schema;
using static System.String;

namespace Mentoring.D2.AdvancedXml.Task1_Checker
{
    public class Validator
    {
        public static string CatalogValidator(string inputFile)
        {
            string message = string.Empty;
            var settings = new XmlReaderSettings();
            settings.Schemas.Add(null, "BooksCatalogSchema.xsd");
            settings.ValidationEventHandler += (o, ea) => message += string.Concat($"[{ea.Exception.LineNumber},{ea.Exception.LinePosition}] - {ea.Exception.Message}", "\n");
            settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;
            try
            {
                using (var reader = XmlReader.Create(inputFile, settings))
                {
                    while (reader.Read()) { }
                }
            }
            catch (XmlException)
            {
                message = "Provided file is not valid XML";
            }

            return message;
        }
    }
}
