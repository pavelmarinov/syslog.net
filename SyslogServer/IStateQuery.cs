using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer
{
    public interface IStateQuery
    {
        List<Host> GetHosts();
        TagMessagesCollection GetTagMessages(string host, string tag);
        Host GetHost(string hostname);
    }
}
