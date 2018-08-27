using System;
using System.Collections.Generic;
using System.IO;
using Mentoring.D2.AOP.MergeService.Interfaces;

namespace Mentoring.D2.AOP.MergeService
{
    internal class FileManager : IFileManager
    {
        private readonly string _filesDirectory;
        private readonly DirectoryInfo _processedDirectory;
        private readonly DirectoryInfo _errorDirectory;

        public FileManager()
        {
            _filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files"); ;
            _processedDirectory = Directory.CreateDirectory(Path.Combine(_filesDirectory, "processed"));
            _errorDirectory = Directory.CreateDirectory(Path.Combine(_filesDirectory, "error"));
        }

        public bool MoveFiles(IEnumerable<FileData> files, bool isSuccessfull)
        {
            var destination = isSuccessfull ? _processedDirectory : _errorDirectory;
            foreach (var file in files)
            {
                try
                {
                    File.Move(Path.Combine(_filesDirectory, file.Name), Path.Combine(destination.FullName, file.Name));
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

    }
}