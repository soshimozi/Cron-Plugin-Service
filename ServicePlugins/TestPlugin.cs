using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using CronPluginService.Framework.Plugin;
using CronPluginService.Framework.Scheduling;
using System.Diagnostics;

namespace ServicePlugins
{
    [PluginMetaData(JobKey="TestPlugin")]
    public class TestPlugin : PluginBase
    {
        #region IScheduledJob Members

        public override void Execute(PluginContext context)
        {
            Debug.WriteLine("Inside TestPlugin.Execute");
        }

        #endregion
    }
}
