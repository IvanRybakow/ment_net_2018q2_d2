using Akka.Actor;
using Mentoring.D2.SharedAssembly;
using Newtonsoft.Json;
using System;
using System.IO;
using Topshelf;

namespace Mentoring.D2.MessageQueue
{
    internal class PdfMergeCentralService : ServiceControl
    {
        private static ActorSystem _actorSystem;
        private readonly IActorRef _centralActor;
        private int _currentTimeOut;

        public PdfMergeCentralService()
        {
            _currentTimeOut = int.Parse(Configuration.Timeout);
            _actorSystem = ActorSystem.Create("PdfMergeCentralActorSystem");
            _centralActor = _actorSystem.ActorOf(Props.Create(() => new CentralActor()), "centralActor");
            var watcher = new FileSystemWatcher(Configuration.Folder);
            watcher.Changed += HandleSettingsChange;
            watcher.EnableRaisingEvents = true;
        }

        private void HandleSettingsChange(object sender, FileSystemEventArgs e)
        {
            string jsonSettings;
            try
            {
                jsonSettings = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Configuration.JsonConfig)); 
            }
            catch (IOException)
            {
                return;
            }

            FolderWatcherSettings settings;
            try
            {
                settings = JsonConvert.DeserializeObject<FolderWatcherSettings>(jsonSettings);
            }
            catch (JsonReaderException)
            {
                return;  
            }

            if (_currentTimeOut == settings.timeout) return;
            _currentTimeOut = settings.timeout;
            var message = new FolderWatcherSettings { timeout = settings.timeout };
            _centralActor.Tell(message);
        }

        public bool Start(HostControl hostControl)
        {
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _centralActor.Tell(PoisonPill.Instance);
            _actorSystem.Terminate();
            return true;
        }
        public void ProcessRequestStatus()
        {
            _centralActor.Tell(new RequestStatusMessage());
        }
    }
}
