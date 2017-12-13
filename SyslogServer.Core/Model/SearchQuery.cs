using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    public class SearchQuery
    {
        private string host;
        public string Host
        {
            get { return this.host; }
            set { this.host = value; }
        }

        private string tag;
        public string Tag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }


        private int page;
        public int Page
        {
            get { return this.page; }
            set { this.page = value; }
        }

        private string text;
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        private string severity;
        public string Severity
        {
            get { return this.severity; }
            set { this.severity = value; }
        }

        private string minSeverity;
        public string MinSeverity
        {
            get { return this.minSeverity; }
            set { this.minSeverity = value; }
        }

        private DateTime? startTime;
        public DateTime? StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }

        private DateTime? endTime;
        public DateTime? EndTime
        {
            get { return this.endTime; }
            set { this.endTime = value; }
        }

        public SearchQuery()
        {
            this.severity = String.Empty;
            this.Page = 1;
        }
    }
}
