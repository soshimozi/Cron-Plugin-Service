using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace CronPluginService.Framework.Plugin
{
    public abstract class PluginBase : IPluginJob, IJob
    {
        #region IPluginJob Members

        public abstract void Execute(PluginContext context);

        #endregion

        #region IJob Members

        public void Execute(JobExecutionContext context)
        {
            Execute(new PluginContext());
        }

        #endregion
    }
}
