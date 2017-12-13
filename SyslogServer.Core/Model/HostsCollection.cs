using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    //[Serializable]
    public class HostsCollection
    {
        public List<Host> Hosts { get; set; }

        public int EventsTotal
        {
            get
            {
                int count = 0;
                foreach (Host host in this.Hosts)
                    count += host.GetMessageSummaryCount();
                return count;
            }
        }
    }
}
