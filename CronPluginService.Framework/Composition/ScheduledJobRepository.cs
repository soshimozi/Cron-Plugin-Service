using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CronPluginService.Framework.Utility;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using CronPluginService.Framework.Sheduling;

namespace CronPluginService.Framework.Composition
{
    public class ScheduledJobRepository : SingletonBase<ScheduledJobRepository>
    {
        [ImportMany]
        private IEnumerable<Lazy<IScheduledJob, IScheduledJobMetaData>> _scheduledJobs;

        public void LoadPlugins(string directory)
        {
            var aggregateCatalog = new AggregateCatalog();

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            // an assembly catalog to load information about parts from this assembly
            var assemblyCatalog = new AssemblyCatalog(executingAssembly);

            if (!string.IsNullOrEmpty(directory))
            {
                var directoryCatalog = new DirectoryCatalog(directory, "*.dll");
                aggregateCatalog.Catalogs.Add(directoryCatalog);
            }

            aggregateCatalog.Catalogs.Add(assemblyCatalog);

            // create a container for our catalogs
            var container = new CompositionContainer(aggregateCatalog);

            // finally, compose the parts
            container.ComposeParts(this);
        }

        public Type GetTypeForJob(string jobKey)
        {
            throw new NotImplementedException();
        }
    }
}
