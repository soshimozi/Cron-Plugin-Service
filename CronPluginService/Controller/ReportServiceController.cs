using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using Quartz;
using CronPluginService.Framework.Scheduling;
using CronPluginService.Jobs;

namespace CronPluginService.Controller
{
    class ReportServiceController : IServiceContext
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IScheduler _scheduler = null;

        private TimeSpan _reportTime;
        public ReportServiceController(TimeSpan reportTime)
        {
            _reportTime = reportTime;
        }

        #region IServiceContext Members

        public void Start(string [] args)
        {
            // create a new quartz instance 
            // using a daily trigger scheduler
            _scheduler = SchedulerFactory.Instance.GetDailyScheduler<GenerateManagementReportJob>(_reportTime);
            _scheduler.Start();
        
        }

        public void Stop()
        {
            // stop quartz scheduler
            // dispose of current quartz scheduler
            _scheduler.Shutdown();
            _scheduler = null;
        }

        public void Pause()
        {
            // pause quartz scheduler
            _scheduler.PauseAll();
        }

        public void Continue()
        {
            // unpause scheduler
            _scheduler.ResumeAll();
        }
        

        #endregion
    }
}
