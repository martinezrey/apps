using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging
{
    public interface ILogServiceSettings
    {
        bool IsUtcDateUsed { get;  }
        DirectoryInfo LogDir { get;  }
        Func<string> GetFileName { get; }
    }

    public interface ILogServiceAsync<out TOptions> : IDisposable
    where TOptions : ILogServiceSettings
    {
        TOptions Options { get; }

        void LogMessage(dynamic message);
    }
}
