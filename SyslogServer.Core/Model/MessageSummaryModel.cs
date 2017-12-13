using SyslogServer.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    //[Serializable]
    public class MessageSummaryModel
    {
        public Severity Severity { get; set; }
        public int Count { get; set; }
    }
}
