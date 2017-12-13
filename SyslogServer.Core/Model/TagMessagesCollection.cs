using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    public class TagMessagesCollection
    {
        public string Host { get; set; }
        public string Tag { get; set; }
        public List<ISyslogMessage> Messages { get; set; }
        
        public TagMessagesCollection(string host, string tag)
        {
            this.Host = host;
            this.Tag = tag;
            this.Messages = new List<ISyslogMessage>();
        }
    }
}
