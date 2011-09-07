using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
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
            var myEventLogInstaller = new EventLogInstaller();

            //// Set the Source of Event Log, to be created.
            myEventLogInstaller.Source = "Cron Plugin Service";

            //// Set the Log that source is created in
            myEventLogInstaller.Log = "Application";

            //set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "Cron Plugin Service";
            serviceInstaller.StartType = ServiceStartMode.Manual;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "Cron Plugin Service";

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
            this.Installers.Add(myEventLogInstaller);
        }
    }
}
