using System;
using CronPluginService.Framework.Utility;
using Quartz;
using Quartz.Impl;
using CronPluginService.Framework.Plugin;

namespace CronPluginService.Framework.Scheduling
{
    public class SchedulerFactory : SingletonBase<SchedulerFactory>
    {
        public IScheduler GetCronScheduler(string expression, Type handlerType, Object [] parameters)
        {
            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            string id = Guid.NewGuid().ToString();

            var job = new JobDetail("job"+id, "group"+id, typeof(PluginJobHandler));
            var trigger = new CronTrigger("trigger"+id, "group"+id, expression);

            sched.JobFactory = new PluginJobFactory(handlerType, parameters);

            // Tell quartz to schedule the job using our trigger
            sched.ScheduleJob(job, trigger);
            return sched;
        }
    }
}
