using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Quartz;
using CronPluginService.Framework.Configuration;
using CronPluginService.Framework.Scheduling;
using CronPluginService.Framework.Plugin;

namespace CronPluginService.Controller
{
    class ScheduledJobController : IServiceContext
    {
        private bool _stopped = true;
        private readonly SchedulerManager _schedulerManager = new SchedulerManager();

        #region IServiceContext Members

        public void Start(string[] args)
        {
            if (_stopped)
            {
                _stopped = false;

                var config =
                    (CronServiceConfiguration)ConfigurationManager.GetSection(
                    "CronServiceConfiguration");

                // clear any residual schedulers
                _schedulerManager.StopSchedulers();
                _schedulerManager.RemoveAll();

                // let's read the list of plugininfos

                // now do our DI
                PluginRepository.Instance.LoadPlugins(
                    (from CronServicePluginInstanceElement plugin 
                     in config.PluginInfo 
                     select plugin.Path).
                     ToArray());

                // for each item, create a new scheduler
                foreach (CronScheduledJobElement element in config.Jobs)
                {
                    string cronExpression = element.Expression;

                    // we could use path, load the assembly
                    // and get the type from the assembly,
                    // but for now we just ask the current
                    // assembly
                    Type handlerType = Type.GetType(element.TypeString, false) ??
                                       PluginRepository.Instance.GetTypeForJob(element.Name);

                    if( handlerType != null )
                    {
                        // get parameters for this job
                        var parameters = new List<Object>();

                        if (element.Parameters != null)
                        {
                            parameters.AddRange(
                                from CronServiceParameterElement parameter 
                                in element.Parameters let parameterType = Type.GetType(parameter.DataType) 
                                where parameterType != null 
                                select Convert.ChangeType(parameter.Value, parameterType));
                        }

                        IScheduler scheduler = SchedulerFactory.Instance.GetCronScheduler(cronExpression, handlerType, parameters.ToArray());
                        _schedulerManager.AddScheduler(scheduler);
                    }
                }

                _schedulerManager.StartSchedulers();

            }
        }

        public void Stop()
        {
            _schedulerManager.StopSchedulers();
            _stopped = true;
        }

        public void Pause()
        {
            _schedulerManager.PauseSchedulers();   
        }

        public void Continue()
        {
            _schedulerManager.ResumeSchedulers();
        }

        #endregion
    }
}
