using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Mentoring.D2.AOP.MergeService.Interfaces;
using Topshelf;

namespace Mentoring.D2.AOP.MergeService
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
