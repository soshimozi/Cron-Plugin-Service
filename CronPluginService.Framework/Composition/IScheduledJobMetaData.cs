using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CronPluginService.Framework.Composition
{
    public interface IScheduledJobMetaData
    {
        string JobKey
        {
            get;
        }
    }
}
