using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
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
        public ScheduledJobController()
        {
        }

        #region IServiceContext Members

        public void Start(string[] args)
        {
            if (_stopped)
            {
                _stopped = false;

                CronServiceConfiguration config =
                    (CronServiceConfiguration)ConfigurationManager.GetSection(
                    "CronServiceConfiguration");

                // clear any residual schedulers
                _schedulerManager.StopSchedulers();
                _schedulerManager.RemoveAll();

                List<string> directoryList = new List<string>();

                // let's read the list of plugininfos
                foreach (CronServicePluginInstanceElement plugin in config.PluginInfo)
                {
                    directoryList.Add(plugin.Path);
                }

                // now do our DI
                PluginRepository.Instance.LoadPlugins(directoryList.ToArray());

                // for each item, create a new scheduler
                foreach (CronScheduledJobElement element in config.Jobs)
                {
                    string cronExpression = element.Expression;

                    // we could use path, load the assembly
                    // and get the type from the assembly,
                    // but for now we just ask the current
                    // assembly
                    Type handlerType = Type.GetType(element.TypeString, false);
                    if (handlerType == null)
                    {
                        // let's see if we can find our type in the DI collection
                        handlerType = PluginRepository.Instance.GetTypeForJob(element.Name);
                    }

                    if( handlerType != null )
                    {
                        // get parameters for this job
                        List<Object> parameters = new List<Object>();
                        foreach (CronServiceParameterElement parameter in config.Parameters)
                        {
                            if (parameter.JobReference.Equals(element.Name))
                            {
                                Type parameterType = Type.GetType(parameter.DataType);
                                parameters.Add(Convert.ChangeType(parameter.Value, parameterType));
                            }
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
