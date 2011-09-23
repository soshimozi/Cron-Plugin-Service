using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServiceParameterCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CronServiceParameterElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CronServiceParameterElement)element).Name;
        }
    }
}
