using System;
using Topshelf;

namespace Mentoring.D2.MessageQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<PdfMergeCentralService>(s =>
                {
                    s.ConstructUsing(name => new PdfMergeCentralService());
                    s.WhenStarted(((service, control) => service.Start(control)));
                    s.WhenStopped((service, control) => service.Stop(control));
                    s.WhenCustomCommandReceived((tc, hc, command) => tc.ProcessRequestStatus());
                });
                x.SetServiceName(Configuration.ServiceName);
                x.SetDisplayName(Configuration.DisplayName);
                x.StartAutomaticallyDelayed();
                x.RunAsLocalSystem();
                    
            });
        }
    }
}
