using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akka.Actor;
using Mentoring.D2.Shared;
using Mentoring.D2.SharedAssembly;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mentoring.D2.WindowsServices
{
    class FolderMonitorActor : ReceiveActor
    {
        private readonly IActorRef _mergeActor;
        private readonly IActorRef _fileManagerActor;
        private IConnection _connection;
        private IModel _channel;
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
        public FolderMonitorActor(string rootFolder)
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30), Context.Self, new RequestStatusMessage(), Context.Self);
            string inputFilesFolder = Path.Combine(rootFolder, "files");
            _filesDirectory = Directory.CreateDirectory(inputFilesFolder);
            _mergeActor = Context.ActorOf(Props.Create(() => new PdfMergeActor(inputFilesFolder)));
            _fileManagerActor = Context.ActorOf(Props.Create(() => new FileManagerActor(inputFilesFolder)));

            Receive<Process>(process =>
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
                        _mergeActor.Tell(new PdfMergeActor.MergeFiles(pendingFiles));
                        ChangeStatus(pendingFiles, FileStatuses.Processing);
                    }
                }

                
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
            Receive<RequestStatusMessage>(r =>
            {
                SendStatus();
            });
        }

        private void SendStatus()
        {
            var status = new StatusMessage();
            if (_fileStatuses.Any(kvp => kvp.Value == FileStatuses.Processing)) status.Status = Statuses.processing;
            else if (_fileStatuses.Any(kvp => kvp.Value == FileStatuses.Pending)) status.Status = Statuses.waitingForFiles;
            else status.Status = Statuses.idle;
            status.ReportTime = DateTime.Now;
            status.WorkerIdentifier = $"{Environment.MachineName}_{_filesDirectory.Name}";
            status.Settings = new FolderWatcherSettings { timeout = int.Parse(Configuration.Timeout) };
            var mess = JsonConvert.SerializeObject(status);
            var body = Encoding.UTF8.GetBytes(mess);
            _channel.ExchangeDeclare(SharedConfig.Exchange, ExchangeType.Topic);
            _channel.BasicPublish(exchange: SharedConfig.Exchange, routingKey: SharedConfig.ReportStatusTopic, basicProperties: null, body: body);
            Console.WriteLine($"status sent: {mess}");
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

        protected override void PreStart()
        {
            var factory = new ConnectionFactory() { HostName = SharedConfig.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(SharedConfig.Exchange, ExchangeType.Topic);

            var statusRequestQueueName = _channel.QueueDeclare().QueueName;
            var statusRequestConsumer = new EventingBasicConsumer(_channel);
            statusRequestConsumer.Received += StatusRequestedHandler;
            _channel.BasicConsume(statusRequestQueueName, true, statusRequestConsumer);
            _channel.QueueBind(statusRequestQueueName, SharedConfig.Exchange, SharedConfig.RequestStatusTopic);

            var settingsChangedQueueName = _channel.QueueDeclare().QueueName;
            var settingsChangedConsumer = new EventingBasicConsumer(_channel);
            settingsChangedConsumer.Received += ChangeSettingsReceivedHandler;
            _channel.BasicConsume(settingsChangedQueueName, true, settingsChangedConsumer);
            _channel.QueueBind(settingsChangedQueueName, SharedConfig.Exchange, SharedConfig.ChangeSettingsTopic);

            base.PreStart();
        }

        private void ChangeSettingsReceivedHandler(object sender, BasicDeliverEventArgs e)
        {
            var mess = Encoding.UTF8.GetString(e.Body);
            var settings = JsonConvert.DeserializeObject<FolderWatcherSettings>(mess);
            Configuration.Timeout = settings.timeout.ToString();
            Console.WriteLine($"settings received: {mess}");
        }

        private void StatusRequestedHandler(object sender, BasicDeliverEventArgs e)
        {
            SendStatus();
            Console.WriteLine("Status request received");
        }

        protected override void PostStop()
        {
            _connection.Dispose();
            _channel.Dispose();
            base.PostStop();
        }

    }
}
