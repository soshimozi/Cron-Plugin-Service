using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace CronPluginService.Framework.Plugin
{
    class PluginJobHandler : IJob
    {
        private readonly IPluginJob _pluginJob;
        public PluginJobHandler(IPluginJob pluginJob)
        {
            _pluginJob = pluginJob;
        }

        #region IJob Members
        public void Execute(JobExecutionContext context)
        {
            _pluginJob.Execute(new PluginContext());
        }
        #endregion
    }
}
