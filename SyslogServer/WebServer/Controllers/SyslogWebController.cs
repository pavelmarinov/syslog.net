using SyslogServer.Core.Model;
using SyslogServer.WebServer.Code;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SyslogServer.WebServer.Controllers
{
    public class SyslogWebController : BaseApiController
    {

        [HttpGet]
        public HostsCollection Hosts([FromUri]string name = null)
        {
            IStateQuery syslogState = base.GetSyslogState();
            List<Host> hosts = syslogState.GetHosts();
            return new HostsCollection() { Hosts = hosts };
        }

        [HttpGet]
        public TagMessagesCollection Messages(string host, string tag)
        {
            IStateQuery syslogState = base.GetSyslogState();
            return syslogState.GetTagMessages(host, tag);
        }

        [HttpGet]
        public TagMessagesCollection Search([FromUri]SearchQuery search)
        {
            IStateQuery syslogState = base.GetSyslogState();
            TagMessagesCollection msgCollection = syslogState.GetTagMessages(search.Host, search.Tag);
            //msgCollection.Messages.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));
            //TODO: This is stable sort but not in-place => possible memory pressure
            msgCollection.Messages = msgCollection.Messages.OrderByDescending(x => x.Timestamp).ToList();
            SearchModel searchModel = FilterUtility.ApplyFilters(search, msgCollection);
            return searchModel.MessagesCollection;
        }
    }
}
