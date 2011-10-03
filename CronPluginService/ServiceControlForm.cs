using System;
using System.Windows.Forms;

namespace CronPluginService
{
    public partial class ServiceControlForm : Form
    {
        private readonly IServiceContext _context;
        public ServiceControlForm()
        {
            InitializeComponent();

            _context = ServiceControllerFactory.Instance.GetScheduledJobController();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            _context.Start(null);
        }

        private void PauseButtonClick(object sender, EventArgs e)
        {
            _context.Pause();
        }

        private void ResumeButtonClick(object sender, EventArgs e)
        {
            _context.Continue();
        }

        private void StopButtonClick(object sender, EventArgs e)
        {
            _context.Stop();
        }
     }
}
