using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Diagnostics;


namespace CronPluginService
{
    [RunInstaller(true)]
    public partial class CronPluginServiceInstaller : Installer
    {
        public CronPluginServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();
            var myEventLogInstaller = new EventLogInstaller {Source = "Cron Plugin Service", Log = "Application"};

            //// Set the Source of Event Log, to be created.

            //// Set the Log that source is created in

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "Cron Plugin Service";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "Cron Plugin Service";

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
            Installers.Add(myEventLogInstaller);
        }
    }
}
