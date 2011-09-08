using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using System.ComponentModel.Composition;

namespace CronPluginService.Framework.Scheduling
{
    public abstract class ScheduledJobBase : IPluginJob
    {
        #region IJob Members

        public void Execute(JobExecutionContext context)
        {
            Execute(new JobContext());
        }

        #endregion

        #region IScheduledJob Members

        public abstract void Execute(JobContext context);

        #endregion
    }
}
