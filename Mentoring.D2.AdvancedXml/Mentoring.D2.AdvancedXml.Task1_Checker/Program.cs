using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace Mentoring.D2.AdvancedXml.Task1_Checker
{
    class Program
    {
        public const string Exit = "Exit";
        [STAThread]
        static void Main(string[] args)
        {

            Console.WriteLine("Please choose xml file.");
            var errorMessage = string.Empty;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                while (true)
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        errorMessage = Validator.CatalogValidator(ofd.FileName);
                    }

                    if (string.IsNullOrEmpty(errorMessage)) Console.WriteLine("You provided valid catalog");
                    else
                    {
                        Console.WriteLine("Errors:");
                        Console.WriteLine(errorMessage);
                    }

                    Console.WriteLine("Type 'Exit' to exit. Or anything else to continue checking");
                    var input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input) || !input.Equals(Exit, StringComparison.InvariantCultureIgnoreCase)) continue;
                    break;
                }
            }

        }
    }
}
