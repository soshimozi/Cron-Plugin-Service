using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronScheduledJobCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CronScheduledJobElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CronScheduledJobElement)element).Name;
        }
    }
}
