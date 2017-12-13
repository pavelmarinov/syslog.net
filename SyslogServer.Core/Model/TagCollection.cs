using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    //[Serializable]
    public class TagCollection
    {
        public List<Tag> Tags { get; set; }
        
        public TagCollection()
        {
            this.Tags = new List<Tag>();
        }
    }
}
