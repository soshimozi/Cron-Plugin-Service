using System.Collections.Generic;
using System.Linq;
using Quartz;

namespace CronPluginService.Framework.Scheduling
{
    public class SchedulerManager
    {
        private readonly object _listLock = new object();
        private readonly IList<IScheduler> _scheduleList = new List<IScheduler>();

        public void AddScheduler(IScheduler scheduler)
        {
            lock (_listLock)
            {
                _scheduleList.Add(scheduler);
            }
        }

        public void RemoveScheduler(IScheduler scheduler)
        {
            lock (_listLock)
            {
                _scheduleList.Remove(scheduler);
            }
        }

        public void RemoveAll()
        {
            lock (_listLock)
            {
                _scheduleList.Clear();
            }
        }

        public void PauseSchedulers()
        {
            IList<IScheduler> copy;
            lock (_listLock)
            {
                copy = _scheduleList.ToList();
            }

            foreach (IScheduler s in copy)
            {
                s.PauseAll();
            }
        }

        public void ResumeSchedulers()
        {
            IList<IScheduler> copy;
            lock (_listLock)
            {
                copy = _scheduleList.ToList();
            }

            foreach (IScheduler s in copy)
            {
                s.ResumeAll();
            }
        }

        public void StopSchedulers()
        {
            IList<IScheduler> copy;
            lock (_listLock)
            {
                copy = _scheduleList.ToList();
            }

            foreach (IScheduler s in copy)
            {
                s.Shutdown();
            }
        }


        public void StartSchedulers()
        {
            IList<IScheduler> copy;
            lock (_listLock)
            {
                copy = _scheduleList.ToList();
            }

            foreach (IScheduler s in copy)
            {
                s.Start();
            }
        }
    }
}
