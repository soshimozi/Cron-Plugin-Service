using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServiceInstanceElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("expression", IsKey = false, IsRequired = true)]
        public string Expression
        {
            get { return (string)base["expression"]; }
            set { base["expression"] = value; }
        }

        [ConfigurationProperty("type", IsKey = false, IsRequired = true)]
        public string TypeString
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

        [ConfigurationProperty("path", IsKey = false, IsRequired = false)]
        public string Path
        {
            get { return (string)base["path"]; }
            set { base["path"] = value; }
        }

        [ConfigurationProperty("isgac", IsKey = false, IsRequired = false)]
        public bool IsGAC
        {
            get { return (bool)base["isgac"]; }
            set { base["isgac"] = value; }
        }
    }
}
