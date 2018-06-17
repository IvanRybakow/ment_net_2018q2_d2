using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public class DownloadModel
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public DownloadStatus Status { get; set; }
        public string Data { get; set; }

    }

    public enum DownloadStatus
    {
        Loading,
        Ready,
        Error,
        Canceled
    }
}
