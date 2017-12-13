using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Model
{
    public class SearchModel
    {
        public FilterModel FilterModel { get; set; }
        public PageOptions PageOptions { get; set; }
        public TagMessagesCollection MessagesCollection { get; set; }
    }
}
