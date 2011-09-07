using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using Quartz;
using CronPluginService.Framework.Service;
using CronPluginService.Framework.Configuration;
using CronPluginService.Framework.Scheduling;

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
                CronServiceConfiguration config =
                    (CronServiceConfiguration)ConfigurationManager.GetSection(
                    "CronServiceConfiguration");

                // clear any residual schedulers
                _schedulerManager.StopSchedulers();
                _schedulerManager.RemoveAll();

                // for each item, create a new scheduler
                foreach (CronServiceInstanceElement element in config.Instances)
                {
                    string cronExpression = element.Expression;
                    string path = element.Path; 

                    // we could use path, load the assembly
                    // and get the type from the assembly,
                    // but for now we just ask the current
                    // assembly
                    Type handlerType = Type.GetType(element.TypeString, false);
                    if (handlerType == null)
                    {
                        // TODO: handle where type string is invalid
                    }
                    else
                    {
                        IScheduler scheduler = SchedulerFactory.Instance.GetCronScheduler(cronExpression, handlerType);
                        _schedulerManager.AddScheduler(scheduler);
                    }
                }

                _schedulerManager.StartSchedulers();

                _stopped = false;
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
