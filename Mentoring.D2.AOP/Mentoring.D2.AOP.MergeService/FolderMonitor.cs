using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mentoring.D2.AOP.MergeService.Interfaces;

namespace Mentoring.D2.AOP.MergeService
{
    internal class FolderMonitor : IFolderMonitor
    {
        private readonly DirectoryInfo _filesDirectory;
        private readonly Dictionary<FileData, FileStatuses> _fileStatuses = new Dictionary<FileData, FileStatuses>();
        private readonly IFileManager _fileManager;
        private readonly IMergeManager _mergeManager;


        public FolderMonitor(IFileManager fileManager, IMergeManager mergeManager)
        {
            string inputFilesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");
            _filesDirectory = Directory.CreateDirectory(inputFilesFolder);
            _fileManager = fileManager;
            _mergeManager = mergeManager;
        }

        public void Process()
        {
            var maxCallsBeforeMerge = int.Parse(Configuration.Timeout);
            var filesToProcess = _filesDirectory.GetFiles().Select(fileInfo => fileInfo.Name).Except(_fileStatuses.Keys.Select(k => k.Name));
            ProcessFileNames(filesToProcess);
            var pendingFiles = _fileStatuses.Where(fs => fs.Value == FileStatuses.Pending).Select(fs => fs.Key).OrderBy(i => i.Number).ToList();
            if (SequenceInterrupted(pendingFiles))
            {
                return;
            }

            var file = _filesDirectory.GetFiles().Where(fi => pendingFiles.Select(fd => fd.Name).Contains(fi.Name))
                .OrderBy(fi => fi.CreationTime).LastOrDefault();
            var date = file?.CreationTime;
            if (date != null)
            {
                var addSeconds = ((DateTime)date).AddSeconds(maxCallsBeforeMerge);
                if (addSeconds < DateTime.Now)
                {
                    ProcessFiles(pendingFiles);
                }
            }
        }

        private void ProcessFiles(IEnumerable<FileData> pendingFiles)
        {
            var mergeIsSuccessfull = _mergeManager.MergeFiles(pendingFiles);
            ChangeStatus(pendingFiles, FileStatuses.Processing);
            var moveIsSuccessfull = _fileManager.MoveFiles(pendingFiles, mergeIsSuccessfull);
            var newStatus = moveIsSuccessfull ? FileStatuses.Done : FileStatuses.MoveError;
            ChangeStatus(pendingFiles, newStatus);
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
                    ProcessFiles(items);
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