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
    class FileManagerActor : ReceiveActor
    {
        private readonly string _filesDirectory;
        private readonly DirectoryInfo _processedDirectory;
        private readonly DirectoryInfo _errorDirectory;
        #region Message types

        public class MoveFilesToProcessedFolder
        {
            public MoveFilesToProcessedFolder(IEnumerable<FileData> fileNames)
            {
                FileNames = fileNames;
            }

            public IEnumerable<FileData> FileNames { get; private set; }
        }
        public class MoveFilesToErrorFolder
        {
            public MoveFilesToErrorFolder(IEnumerable<FileData> fileNames)
            {
                FileNames = fileNames;
            }

            public IEnumerable<FileData> FileNames { get; private set; }
        }
        #endregion

        public FileManagerActor(string rootFolder)
        {
            _filesDirectory = rootFolder;
            _processedDirectory = Directory.CreateDirectory(Path.Combine(rootFolder, "processed"));
            _errorDirectory = Directory.CreateDirectory(Path.Combine(rootFolder, "error"));

            Receive<MoveFilesToProcessedFolder>(fw =>
            {
                foreach (var files in fw.FileNames)
                {
                    try
                    {
                        File.Move(Path.Combine(_filesDirectory, files.Name), Path.Combine(_processedDirectory.FullName, files.Name));
                    }
                    catch (Exception)
                    {
                        Context.Sender.Tell(new FolderMonitorActor.ErrorMovingFile(files));
                    }
                }
                Context.Sender.Tell(new FolderMonitorActor.SuccessMovingFile(fw.FileNames));
            });
            Receive<MoveFilesToErrorFolder>(fw =>
            {
                foreach (var files in fw.FileNames)
                {
                    try
                    {
                        File.Move(Path.Combine(_filesDirectory, files.Name), Path.Combine(_errorDirectory.FullName, files.Name));
                    }
                    catch (Exception)
                    {
                        Context.Sender.Tell(new FolderMonitorActor.ErrorMovingFile(files));
                    }
                }
                Context.Sender.Tell(new FolderMonitorActor.SuccessMovingFile(fw.FileNames));
            });
        }

    }
}
