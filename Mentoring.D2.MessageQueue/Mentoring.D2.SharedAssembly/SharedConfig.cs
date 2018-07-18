using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentoring.D2.Shared

{
    public class SharedConfig
    {
        public static string Exchange => ConfigurationManager.AppSettings["exchangeName"];
        public static string HostName => ConfigurationManager.AppSettings["hostName"];
        public static string RequestStatusTopic => ConfigurationManager.AppSettings["requestStatusTopic"];
        public static string ReportStatusTopic => ConfigurationManager.AppSettings["reportStatusTopic"];
        public static string ChangeSettingsTopic => ConfigurationManager.AppSettings["changeSettingsTopic"];
        public static string FileSendTopic => ConfigurationManager.AppSettings["fileSendTopic"];
    }
}

