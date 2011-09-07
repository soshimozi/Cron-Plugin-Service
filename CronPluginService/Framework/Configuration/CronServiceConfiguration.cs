using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServiceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public CronServiceInstanceCollection Instances
        {
            get { return (CronServiceInstanceCollection)this[""]; }
            set { this[""] = value; }
        }
    }
}
