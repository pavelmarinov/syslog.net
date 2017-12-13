using SyslogServer.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Logging
{
    public class ConsoleLogger : ILogger
    {
        public bool IsEnabled(Severity severity)
        {
            return true;
        }

        public void Log(Severity severity, string message, params object[] args)
        {
            if (args != null && args.Length > 0)
                message = String.Format(CultureInfo.InvariantCulture, message, args);
            message = String.Format(CultureInfo.InvariantCulture,
                                     "{0} {1} [{2}] {3}",
                                     DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture),
                                     severity.ToString(),
                                     System.Threading.Thread.CurrentThread.ManagedThreadId,
                                     message);
            Console.OutputEncoding = Encoding.UTF8;
            switch (severity)
            {
                case Severity.Informational:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Severity.Notice:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case Severity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Severity.Alert:
                case Severity.Critical:
                case Severity.Emergency:
                case Severity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            if (severity <= Severity.Error)
                Console.Error.WriteLine(message);
            else
                Console.WriteLine(message);
            Console.ResetColor();
        }

        public void Dispose()
        {
            // nothing to do
        }
    }
}
