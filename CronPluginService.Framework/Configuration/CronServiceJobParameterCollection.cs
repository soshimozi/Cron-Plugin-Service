using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServiceJobParameterCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CronServiceJobParameterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CronServiceJobParameterElement)element).JobKey;
        }
    }
}
