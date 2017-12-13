using SyslogServer.WebServer.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SyslogServer.WebServer.Controllers
{
    public class WebServerController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage Images(string file)
        {
            var response = new HttpResponseMessage();
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            MemoryStream rsrcStream = ResourceCache.Instance.GetResource(Path.Combine(base.GetResourceDirectory(), "img", file));
            if (rsrcStream != null)
            {
                response.Content = new StreamContent(rsrcStream);
                return response;
            }
            else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        public HttpResponseMessage Css(string file)
        {
            var response = new HttpResponseMessage();
            MemoryStream rsrcStream = ResourceCache.Instance.GetResource(Path.Combine(base.GetResourceDirectory(), file));
            if (rsrcStream != null)
            {
                response.Content = new StreamContent(rsrcStream);
                return response;
            }
            else
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}
