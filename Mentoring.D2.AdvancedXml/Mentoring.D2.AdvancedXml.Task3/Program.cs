using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Xsl;

namespace Mentoring.D2.AdvancedXml.Task3
{
    class Program
    {

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            var inputFileName = string.Empty;
            var outputFileName = string.Empty;
            var error = false;

            using (var ofd = new OpenFileDialog())
            using (var sfd = new SaveFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK) inputFileName = ofd.FileName;
                if (sfd.ShowDialog() == DialogResult.OK) outputFileName = sfd.FileName;
            }

            try
            {
                ReportProducer.CreateReport(inputFileName, outputFileName, "BooksToHtmlReport.xslt");
            }
            catch (ArgumentException e)
            {
                error = true;
                Console.WriteLine(e.Message);
            }
            if (!error) System.Diagnostics.Process.Start(outputFileName);

            Console.ReadKey();
        }
    }
    
}
