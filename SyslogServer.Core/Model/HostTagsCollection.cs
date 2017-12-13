using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    public class HostTagsCollection
    {
        public string Host { get; private set; }

        public List<Tag> Tags { get; }

        public HostTagsCollection(string host)
        {
            this.Host = host;
            this.Tags = new List<Tag>();
        }
    }
}
