using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz.Spi;
using CronPluginService.Framework.Plugin;
using Quartz;
using CronPluginService.Framework.Scheduling;

namespace CronPluginService.Framework.Plugin
{
    public class PluginJobFactory : IJobFactory
    {
        private readonly IPluginJob _pluginJob;
        public PluginJobFactory(IPluginJob pluginJob)
        {
            _pluginJob = pluginJob; 
        }

        #region IJobFactory Members
        public IJob NewJob(TriggerFiredBundle bundle)
        {
            return new PluginJobHandler(_pluginJob);
        }
        #endregion
    }
}
