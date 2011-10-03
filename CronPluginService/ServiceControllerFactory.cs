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
                return _instance ?? (_instance = new ServiceControllerFactory());
            }
        }

        public IServiceContext GetScheduledJobController()
        {
            return new ScheduledJobController();
        }
        //public IServiceContext GetReportController(TimeSpan reportTime)
        //{
        //    return new ReportServiceController(reportTime);
        //}
    }
}
