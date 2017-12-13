using SyslogServer.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Logging
{

    public interface ILogger : IDisposable
    {
        bool IsEnabled(Severity severity);

        void Log(Severity severity,
                  string message,
                  params object[] args);
    }
}
