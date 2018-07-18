using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Mentoring.D2.Shared;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using RabbitMQ.Client;

namespace Mentoring.D2.WindowsServices
{
    class PdfMergeActor : ReceiveActor
    {
        private readonly string _filesDirectory;
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

            Receive<MergeFiles>(fw =>
            {
                using (var output = new PdfDocument())
                using (var memoryStream = new MemoryStream())
                {
                    foreach (var file in fw.FileNames)
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
                                Context.Sender.Tell(new FolderMonitorActor.ErrorMergingFiles(fw.FileNames));
                                return;
                            }
                        }

                    }
                    output.Save(memoryStream, true);
                    byte[] toSend = memoryStream.ToArray();
                    SendFileToQueue(toSend);
                    Context.Sender.Tell(new FolderMonitorActor.SuccessMergingFiles(fw.FileNames));
                }
            });
        }

        private void SendFileToQueue (byte[] file)
        {
            var factory = new ConnectionFactory() { HostName = SharedConfig.HostName };
            using (var conn = factory.CreateConnection())
            using (var channel = conn.CreateModel())
            {
                channel.QueueDeclare(SharedConfig.FileSendTopic, false, false, false, null);
                channel.BasicPublish(exchange: "", routingKey: SharedConfig.FileSendTopic, basicProperties: null, body: file);
                Console.WriteLine("File sent");
            }
        }

    }
}
