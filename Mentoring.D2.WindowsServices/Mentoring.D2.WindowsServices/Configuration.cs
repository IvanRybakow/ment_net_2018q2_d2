using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentoring.D2.WindowsServices
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

    }
}

