using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CronPluginService.Utility;
using Quartz;
using Quartz.Impl;

namespace CronPluginService.Scheduling
{
    class SchedulerFactory : SingletonBase<SchedulerFactory>
    {
        public IScheduler GetCronScheduler(string expression, Type handlerType)
        {
            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            // define the job and tie it to our HelloJob class
            JobDetail job = new JobDetail("job"+handlerType.ToString(), "group"+handlerType.ToString(), handlerType);

            CronTrigger trigger = new CronTrigger("trigger"+handlerType.ToString(), "group"+handlerType.ToString(), expression);

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
