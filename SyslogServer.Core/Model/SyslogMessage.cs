using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SyslogServer.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    //[Serializable]
    public class SyslogMessage : ISyslogMessage
    {
        public string Hostname { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Severity Severity { get; set; }
        public string Facility { get; set; }
        //[JsonConverter(typeof(JavaScriptDateTimeConverter))]
        public DateTime Timestamp { get; set; }
        public string Tag { get; set; }
        public string Content { get; set; }
        public bool IsDirty { get; set; }


        public SyslogMessage(string hostname, 
                             string facility, 
                             Severity severity, 
                             DateTime timestamp,
                             string tag,            
                             string message)
        {
            this.Hostname = hostname;
            this.Facility = facility;
            this.Severity = severity;
            this.Timestamp = timestamp;
            this.Content = message;
            this.Tag = tag;
            this.IsDirty = true;
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}] {2} {3}: {4}", this.Timestamp, this.Severity, this.Facility, this.Tag, this.Content);
        }
    }
}
