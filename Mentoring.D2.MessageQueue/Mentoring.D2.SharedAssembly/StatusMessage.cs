using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentoring.D2.SharedAssembly
{
    public class StatusMessage
    {
        public DateTime ReportTime { get; set; }
        public string WorkerIdentifier { get; set; }
        public Statuses Status { get; set; }
        public FolderWatcherSettings Settings { get; set; }
    }
}
