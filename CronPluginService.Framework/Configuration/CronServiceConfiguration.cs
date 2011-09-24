using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServiceConfiguration : ConfigurationSection
    {
        //[ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        //public CronServiceInstanceCollection Instances
        //{
        //    get { return (CronServiceInstanceCollection)this[""]; }
        //    set { this[""] = value; }
        //}

        [ConfigurationProperty("Jobs", IsRequired = true)]
        public CronScheduledJobCollection Jobs
        {
            get { return (CronScheduledJobCollection)this["Jobs"]; }
            set { this["Jobs"] = value; }
        }

        //[ConfigurationProperty("JobParameters", IsRequired = true)]
        //public CronServiceJobParameterCollection JobParameters
        //{
        //    get { return (CronServiceJobParameterCollection)this["JobParameters"]; }
        //    set { this["JobParameters"] = value; }
        //}

        [ConfigurationProperty("PluginConfiguration", IsRequired = true)]
        public CronServicePluginInfoCollection PluginInfo
        {
            get { return (CronServicePluginInfoCollection)this["PluginConfiguration"]; }
            set { this["PluginConfiguration"] = value; }
        }
    
    }
}
