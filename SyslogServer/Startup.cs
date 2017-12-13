using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Net.Http.Headers;

[assembly: OwinStartup(typeof(SyslogServer.Startup))]

namespace SyslogServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            //HttpConfiguration config = new HttpConfiguration();

            //config.Routes.MapHttpRoute(
            //    name: "SyslogWebDefault",
            //    routeTemplate: "{controller}/{action}"
            //);
            SyslogService.HttpConfig.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            app.UseWebApi(SyslogService.HttpConfig); 
        }
    }
}
