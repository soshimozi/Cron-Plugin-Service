using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        private void startButton_Click(object sender, EventArgs e)
        {
            _context.Start(null);
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            _context.Pause();
        }

        private void resumeButton_Click(object sender, EventArgs e)
        {
            _context.Continue();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            _context.Stop();
        }
     }
}
