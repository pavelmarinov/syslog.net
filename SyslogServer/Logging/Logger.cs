using SyslogServer.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer.Logging
{
    public class Logger
    {
        private List<ILogger> loggers = new List<ILogger>();

        public static Logger Instance { get; private set; }

        private Logger(List<ILogger> loggers)
        {
            this.loggers.AddRange(loggers);
        }

        public static void Init(ILogger logger)
        {
            if (Logger.Instance == null)
                Logger.Instance = new Logger(new List<ILogger>() { logger });
        }


        public void Assign(ILogger logger)
        {
            this.loggers.Add(logger);
        }

        public void Log(Severity severity,
          string message,
          params object[] args)
        {
            foreach (var logger in this.loggers)
                logger.Log(severity, message, args);
        }
    }
}
