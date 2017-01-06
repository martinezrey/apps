using EndpointClients.LoggingApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging.WebClient
{
    public class ApiLogServiceOptions : ILogServiceSettings
    {
        public bool IsUtcDateUsed { get; set; }
        public DirectoryInfo LogDir { get; set; }
        public LoggingApiClient LogClient { get; set; }

        public Func<string> GetFileName
        {
            get { throw new NotImplementedException(); }
        }
    }
}
