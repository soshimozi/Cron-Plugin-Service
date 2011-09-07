using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CronPluginService.Framework.Sheduling
{
    public interface IScheduledJob
    {
        void Execute(JobContext context);
    }
}
