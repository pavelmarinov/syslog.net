using SyslogServer.Core.Protocol;
using SyslogServer.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer
{
    class ServiceMain
    {
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            ServiceMain.SetupLogging();
            using (SyslogService service = new SyslogService())
            {
                try
                {
                    string command = (args.Length > 0 ? args[0] : String.Empty);
                    if (String.IsNullOrEmpty(command) && Environment.UserInteractive)
                        command = "--console";

                    switch (command)
                    {
                        case "-c":
                        case "--console":
                            Console.Title = service.ServiceName;
                            service.Start(args);
                            Console.WriteLine("Press Enter to stop...");
                            Console.ReadLine();
                            service.Stop();
                            return 0;

                        case "-i":
                        case "--install":
                        case "-u":
                        case "--uninstall":
                            if (command == "-i" || command == "--install")
                                SyslogServiceInstaller.Perform(true, null);
                            else
                                SyslogServiceInstaller.Perform(false, null);
                            return 0;

                        case "-s":
                        case "--start":
                        case "-t":
                        case "--stop":
                            using (ServiceController sc = new ServiceController(service.ServiceName))
                            {
                                ServiceControllerStatus targetStatus;
                                if (command == "-s" || command == "--start")
                                {
                                    Console.Write("Starting {0}...", service.ServiceName);
                                    sc.Start(args);
                                    targetStatus = ServiceControllerStatus.Running;
                                }
                                else
                                {
                                    Console.Write("Stopping {0}...", service.ServiceName);
                                    sc.Stop();
                                    targetStatus = ServiceControllerStatus.Stopped;
                                }
                                sc.WaitForStatus(targetStatus, TimeSpan.FromSeconds(30));
                                if (sc.Status == targetStatus)
                                    Console.WriteLine("done");
                                else
                                    Console.Error.WriteLine("timed out");
                                return (sc.Status == targetStatus ? 0 : 1);
                            }
                    }
                    
                    System.ServiceProcess.ServiceBase.Run(service);
                    return 0; // run as service
                }
                catch (Exception x)
                {
                    Logger.Instance.Log(Severity.Alert, "Syslog Service failed to start. Details: {0}", x.Message);
                    return 1; // an error occurred
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception x = e.ExceptionObject as Exception;
            Logger.Instance.Log(Severity.Alert, "{0}: {1}", x.Source, x.Message);
        }

        private static void SetupLogging()
        {
            string severity = ConfigurationManager.AppSettings.Get("log-minSeverity") ?? "Informational";

            Logger.Init(new EventLogLogger((Severity)Enum.Parse(typeof(Severity), severity)));
            if (Environment.UserInteractive)
                Logger.Instance.Assign(new ConsoleLogger());
        }
    }
}
