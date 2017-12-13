using Microsoft.Owin.Hosting;
using SyslogServer.Core.Protocol;
using SyslogServer.Core.Storage;
using SyslogServer.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SyslogServer
{
    public class SyslogService : ServiceBase
    {
        private bool disposed = false;
        SyslogServer syslogServer;
        IDisposable webServer;

        //WebServer configuration
        internal static HttpConfiguration HttpConfig { get; set; }

        public SyslogService()
        {
            this.ServiceName = "PM Syslog Server";
            this.EventLog.Log = "Application";
            this.CanHandlePowerEvent = false;
            this.CanHandleSessionChangeEvent = false;
            this.CanPauseAndContinue = false;
            this.CanShutdown = false;
            this.CanStop = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing)
            {
                if (this.syslogServer != null)
                    this.syslogServer.Dispose();
                if (this.webServer != null)
                    this.webServer.Dispose();
            }
            this.disposed = true;
            base.Dispose(disposing);
        }

        public void Start(string[] args)
        {
            this.OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Logger.Instance.Log(Severity.Informational, "{0}: starting", this.ServiceName);
                //TODO: Configuration
                string logRootDir = ConfigurationManager.AppSettings.Get("logs-root-dir") ?? @"C:\logs";
                int queueLimit = Int32.Parse(ConfigurationManager.AppSettings.Get("queue-limit") ?? "50000");
                int flushCount = Int32.Parse(ConfigurationManager.AppSettings.Get("flush-count") ?? "100");
                int flushInterval = Int32.Parse(ConfigurationManager.AppSettings.Get("flush-interval") ?? "5");
                string severity = ConfigurationManager.AppSettings.Get("min-severity") ?? "Informational";
                Severity minSeverity = (Severity)Enum.Parse(typeof(Severity), severity);
                IStorage storageHandler = new RollingFileStorage(logRootDir);
                string parserMode = ConfigurationManager.AppSettings.Get("parser-mode") ?? "GENERIC";
                this.syslogServer = new SyslogServer(queueLimit, flushCount, flushInterval, 
                                                     this.GetFacilities(), minSeverity, 
                                                     new List<IStorage> { storageHandler }, parserMode);
                this.InitHttpConfig();
                this.webServer = WebApp.Start<Startup>(url: "http://+:8514");
            }
            catch (Exception x)
            {
                Logger.Instance.Log(Severity.Alert, "{0}: {1}; aborting", this.ServiceName, x.Message);
                throw;
            }
        }

        protected override void OnStop()
        {
            Logger.Instance.Log(Severity.Informational, "{0}: stopping", this.ServiceName);
            this.syslogServer.SaveState();
            if (this.syslogServer != null)
                this.syslogServer.Dispose();
            if (this.webServer != null)
                this.webServer.Dispose();
        }

        private Dictionary<int, string> GetFacilities()
        {
            var facilities = new Dictionary<int, string>();
            facilities.Add((int)Facility.kernel, Facility.kernel.ToString());
            facilities.Add((int)Facility.user_level, Facility.user_level.ToString());
            facilities.Add((int)Facility.mail, Facility.mail.ToString());
            facilities.Add((int)Facility.system, Facility.system.ToString());
            facilities.Add((int)Facility.security, Facility.security.ToString());
            facilities.Add((int)Facility.syslogd, Facility.syslogd.ToString());
            facilities.Add((int)Facility.printer, Facility.printer.ToString());
            facilities.Add((int)Facility.news, Facility.news.ToString());
            facilities.Add((int)Facility.uucp, Facility.uucp.ToString());
            facilities.Add((int)Facility.clock9, Facility.clock9.ToString());
            facilities.Add((int)Facility.authorization, Facility.authorization.ToString());
            facilities.Add((int)Facility.ftp, Facility.ftp.ToString());
            facilities.Add((int)Facility.ntp, Facility.ntp.ToString());
            facilities.Add((int)Facility.log_audit, Facility.log_audit.ToString());
            facilities.Add((int)Facility.log_alert, Facility.log_alert.ToString());
            facilities.Add((int)Facility.clock15, Facility.clock15.ToString());
            facilities.Add((int)Facility.local0, Facility.local0.ToString());
            facilities.Add((int)Facility.local1, Facility.local1.ToString());
            facilities.Add((int)Facility.local2, Facility.local2.ToString());
            facilities.Add((int)Facility.local3, Facility.local3.ToString());
            facilities.Add((int)Facility.local4, Facility.local4.ToString());
            facilities.Add((int)Facility.local5, Facility.local5.ToString());
            facilities.Add((int)Facility.local6, Facility.local6.ToString());
            facilities.Add((int)Facility.local7, Facility.local7.ToString());
            facilities.Add((int)Facility.unknown, Facility.unknown.ToString());
            //TODO: Add configuration facilities
            return facilities;
        }

        private void InitHttpConfig()
        {
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "WebServer",
                routeTemplate: "{controller}/{action}/{file}"
            );

            config.Routes.MapHttpRoute(
                name: "SyslogWebDefault",
                routeTemplate: "{controller}/{action}",
                defaults: new { controller = "SyslogView", action = "Redirect" }
            );



            config.Properties.AddOrUpdate("syslog-state", this.syslogServer, (key, old) => this.syslogServer);

            SyslogService.HttpConfig = config;
        }
    }
}
