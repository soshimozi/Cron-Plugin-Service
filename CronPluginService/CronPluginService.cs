using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using log4net;
using System.Reflection;
using CronPluginService.Framework.Service;

namespace CronPluginService
{
    public partial class CronPluginService : ServiceBase
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        IServiceContext serverContext;
        public CronPluginService()
        {
            InitializeComponent();

            serverContext = ServiceControllerFactory.Instance.GetScheduledJobController();

        }

        protected override void OnStart(string[] args)
        {
            serverContext.Start(args);
        }

        protected override void OnStop()
        {
            serverContext.Stop();
            serverContext = null;
        }

        protected override void OnPause()
        {
            serverContext.Pause();
        }

        protected override void OnContinue()
        {
            serverContext.Continue();
        }
    }
}
