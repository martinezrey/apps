using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Web.Api.Auth
{
    public class CustomLogServiceOptions : ILogServiceSettings
    {
        public bool IsUtcDateUsed { get; set; }

        public DirectoryInfo LogDir { get; set; }

        public Func<string> GetFileName
        {
            get
            {
                return new Func<string>(() =>
                {
                    string date = IsUtcDateUsed ? DateTime.UtcNow.ToString(ConfigurationManager.AppSettings["LogFileNameDateFormat"])
                                                : DateTime.Now.ToString(ConfigurationManager.AppSettings["LogFileNameDateFormat"]);

                    var fileName = String.Format("{0}{1}{2}", ConfigurationManager.AppSettings["LogFileName"], date, ConfigurationManager.AppSettings["LogFileExt"]);

                    return Path.GetFileName(fileName);
                });
            }
        }

        public CustomLogServiceOptions()
        {
            IsUtcDateUsed = true;
            LogDir = new DirectoryInfo(Path.GetDirectoryName(ConfigurationManager.AppSettings["LogFileName"]));
        }
    }
}