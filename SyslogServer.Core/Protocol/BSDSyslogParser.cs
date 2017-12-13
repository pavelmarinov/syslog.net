using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SyslogServer.Core.Protocol
{
    //BSD syslog protocol - RFC 3164
    public class BSDSyslogParser : BaseSyslogParser
    {
        private Regex bsdMessageRegex = new Regex(@"
(\<(?<PRI>\d{1,3})\>){0,1}
(?<HDR>
  (?<TIMESTAMP>(?<MMM>Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s
               (?<DD>[ 0-9][0-9])\s
	           (?<HH>[0-9]{2})\:(?<MM>[0-9]{2})\:(?<SS>[0-9]{2})
  )\s
  (?<HOSTNAME>[^ ]+?)\s
){0,1}
(?<MSG>.*)
", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);


        private Dictionary<string, int> months = new Dictionary<string, int>()
        {
            { "jan", 1 },
            { "feb", 2 },
            { "mar", 3 },
            { "apr", 4 },
            { "may", 5 },
            { "jun", 6 },
            { "jul", 7 },
            { "aug", 8 },
            { "sep", 9 },
            { "oct", 10 },
            { "nov", 11 },
            { "dec", 12 }
        };

        public BSDSyslogParser(Dictionary<int, string> facilities)
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

            Match m = bsdMessageRegex.Match(packet);
            if (!m.Success ||
                String.IsNullOrEmpty(m.Groups["PRI"].Value) ||
                String.IsNullOrEmpty(m.Groups["HDR"].Value) ||
                m.Groups["MSG"].Value == null)
            {
                throw new ArgumentException("Invalid message");
            }

            base.ParsePRI(m.Groups["PRI"].Value, out facility, out sev);
            timestamp = this.ParseDateTime(m);
            hostname = m.Groups["HOSTNAME"].Value ?? ipAddress;
            message = m.Groups["MSG"].Value;
            tag = this.SearchForTag(ref message);
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
                                              DateTime.Now.Year,
                                              MonthNumber(m.Groups["MMM"].Value),
                                              int.Parse(m.Groups["DD"].Value),
                                              int.Parse(m.Groups["HH"].Value),
                                              int.Parse(m.Groups["MM"].Value),
                                              int.Parse(m.Groups["SS"].Value)
                                            );
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


        private int MonthNumber(string monthName)
        {
            string monthKey = monthName.ToLower();
            int monthValue;
            if(this.months.TryGetValue(monthKey, out monthValue))
                return monthValue;
            else
                throw new ArgumentException("Unknown month: " + monthName);
        }

        private string SearchForTag(ref string message)
        {
            //Tag end is : or [
            int tagEndIndex = Array.FindIndex(message.ToCharArray(), c => (c == ':' || c == '['));
            if (tagEndIndex > 0)
            {//tag found
                string tag = message.Substring(0, tagEndIndex);
                message = message.Substring(tagEndIndex + 1);
                return tag;
            }
            return String.Empty;//tag not found
        }
    }
}
