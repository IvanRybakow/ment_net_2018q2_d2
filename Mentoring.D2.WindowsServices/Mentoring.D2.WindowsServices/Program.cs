using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Runtime;

namespace Mentoring.D2.WindowsServices
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<PdfMergeService>();
                x.SetServiceName(Configuration.ServiceName);
                x.SetDisplayName(Configuration.DisplayName);
                x.StartAutomaticallyDelayed();
                x.RunAsLocalSystem();
            });
        }
    }
}
