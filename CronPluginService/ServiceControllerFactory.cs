using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CronPluginService.Controller;

namespace CronPluginService
{
    class ServiceControllerFactory
    {
        private static ServiceControllerFactory _instance;

        protected ServiceControllerFactory() 
        {
        }

        public static ServiceControllerFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceControllerFactory();
                }

                return _instance;
            }
        }

        public IServiceContext GetScheduledJobController()
        {
            return new ScheduledJobController();
        }
    }
}
