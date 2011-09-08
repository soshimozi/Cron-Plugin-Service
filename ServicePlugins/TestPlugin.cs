using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using CronPluginService.Framework.Composition;
using CronPluginService.Framework.Scheduling;
using System.Diagnostics;

namespace ServicePlugins
{
    [Export(typeof(IScheduledJob))]
    [ScheduledJobMetaData(JobKey="TestPlugin-Mod")]
    public class TestPlugin : ScheduledJobBase
    {
        #region IScheduledJob Members

        public override void Execute(JobContext context)
        {
            Debug.WriteLine("Inside TestPlugin.Execute Mod 2");
        }

        #endregion
    }
}
