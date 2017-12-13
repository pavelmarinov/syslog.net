using SyslogServer.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    //This is the root of the host tree
    //Host
    //.....Tag
    //.........Message1 Message2.......MessageN
    //[Serializable]
    public class Host : IEqualityComparer<Host>
    {
        public string Name { get; set; }
        public List<Tag> Tags { get; set; }

        public Host(string name)
        {
            this.Name = name;
            this.Tags = new List<Tag>();
        }

        public int GetMessageSummaryCount()
        {
            int count = 0;
            foreach (Tag tag in this.Tags)
            {
                foreach (MessageSummaryModel msgModel in tag.Messages)
                    count += msgModel.Count;
            }
            return count;
        }

        public int GetMessageSummaryCount(Severity sev)
        {
            int count = 0;
            MessageSummaryModel curr;
            foreach (Tag tag in this.Tags)
            {
                curr = tag.Messages.First(m => m.Severity == sev);
                count += curr.Count;
            }
            return count;
        }

        public bool Equals(Host x, Host y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(Host obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
