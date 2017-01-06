using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Logging
{
    public class LogServiceOptions : ILogServiceSettings
    {
        public bool IsUtcDateUsed { get; set; }

        public DirectoryInfo LogDir { get; set; }

        public Func<string> GetFileName
        {
            get {
                return new Func<string>(() =>
                {
                    return String.Format("{0}.txt", IsUtcDateUsed ? DateTime.UtcNow.ToString("MM.dd.yyyy")
                                                                                                : DateTime.Now.ToString("MM.dd.yyyy"));
                });
            }
        }

        public LogServiceOptions()
        {
            IsUtcDateUsed = false;
            LogDir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs"));
        }

        public LogServiceOptions(string logDir = null)
        {
            if (logDir == null)
                logDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs");

            IsUtcDateUsed = false;
            LogDir = new DirectoryInfo(logDir);
        }
    }

    public class WebLogServiceOptions : ILogServiceSettings
    {
        public bool IsUtcDateUsed { get; set; }

        public DirectoryInfo LogDir { get; set; }

        public Func<string> GetFileName
        {
            get
            {
                return new Func<string>(() =>
                {
                    return String.Format("{0}.txt", IsUtcDateUsed ? DateTime.UtcNow.ToString("MM.dd.yyyy")
                                                                                                : DateTime.Now.ToString("MM.dd.yyyy"));
                });
            }
        }

        public WebLogServiceOptions()
        {
            IsUtcDateUsed = true;
            LogDir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Logs"));
        }
    }
}
