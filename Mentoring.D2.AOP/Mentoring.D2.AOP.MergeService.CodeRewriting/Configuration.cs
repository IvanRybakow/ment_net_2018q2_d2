using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentoring.D2.AOP.MergeService
{
    class Configuration
    {
        public static string DirectoryPath => ConfigurationManager.AppSettings["directoryPath"];

        public static string Pattern => ConfigurationManager.AppSettings["pattern"];

        public static List<string> FileExtensions
        {
            get
            {
                var fileExtensionsValue = ConfigurationManager.AppSettings["fileExtensions"];
                return fileExtensionsValue.Split(' ').ToList();
            }
        }

        public static string ServiceName => ConfigurationManager.AppSettings["serviceName"];

        public static string DisplayName => ConfigurationManager.AppSettings["displayName"];
        public static string Period => ConfigurationManager.AppSettings["period"];

        public static string ReportInterval => ConfigurationManager.AppSettings["reportSendInterval"];

        public static string Timeout
        {
            get => ConfigurationManager.AppSettings["timeout"];
            set
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["timeout"].Value = value;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
    }
}
