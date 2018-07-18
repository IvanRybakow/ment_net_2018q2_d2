using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Topshelf;

namespace Mentoring.D2.WindowsServices
{
    class PdfMergeService : ServiceControl
    {
        private readonly Timer _timer;
        private static ActorSystem _actorSystem;
        private readonly IActorRef _folderMonitorActor;

        public PdfMergeService()
        {
            _actorSystem = ActorSystem.Create("PdfMergeActorSystem");
            Props folderMonitorProps = Props.Create(() => new FolderMonitorActor(AppDomain.CurrentDomain.BaseDirectory));
            _folderMonitorActor = _actorSystem.ActorOf(folderMonitorProps, "folderMonitorActor");

            _timer = new Timer((o) => _folderMonitorActor.Tell(new FolderMonitorActor.Process())); 
        }
        public bool Start(HostControl hostControl)
        {
            _timer.Change(0, int.Parse(Configuration.Period));
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _timer.Change(Timeout.Infinite, 0);
            _folderMonitorActor.Tell(PoisonPill.Instance);
            _actorSystem.Terminate();
            return true;
        }
    }
}
