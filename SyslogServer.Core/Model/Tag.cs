using SyslogServer.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    //[Serializable]
    public class Tag : IEqualityComparer<Tag>
    {
        public string Name { get; }
        public string Facility { get; }
        public List<MessageSummaryModel> Messages { get; set; }

        public Tag(string name, string facility)
        {
            this.Name = name;
            this.Facility = facility;
            this.Messages = new List<MessageSummaryModel>();
            this.Messages.Add(new MessageSummaryModel() { Severity = Protocol.Severity.Alert, Count = 0 });
            this.Messages.Add(new MessageSummaryModel() { Severity = Protocol.Severity.Critical, Count = 0 });
            this.Messages.Add(new MessageSummaryModel() { Severity = Protocol.Severity.Debug, Count = 0 });
            this.Messages.Add(new MessageSummaryModel() { Severity = Protocol.Severity.Emergency, Count = 0 });
            this.Messages.Add(new MessageSummaryModel() { Severity = Protocol.Severity.Error, Count = 0 });
            this.Messages.Add(new MessageSummaryModel() { Severity = Protocol.Severity.Informational, Count = 0 });
            this.Messages.Add(new MessageSummaryModel() { Severity = Protocol.Severity.Notice, Count = 0 });
            this.Messages.Add(new MessageSummaryModel() { Severity = Protocol.Severity.Warning, Count = 0 });
        }

        public bool Equals(Tag x, Tag y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(Tag obj)
        {
            return obj.GetHashCode();
        }

        public void AddMessage(Severity sev)
        {
            MessageSummaryModel msgSummary = this.Messages.First(m => m.Severity == sev);
            msgSummary.Count++;
        }

        public int GetMessageSummaryCount(Severity sev)
        {
            int count = this.Messages.First(m => m.Severity == sev).Count;
            return count;
            
        }
    }
}
