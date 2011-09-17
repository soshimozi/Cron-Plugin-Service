using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using CronPluginService.Framework.Plugin;

namespace CronPluginService
{
    class ScheduledJob : IJob
    {
        private readonly IPluginJob _pluginJob;
        public ScheduledJob(IPluginJob pluginJob)
        {
            _pluginJob = pluginJob;
        }

        #region IJob Members
        public void Execute(JobExecutionContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
