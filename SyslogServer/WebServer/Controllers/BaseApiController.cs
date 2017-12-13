using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SyslogServer.WebServer.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        protected IStateQuery GetSyslogState()
        {
            IStateQuery syslogState;
            object syslogStateObj;
            if (this.Configuration.Properties.TryGetValue("syslog-state", out syslogStateObj))
                syslogState = (IStateQuery)syslogStateObj;
            else
                throw new InvalidOperationException("Cannot get syslog state instance");
            return syslogState;
        }

        [HttpGet]
        public HttpResponseMessage Redirect()
        {//Used to perform redirect from the default route
            HttpResponseMessage resp = new HttpResponseMessage(System.Net.HttpStatusCode.Redirect);
            resp.Headers.Location = new Uri(this.Request.RequestUri, "/syslogview/hostview");
            return resp;
        }

        protected string GetViewDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebServer" ,"Views");
        }

        protected string GetResourceDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebServer", "Resources");
        }
    }
}
