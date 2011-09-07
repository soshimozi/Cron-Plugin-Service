using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CronPluginService.Framework.Sheduling;
using System.ComponentModel.Composition;

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
