using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using CronPluginService.Framework.Composition;
using CronPluginService.Framework.Scheduling;

namespace ServicePlugins
{
    [Export(typeof(IScheduledJob))]
    [ScheduledJobMetaData(JobKey="DailyReports")]
    public class GenerateReportPlugin : IScheduledJob
    {
        #region IScheduledJob Members

        public void Execute(JobContext context)
        {
        }

        #endregion
    }
}
