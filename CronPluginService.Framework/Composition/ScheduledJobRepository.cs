using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CronPluginService.Framework.Utility;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using CronPluginService.Framework.Scheduling;

namespace CronPluginService.Framework.Composition
{
    public class ScheduledJobRepository : SingletonBase<ScheduledJobRepository>
    {
        [ImportMany]
        private IEnumerable<Lazy<IScheduledJob, IScheduledJobMetaData>> _scheduledJobs;

        public void LoadPlugins(string [] directories)
        {
            var aggregateCatalog = new AggregateCatalog();

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            // an assembly catalog to load information about parts from this assembly
            var assemblyCatalog = new AssemblyCatalog(executingAssembly);

            foreach (string directory in directories)
            {
                if (!string.IsNullOrEmpty(directory))
                {
                    // search for dlls only in the specified directory
                    var directoryCatalog = new DirectoryCatalog(directory, "*.dll");
                    aggregateCatalog.Catalogs.Add(directoryCatalog);
                }
            }

            aggregateCatalog.Catalogs.Add(assemblyCatalog);

            // create a container for our catalogs
            var container = new CompositionContainer(aggregateCatalog);

            // clear old list, if any
            _scheduledJobs = null;

            // finally, compose the parts
            container.ComposeParts(this);
        }

        public Type GetTypeForJob(string jobKey)
        {
            var query = from lz in _scheduledJobs
                        where lz.Metadata.JobKey.Equals(jobKey)
                        select lz.Value;


            IScheduledJob job = query.FirstOrDefault();
            return job == null ? null : job.GetType();
        }
    }
}
