using System;
using System.Threading;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Mentoring.D2.AOP.MergeService.Interfaces;
using Topshelf;

namespace Mentoring.D2.AOP.MergeService
{
    internal class PdfMergeService : ServiceControl
    {
        private readonly Timer _timer;
        private readonly IFolderMonitor _folderMonitor;
        private static IContainer Container { get; set; }

        public PdfMergeService()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LogInterceptor>();
            builder.RegisterType<FolderMonitor>().As<IFolderMonitor>().EnableInterfaceInterceptors().InterceptedBy(typeof(LogInterceptor));
            builder.RegisterType<FileManager>().As<IFileManager>().EnableInterfaceInterceptors().InterceptedBy(typeof(LogInterceptor));
            builder.RegisterType<MergeManager>().As<IMergeManager>().EnableInterfaceInterceptors().InterceptedBy(typeof(LogInterceptor));
            Container = builder.Build();

            using (var scope = Container.BeginLifetimeScope())
            {
                _folderMonitor = scope.Resolve<IFolderMonitor>();
            }
            _timer = new Timer(o => _folderMonitor.Process());
        }

        public bool Start(HostControl hostControl)
        {
            _timer.Change(0, int.Parse(Configuration.Period));
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _timer.Change(Timeout.Infinite, 0);
            return true;
        }
    }
}