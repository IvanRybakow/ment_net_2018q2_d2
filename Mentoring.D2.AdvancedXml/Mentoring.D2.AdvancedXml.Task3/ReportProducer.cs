using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Mentoring.D2.AdvancedXml.Task1_Checker;

namespace Mentoring.D2.AdvancedXml.Task3
{
    public class ReportProducer
    {
        public static void CreateReport(string input, string output, string transformationFile)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(output)) throw new ArgumentException("You should choose input and output files");
            if (!string.IsNullOrEmpty(Validator.CatalogValidator(input))) throw new ArgumentException("Provided file is not valid book catalog xml");
            var xsl = new XslCompiledTransform();
            xsl.Load(transformationFile);
            var xslParams = new XsltArgumentList();
            xslParams.AddParam("Date", "", DateTime.Now.ToShortDateString());
            using (FileStream fs = File.Create(output))
            {
                xsl.Transform(input, xslParams, fs);
            }
        }
    }
}
