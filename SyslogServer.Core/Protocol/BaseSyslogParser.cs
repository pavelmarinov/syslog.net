using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Protocol
{
    public abstract class BaseSyslogParser : ISyslogParser
    {
        protected Dictionary<int, string> Facilities;

        public BaseSyslogParser(Dictionary<int, string> facilities)
        {
            this.Facilities = facilities;
        }

        public void ParsePRI(string priorityString, out string facility, out Severity sev)
        {
            int priority = Int32.Parse(priorityString);
            if (priority > 0)
            {
                //Facility code is the nearest whole number of the priority value divided by 8
                int facilityCode = (int)Math.Floor((double)priority / 8);
                //Severity code is the remainder of the priority value divided by 8
                sev = (Severity)(priority % 8);
                if (this.Facilities.ContainsKey(facilityCode))
                    facility = this.Facilities[facilityCode];
                else
                    facility = Facility.unknown.ToString();
            }
            else
            {
                throw new ArgumentException("Invalid priority!");
            }
        }

        public abstract SyslogMessage Parse(string ipAddress, string packet);
        public virtual bool TryParse(string ipAddress, string packet, out SyslogMessage message)
        {
            try
            {
                message = this.Parse(ipAddress, packet);
                return true;
            }
            catch (Exception)
            {
                message = null;
                return false;
            }
        }
    }
}
