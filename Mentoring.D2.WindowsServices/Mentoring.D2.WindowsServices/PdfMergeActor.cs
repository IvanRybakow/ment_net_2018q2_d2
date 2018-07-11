using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Mentoring.D2.WindowsServices
{
    class PdfMergeActor : ReceiveActor
    {
        private readonly string _filesDirectory;
        private readonly DirectoryInfo _completedDirectory;
        #region Message types

        public class MergeFiles
        {
            public MergeFiles(IEnumerable<FileData> fileNames)
            {
                FileNames = fileNames;
            }

            public IEnumerable<FileData> FileNames { get; private set; }
        }

        #endregion

        public PdfMergeActor(string rootFolder)
        {
            _filesDirectory = rootFolder;
            _completedDirectory = Directory.CreateDirectory(Path.Combine(rootFolder, "completed"));

            Receive<MergeFiles>(fw =>
            {
                using (var output = new PdfDocument()) {
                    foreach (var file in fw.FileNames)
                    {
                        var page = output.AddPage();
                        XGraphics graphics = XGraphics.FromPdfPage(page);
                        try
                        {
                            XImage img = XImage.FromFile(Path.Combine(_filesDirectory, file.Name));
                            graphics.DrawImage(img, 50, 50, 500, 500);
                            img.Dispose();
                        }
                        catch (Exception)
                        {
                            Context.Sender.Tell(new FolderMonitorActor.ErrorMergingFiles(fw.FileNames));
                            return;
                        }
                    }

                    string generatedFileName = $"{string.Join("_", fw.FileNames.Select(f => f.Number))}.pdf";
                    output.Save(Path.Combine(_completedDirectory.FullName, generatedFileName));
                    Context.Sender.Tell(new FolderMonitorActor.SuccessMergingFiles(fw.FileNames));
                }
            });
        }

    }
}
