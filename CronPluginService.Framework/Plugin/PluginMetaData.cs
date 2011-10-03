using System;
using System.ComponentModel.Composition;

namespace CronPluginService.Framework.Plugin
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false), ExportAttribute]
    public class PluginMetaData : ExportAttribute, IPluginMetaData
    {
        public PluginMetaData() : this(string.Empty)
        {
        }

        public PluginMetaData(string jobKey) : base(typeof(IPluginJob))
        {
            JobKey = jobKey;
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
