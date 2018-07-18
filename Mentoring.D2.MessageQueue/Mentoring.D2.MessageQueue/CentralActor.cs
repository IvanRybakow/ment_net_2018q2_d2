using Akka.Actor;
using CsvHelper;
using Mentoring.D2.Shared;
using Mentoring.D2.SharedAssembly;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mentoring.D2.MessageQueue
{
    class CentralActor : ReceiveActor
    {
        private readonly DirectoryInfo saveDirectory;
        private readonly DirectoryInfo reportDirectory;
        private IConnection connection;
        private IModel channel;
        private int currentFileNumber;
        public CentralActor()
        {
            reportDirectory = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports"));
            saveDirectory = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedFiles"));
            Receive<FolderWatcherSettings>(fws =>
            {
                var jsonMessage = JsonConvert.SerializeObject(fws);
                var body = Encoding.UTF8.GetBytes(jsonMessage);
                channel.ExchangeDeclare(SharedConfig.Exchange, ExchangeType.Topic);
                channel.BasicPublish(exchange: SharedConfig.Exchange, routingKey: SharedConfig.ChangeSettingsTopic, basicProperties: null, body: body);
                Console.WriteLine($"settings sent: {jsonMessage}");
            });

            Receive<RequestStatusMessage>(rsm =>
            {
                var jsonMessage = JsonConvert.SerializeObject(rsm);
                var body = Encoding.UTF8.GetBytes(jsonMessage);
                channel.ExchangeDeclare(SharedConfig.Exchange, ExchangeType.Topic);
                channel.BasicPublish(exchange: SharedConfig.Exchange, routingKey: SharedConfig.RequestStatusTopic, basicProperties: null, body: body);
                Console.WriteLine("status requested");
            });
        }
        protected override void PreStart()
        {
            currentFileNumber = saveDirectory.GetFiles()
                .Where(f => Regex.IsMatch(f.Name, Configuration.Pattern))
                .Select(f => Regex.Match(f.Name, Configuration.Pattern).Groups[1].Value)
                .Select(int.Parse).OrderBy(i => i).LastOrDefault();

            var factory = new ConnectionFactory() { HostName = SharedConfig.HostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(SharedConfig.Exchange, ExchangeType.Topic);
            channel.QueueDeclare(SharedConfig.FileSendTopic, false, false, false, null);

            var fileConsumer = new EventingBasicConsumer(channel);
            fileConsumer.Received += FileReceivedHandler;
            channel.BasicConsume(SharedConfig.FileSendTopic, true, fileConsumer);

            var statusQueueName = channel.QueueDeclare().QueueName;
            var statusConsumer = new EventingBasicConsumer(channel);
            statusConsumer.Received += StatusReceivedHandler;
            channel.BasicConsume(statusQueueName, true, statusConsumer);
            channel.QueueBind(statusQueueName, SharedConfig.Exchange, SharedConfig.ReportStatusTopic);

            base.PreStart();
        }

        private void StatusReceivedHandler(object sender, BasicDeliverEventArgs e)
        {
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);
            var status = JsonConvert.DeserializeObject<StatusMessage>(message);
            string fileName = Path.Combine(reportDirectory.FullName, $"{status.WorkerIdentifier}.csv");
            using (var sw = File.AppendText(fileName))
            using (var csv = new CsvWriter(sw))
            {
                if (new FileInfo(fileName).Length == 0)
                {
                    csv.WriteHeader<StatusMessage>();
                    csv.NextRecord();
                }
                csv.WriteRecord(status);
                csv.NextRecord();
            }

            Console.WriteLine($"Status received {message}");
        }

        private void FileReceivedHandler(object sender, BasicDeliverEventArgs e)
        {
            string filePath = Path.Combine(saveDirectory.FullName, $"document_{++currentFileNumber}.pdf");
            File.WriteAllBytes(filePath, e.Body);
            Console.WriteLine($"file saved: {filePath}");
        }

        protected override void PostStop()
        {
            connection.Dispose();
            channel.Dispose();
            base.PostStop();
        }
    }
}
