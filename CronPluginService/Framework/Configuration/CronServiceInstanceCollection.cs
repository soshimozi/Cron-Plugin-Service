using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServiceInstanceCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CronServiceInstanceElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CronServiceInstanceElement)element).Name;
        }
    }
}
