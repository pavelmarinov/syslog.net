using SyslogServer.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SyslogServer.WebServer.Code
{
    public class HtmlActionResult<T> : IHttpActionResult
    {
        private readonly string view;
        private T model;
        private string viewPath;

        public HtmlActionResult(string viewPath, T model)
        {
            this.model = model;
            this.viewPath = viewPath;
            this.view = HtmlActionResult<T>.LoadView(this.viewPath);
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var parsedView = RazorEngine.Razor.Parse<T>(view, this.model, this.viewPath);
            //var parsedView = RazorEngine.Razor.Parse(view, model);
            response.Content = new StringContent(parsedView);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
        }

        private static string LoadView(string viewPath)
        {
            var view = File.ReadAllText(viewPath);
            return view;
        }
    }
}
