using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Protocol
{
    //This is the public API
    public class SyslogParser : BaseSyslogParser
    {
        private ISyslogParser bsdSyslogParser;
        private ISyslogParser syslogProtocolParser;

        public SyslogParser(Dictionary<int, string> facilities)
            : base(facilities)
        {
            this.bsdSyslogParser = new BSDSyslogParser(facilities);
            this.syslogProtocolParser = new SyslogProtocolParser(facilities);
        }

        public override SyslogMessage Parse(string ipAddress, string packet)
        {
            SyslogMessage message;
            if (this.syslogProtocolParser.TryParse(ipAddress, packet, out message))
                return message;
            else if (this.bsdSyslogParser.TryParse(ipAddress, packet, out message))
                return message;
            else
                throw new ArgumentException("Invalid packet!");
        }
    }
}
