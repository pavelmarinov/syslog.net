using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Core.Storage
{
    public interface IStorage
    {
        void Save(IEnumerable<ISyslogMessage> messages);
    }
}
