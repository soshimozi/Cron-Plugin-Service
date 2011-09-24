using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServiceJobParameterElement : ConfigurationElement
    {
        [ConfigurationProperty("jobkey", IsKey = true, IsRequired = true)]
        public string JobKey
        {
            get { return (string)base["jobkey"]; }
            set { base["jobkey"] = value; }
        }

        [ConfigurationProperty("Parameters", IsRequired = true)]
        public CronServiceParameterCollection Parameters
        {
            get { return (CronServiceParameterCollection)this["Parameters"]; }
            set { this["Parameters"] = value; }
        }

    }
}
