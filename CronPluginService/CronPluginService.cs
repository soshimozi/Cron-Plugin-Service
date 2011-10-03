using System.ServiceProcess;

namespace CronPluginService
{
    public partial class CronPluginService : ServiceBase
    {
        //private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        IServiceContext _serverContext;
        public CronPluginService()
        {
            InitializeComponent();

            _serverContext = ServiceControllerFactory.Instance.GetScheduledJobController();

        }

        protected override void OnStart(string[] args)
        {
            _serverContext.Start(args);
        }

        protected override void OnStop()
        {
            _serverContext.Stop();
            _serverContext = null;
        }

        protected override void OnPause()
        {
            _serverContext.Pause();
        }

        protected override void OnContinue()
        {
            _serverContext.Continue();
        }
    }
}
