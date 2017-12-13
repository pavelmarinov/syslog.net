using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer
{
    [RunInstaller(true)]
    public class SyslogServiceInstaller : Installer
    {
        public SyslogServiceInstaller()
        {
            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;
            this.Installers.Add(processInstaller);

            Assembly callingAssembly = typeof(ServiceMain).Assembly;
            Type svcType = callingAssembly.GetType(typeof(SyslogService).ToString());
            using (ServiceBase svc = (ServiceBase)Activator.CreateInstance(svcType))
            {
                ServiceInstaller serviceInstaller = new ServiceInstaller();
                serviceInstaller.StartType = ServiceStartMode.Automatic;
                serviceInstaller.ServiceName = svc.ServiceName;
                serviceInstaller.Description = "PM Syslog Server";
                this.Installers.Add(serviceInstaller);
            }
        }

        public static void Perform(bool install, string[] args)
        {
            Assembly myAssembly = System.Reflection.Assembly.GetEntryAssembly();
            using (AssemblyInstaller installer = new AssemblyInstaller(myAssembly, args))
            {
                IDictionary state = new Hashtable();
                installer.UseNewContext = true;
                if (install)
                    try
                    { // install service
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch
                        {
                        }
                        throw;
                    }
                else
                {
                    // uninstall service
                    installer.Uninstall(state);
                }
            }
        }
    }
}
