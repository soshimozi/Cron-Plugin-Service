using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServicePluginInfoCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CronServicePluginInstanceElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CronServicePluginInstanceElement)element).Name;
        }
    }
}
