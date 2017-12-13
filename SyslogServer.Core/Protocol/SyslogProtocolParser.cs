using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SyslogServer.Core.Protocol
{
    //New syslog protocol - RFC 5424
    //TODO: Parse structure data
    public class SyslogProtocolParser : BaseSyslogParser
    {
        private const string NIL_VALUE = "-";
        private Regex syslogMessageRegex = new Regex(@"
(?<HDR>
	(\<(?<PRI>\d{1,3})\>){0,1}
	(?<VER>\d{1})\s
	((?<TIMESTAMP>(?<Year>[0-9]{4})-(?<Month>[0-9]{2})-(?<Day>[0-9]{2})
        ([Tt](?<HH>[0-9]{2}):(?<MM>[0-9]{2}):(?<SS>[0-9]{2})(\.[0-9]+){0,1})?
        ([Tt](?<HH>[0-9]{2}):(?<MM>[0-9]{2}):(?<SS>[0-9]{2})(\\.[0-9]+){0,1})?
        (([Zz]|(?<OffsetSign>[+-])(?<OffsetHours>[0-9]{2}):(?<OffsetMinutes>[0-9]{2})))?
	)|(?<NIL>-))\s
	((?<HOSTNAME>[^ ]+?)|(?<NIL>-))\s
	((?<APPNAME>[^ ]+?)|(?<NIL>-))\s
	((?<PROCID>[^ ]+?)|(?<NIL>-))\s
	((?<MSGID>[^ ]+?)|(?<NIL>-))\s
	((?<SD>(\[.*\])+)|(?<NIL>-))	
)
(\s(?<MSG>.*))?
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        public SyslogProtocolParser(Dictionary<int, string> facilities)
            : base(facilities)
        {

        }

        public override SyslogMessage Parse(string ipAddress, string packet)
        {
            Severity sev; string facility;
            string hostname = null, message = null, tag = String.Empty;
            DateTime timestamp;
            if (String.IsNullOrEmpty(packet))
                throw new ArgumentException("packet");

            Match m = syslogMessageRegex.Match(packet);
            if(!m.Success)
                throw new ArgumentException("Invalid message");
            base.ParsePRI(m.Groups["PRI"].Value, out facility, out sev);
            hostname = m.Groups["HOSTNAME"].Value ?? ipAddress;
            message = m.Groups["MSG"].Value;
            //use APPNAME as tag as the RFC suggests
            if (!String.IsNullOrEmpty(m.Groups["APPNAME"].Value))
                tag = m.Groups["APPNAME"].Value;
            //add message ID and process ID to the message content
            string messageID = m.Groups["MSGID"].Value;
            if (!String.IsNullOrEmpty(messageID) && messageID != NIL_VALUE)
                message = messageID + " " + message;
            string procID = m.Groups["PROCID"].Value;
            if (!String.IsNullOrEmpty(procID) && procID != NIL_VALUE)
                message = procID + " " + message;
            timestamp = this.ParseDateTime(m);
            return new SyslogMessage(hostname, facility, sev, timestamp, tag, message.Trim());
        }

        private DateTime ParseDateTime(Match m)
        {
            DateTime timestamp;
            if (!String.IsNullOrEmpty(m.Groups["TIMESTAMP"].Value))
            {
                try
                {
                    timestamp = new DateTime(
                                              int.Parse(m.Groups["Year"].Value),
                                              int.Parse(m.Groups["Month"].Value),
                                              int.Parse(m.Groups["Day"].Value),
                                              int.Parse(m.Groups["HH"].Value),
                                              int.Parse(m.Groups["MM"].Value),
                                              int.Parse(m.Groups["SS"].Value)
                                            );
                    if(!String.IsNullOrEmpty(m.Groups["OffsetSign"].Value) && 
                       !String.IsNullOrEmpty(m.Groups["OffsetHours"].Value))
                    {
                        int offsetMinutes = 0;
                        if (int.TryParse(m.Groups["OffsetMinutes"].Value, out offsetMinutes)) { }
                        int offsetHours = int.Parse(m.Groups["OffsetHours"].Value);
                        if(m.Groups["OffsetSign"].Value == "-")
                            offsetHours *= -1;
                        timestamp = new DateTimeOffset(timestamp, new TimeSpan(offsetHours, offsetMinutes, 0)).DateTime;
                    }

                    return timestamp;
                }
                catch (ArgumentException)
                {
                    return DateTime.Now;
                }
            }
            else
            {
                return DateTime.Now;
            }
        }
    }
}
