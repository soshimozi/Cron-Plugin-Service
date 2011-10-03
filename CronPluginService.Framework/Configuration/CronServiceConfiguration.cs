using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServiceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("Jobs", IsRequired = true)]
        public CronScheduledJobCollection Jobs
        {
            get { return (CronScheduledJobCollection)this["Jobs"]; }
            set { this["Jobs"] = value; }
        }

        [ConfigurationProperty("PluginConfiguration", IsRequired = true)]
        public CronServicePluginInfoCollection PluginInfo
        {
            get { return (CronServicePluginInfoCollection)this["PluginConfiguration"]; }
            set { this["PluginConfiguration"] = value; }
        }
    
    }
}
