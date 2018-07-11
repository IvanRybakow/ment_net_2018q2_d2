using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akka.Actor;

namespace Mentoring.D2.WindowsServices
{
    class FolderMonitorActor : ReceiveActor
    {
        private readonly int _maxCallsBeforeMerge;
        private int currenNumberOfCalls = 0;
        private readonly IActorRef _mergeActor;
        private readonly IActorRef _fileManagerActor;
        private readonly DirectoryInfo _filesDirectory;
        private readonly Dictionary<FileData, FileStatuses> _fileStatuses = new Dictionary<FileData, FileStatuses>();
        #region Message types

        public class Process { }
        public class ErrorMergingFiles
        {
            public ErrorMergingFiles(IEnumerable<FileData> fileNames)
            {
                FileNames = fileNames;
            }

            public IEnumerable<FileData> FileNames { get; private set; }
        }

        public class SuccessMergingFiles
        {
            public SuccessMergingFiles(IEnumerable<FileData> fileNames)
            {
                FileNames = fileNames;
            }

            public IEnumerable<FileData> FileNames { get; private set; }
        }
        public class ErrorMovingFile
        {
            public ErrorMovingFile(FileData file)
            {
                File = file;
            }

            public FileData File { get; private set; }
        }
        public class SuccessMovingFile
        {
            public SuccessMovingFile(IEnumerable<FileData> fileNames)
            {
                FileNames = fileNames;
            }

            public IEnumerable<FileData> FileNames { get; private set; }
        }
        #endregion
        public FolderMonitorActor(string rootFolder, int maxCallsBeforeMerge)
        {
            string inputFilesFolder = Path.Combine(rootFolder, "files");
            _filesDirectory = Directory.CreateDirectory(inputFilesFolder);
            _maxCallsBeforeMerge = maxCallsBeforeMerge;
            _mergeActor = Context.ActorOf(Props.Create(() => new PdfMergeActor(inputFilesFolder)), "PdfMergeActor");
            _fileManagerActor = Context.ActorOf(Props.Create(() => new FileManagerActor(inputFilesFolder)),
                "FileManagerActor");

            Receive<Process>(process =>
            {
                var filesToProcess = _filesDirectory.GetFiles().Select(fileInfo => fileInfo.Name).Except(_fileStatuses.Keys.Select(k => k.Name));
                ProcessFileNames(filesToProcess);
                var penfingFiles = _fileStatuses.Where(fs => fs.Value == FileStatuses.Pending).Select(fs => fs.Key).OrderBy(i => i.Number).ToList();
                if (SequenceInterrupted(penfingFiles))
                {
                    currenNumberOfCalls = 0;
                    return;
                }
                if (penfingFiles.Any() && currenNumberOfCalls >= maxCallsBeforeMerge)
                {
                    _mergeActor.Tell(new PdfMergeActor.MergeFiles(penfingFiles));
                    ChangeStatus(penfingFiles, FileStatuses.Processing);
                    currenNumberOfCalls = 0;
                    return;
                }

                currenNumberOfCalls += 1;
            });

            Receive<ErrorMergingFiles>(err => _fileManagerActor.Tell(new FileManagerActor.MoveFilesToErrorFolder(err.FileNames)));

            Receive<SuccessMergingFiles>(suc => _fileManagerActor.Tell(new FileManagerActor.MoveFilesToProcessedFolder(suc.FileNames)));

            Receive<ErrorMovingFile>(err => _fileStatuses[err.File] = FileStatuses.MoveError);

            Receive<SuccessMovingFile>(s =>
            {
                foreach (var file in s.FileNames)
                {
                    _fileStatuses[file] = FileStatuses.Done;
                }
            });
        }

        private bool SequenceInterrupted(List<FileData> penfingFiles)
        {
            if (!penfingFiles.Any()) return false;

            var toProcess = penfingFiles.GroupAdjacentBy((data, data2) => data.Number + 1 == data2.Number).ToList();
            if (toProcess.Count() > 1)
            {
                toProcess.RemoveAt(toProcess.Count - 1);
                foreach (var items in toProcess)
                {
                    _mergeActor.Tell(new PdfMergeActor.MergeFiles(items));
                    ChangeStatus(items, FileStatuses.Processing);
                }

                return true;
            }

            return false;
        }

        private void ProcessFileNames(IEnumerable<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                Match match = Regex.Match(fileName, Configuration.Pattern);
                if (!match.Success) continue;
                if (Configuration.FileExtensions.Contains(match.Groups[2].Value))
                {
                    var fileData = new FileData(match.Groups[2].Value, match.Groups[0].Value,
                        int.Parse(match.Groups[1].Value));
                    _fileStatuses[fileData] = FileStatuses.Pending;
                }
            }
        }

        private void ChangeStatus(IEnumerable<FileData> fileData, FileStatuses status)
        {
            foreach (var file in fileData)
            {

                _fileStatuses[file] = status;
            }
        }


    }
}
