using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using CronPluginService.Framework.Scheduling;

namespace CronPluginService.Framework.Plugin
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false), ExportAttribute]
    public class PluginMetaData : ExportAttribute, IPluginMetaData
    {
        public PluginMetaData() : this(string.Empty)
        {
        }

        public PluginMetaData(string JobKey) : base(typeof(IPluginJob))
        {
            this.JobKey = JobKey;
        }

        #region IPluginMetaData Members

        public string JobKey
        {
            get;
            set;
        }

        #endregion
    }
}
