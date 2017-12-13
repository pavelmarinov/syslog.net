using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Protocol
{
    public interface ISyslogParser
    {
        SyslogMessage Parse(string ipAddress, string packet);
        bool TryParse(string ipAddress, string packet, out SyslogMessage message);
    }

    public enum Severity
    {
        Emergency,
        Alert,
        Critical,
        Error,
        Warning,
        Notice,
        Informational,
        Debug
    };


    public enum Facility
    {
        kernel,
        user_level,
        mail,
        system,
        security,
        syslogd,
        printer,
        news,
        uucp,
        clock9,
        authorization,
        ftp,
        ntp,
        log_audit,
        log_alert,
        clock15,
        local0,
        local1,
        local2,
        local3,
        local4,
        local5,
        local6,
        local7,
        unknown //25
    };
}
