using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CronPluginService.Framework.Configuration
{
    public class CronServiceParameterElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("jobref", IsRequired = true)]
        public string JobReference
        {
            get { return (string)base["jobref"]; }
            set { base["jobref"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }

        [ConfigurationProperty("datatype", IsRequired = true)]
        public string DataType
        {
            get { return (string)base["datatype"]; }
            set { base["datatype"] = value; }
        }
    }
}
