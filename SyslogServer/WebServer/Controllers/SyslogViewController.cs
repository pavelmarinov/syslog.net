using SyslogServer.Core.Model;
using SyslogServer.WebServer.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SyslogServer.WebServer.Controllers
{
    public class SyslogViewController : BaseApiController
    {
        [HttpGet]
        public HtmlActionResult<HostsCollection> HostView()
        {
            IStateQuery syslogState = base.GetSyslogState();
            List<Host> hosts = syslogState.GetHosts();
            HostsCollection coll = new HostsCollection() { Hosts = hosts };
            return new HtmlActionResult<HostsCollection>(Path.Combine(base.GetViewDirectory(), "Hosts.cshtml"), coll);
        }

        [HttpGet]
        public HtmlActionResult<Host> TagView(string host)
        {
            IStateQuery syslogState = base.GetSyslogState();
            Host h = syslogState.GetHost(host);
            return new HtmlActionResult<Host>(Path.Combine(base.GetViewDirectory(), "Tags.cshtml"), h);
        }

        [HttpGet]
        public HtmlActionResult<TagMessagesCollection> MessageView(string host, string tag)
        {
            IStateQuery syslogState = base.GetSyslogState();
            TagMessagesCollection msgCollection = syslogState.GetTagMessages(host, tag);
            msgCollection.Messages.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));
            return new HtmlActionResult<TagMessagesCollection>(Path.Combine(base.GetViewDirectory(), "Messages.cshtml"), msgCollection);
        }

        [HttpGet]
        public HtmlActionResult<SearchModel> Search([FromUri]SearchQuery search)
        {
            IStateQuery syslogState = base.GetSyslogState();
            TagMessagesCollection msgCollection = syslogState.GetTagMessages(search.Host, search.Tag);
            //msgCollection.Messages.Sort((x, y) => y.Timestamp.CompareTo(x.Timestamp));
            //TODO: This is stable sort but not in-place => possible memory pressure
            msgCollection.Messages = msgCollection.Messages.OrderByDescending(x => x.Timestamp).ToList();
            SearchModel searchModel = FilterUtility.ApplyFilters(search, msgCollection);
            return new HtmlActionResult<SearchModel>(Path.Combine(base.GetViewDirectory(), "Search.cshtml"), searchModel);
        }
    }
}
