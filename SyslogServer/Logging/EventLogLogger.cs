using SyslogServer.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Logging
{
    public class EventLogLogger : ILogger
    {
        private Severity _minSeverity;
        private string source;
        private string logName;


        public EventLogLogger(Severity minSeverity = Severity.Informational, string source = "BSD Syslog Server", string logName = "Application")
        {
            this._minSeverity = minSeverity;
            this.source = source;
            this.logName = logName;
        }

        public bool IsEnabled(Severity severity)
        {
            return (severity <= this._minSeverity);
        }

        public void Log(Severity severity, string message, params object[] args)
        {
            try
            {
                EventLogEntryType entryType;
                switch (severity)
                {
                    case Severity.Debug:
                    case Severity.Informational:
                    case Severity.Notice:
                        entryType = EventLogEntryType.Information;
                        break;
                    case Severity.Warning:
                        entryType = EventLogEntryType.Warning;
                        break;
                    case Severity.Alert:
                    case Severity.Critical:
                    case Severity.Emergency:
                    case Severity.Error:
                        entryType = EventLogEntryType.Error;
                        break;
                    default:
                        entryType = EventLogEntryType.Warning;
                        break;
                }
                if (!EventLog.SourceExists(this.source))
                    EventLog.CreateEventSource(this.source, this.logName);

                EventLog.WriteEntry(source, String.Format(message, args), entryType);
            }
            catch (Exception)
            {
                //Make sure no errors are thrown here to avoid application crash.
            }
        }

        public void Dispose()
        {
            //nothing to do...
        }
    }
}
