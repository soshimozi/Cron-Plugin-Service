using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CronPluginService.Framework.Plugin
{
    public interface IPluginMetaData
    {
        string JobKey
        {
            get;
        }
    }
}
