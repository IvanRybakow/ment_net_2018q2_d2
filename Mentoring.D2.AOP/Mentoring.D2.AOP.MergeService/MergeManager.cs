using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mentoring.D2.AOP.MergeService.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Mentoring.D2.AOP.MergeService
{
    internal class MergeManager : IMergeManager
    {
        private readonly string _filesDirectory;
        private readonly DirectoryInfo _completedDirectory;

        public MergeManager()
        {
            _filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files"); ;
            _completedDirectory = Directory.CreateDirectory(Path.Combine(_filesDirectory, "completed"));
        }

        public bool MergeFiles(IEnumerable<FileData> fileNames)
        {
            using (var output = new PdfDocument())
            {
                foreach (var file in fileNames)
                {
                    var page = output.AddPage();
                    using (XGraphics graphics = XGraphics.FromPdfPage(page))
                    using (XImage img = XImage.FromFile(Path.Combine(_filesDirectory, file.Name)))
                    {
                        try
                        {
                            graphics.DrawImage(img, 50, 50, 500, 500);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }

                }

                string generatedFileName = $"{string.Join("_", fileNames.Select(f => f.Number))}.pdf";
                output.Save(Path.Combine(_completedDirectory.FullName, generatedFileName));
                return true;
            }
        }
    }
}