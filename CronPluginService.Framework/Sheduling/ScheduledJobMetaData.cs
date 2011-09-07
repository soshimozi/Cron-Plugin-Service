using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace CronPluginService.Framework.Sheduling
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class ScheduledJobMetaData : ExportAttribute, IScheduledJobMetaData
    {
        public ScheduledJobMetaData()
        {
        }

        public ScheduledJobMetaData(string JobKey)
        {
            this.JobKey = JobKey;
        }

        #region IScheduledJobMetaData Members

        public string JobKey
        {
            get;
            set;
        }

        #endregion
    }
}
