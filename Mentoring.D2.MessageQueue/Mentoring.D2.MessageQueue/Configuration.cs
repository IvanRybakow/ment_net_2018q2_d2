using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentoring.D2.MessageQueue
{
    class Configuration
    {

        public static string Pattern => ConfigurationManager.AppSettings["pattern"];

        public static string ServiceName => ConfigurationManager.AppSettings["serviceName"];

        public static string DisplayName => ConfigurationManager.AppSettings["displayName"];
        public static string Timeout => ConfigurationManager.AppSettings["timeout"];
        public static string Period => ConfigurationManager.AppSettings["period"];
        public static string JsonConfig => ConfigurationManager.AppSettings["JsonConfig"];
        public static string Folder => ConfigurationManager.AppSettings["folderToWatch"];
    }
}
