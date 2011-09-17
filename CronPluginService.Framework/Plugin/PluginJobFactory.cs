using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz.Spi;
using CronPluginService.Framework.Plugin;
using Quartz;
using CronPluginService.Framework.Scheduling;

namespace CronPluginService.Framework.Plugin
{
    public class PluginJobFactory : IJobFactory
    {
        private Type _pluginType;
        public PluginJobFactory(Type pluginType)
        {
            PluginType = pluginType;
        }

        #region IJobFactory Members
        public IJob NewJob(TriggerFiredBundle bundle)
        {
            return new PluginJobHandler(Activator.CreateInstance(PluginType) as IPluginJob);
        }
        #endregion

        public Type PluginType
        {
            get
            {
                return _pluginType;
            }

            set
            {
                if (!typeof(IPluginJob).IsAssignableFrom(value))
                {
                    throw new ArgumentException("Handler must implement the IPluginJob interface.");
                }

                _pluginType = value;
            }
        }
    }
}
