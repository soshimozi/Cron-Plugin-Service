using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CronPluginService.Framework.Utility;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using CronPluginService.Framework.Plugin;

namespace CronPluginService.Framework.Scheduling
{
    public class SchedulerFactory : SingletonBase<SchedulerFactory>
    {
        public IScheduler GetCronScheduler(string expression, Type handlerType)
        {
            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            string id = Guid.NewGuid().ToString();
            // define the job and tie it to our HelloJob class
            JobDetail job = new JobDetail("job"+id, "group"+id, typeof(PluginJobHandler));

            CronTrigger trigger = new CronTrigger("trigger"+id, "group"+id, expression);

            sched.JobFactory = new PluginJobFactory(handlerType);

            // Tell quartz to schedule the job using our trigger
            sched.ScheduleJob(job, trigger);
            return sched;
        }

        public IScheduler GetPeriodScheduler<THandler>(int startSeconds, int periodicity)
        {
            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            // define the job and tie it to our HelloJob class
            JobDetail job = new JobDetail("job1", "group1", typeof(THandler));

            // Trigger the job to run on the next round minute
            CronTrigger trigger = new CronTrigger("trigger1", "group1", string.Format("{0}/{1} * * * * ?", startSeconds, periodicity));

            // Tell quartz to schedule the job using our trigger
            sched.ScheduleJob(job, trigger);

            return sched;
        }

        public IScheduler GetDailyScheduler<THandler>(TimeSpan timeOfDay)
        {
            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            // define the job and tie it to our HelloJob class
            JobDetail job = new JobDetail("job1", "group1", typeof(THandler));

            // Trigger the job to run on the next round minute
            CronTrigger trigger = new CronTrigger("trigger1", "group1", string.Format("{0} {1} {2} ? * *", timeOfDay.Seconds, timeOfDay.Minutes, timeOfDay.Hours));

            // Tell quartz to schedule the job using our trigger
            sched.ScheduleJob(job, trigger);

            return sched;
        }
    }
}
