using System;
using SyslogServer.Core.Protocol;

namespace SyslogServer.Core.Model
{
    public interface ISyslogMessage
    {
        string Content { get; set; }
        string Facility { get; set; }
        string Hostname { get; set; }
        bool IsDirty { get; set; }
        Severity Severity { get; set; }
        string Tag { get; set; }
        DateTime Timestamp { get; set; }

        string ToString();
    }
}