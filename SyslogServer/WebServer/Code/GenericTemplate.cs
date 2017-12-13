using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.WebServer.Code
{
    public class GenericTemplate<T> : TemplateBase<T>
    {
        public T Model { get; set; }

        public GenericTemplate()
        {
        }
    }
}
