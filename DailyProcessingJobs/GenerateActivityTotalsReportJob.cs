using System;
using CronPluginService.Framework.Plugin;
using DailyProcessingJobs.Model;
using log4net;
using System.Reflection;
using DailyProcessingJobs.Data;
using System.IO;
using System.Net.Mail;
using CronPluginService.Framework.Communication;

namespace DailyProcessingJobs
{
    [PluginMetaData(JobKey = "ActivityTotalsReport")]
    public class GenerateActivityTotalsReportJob : PluginBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _reportPath;
        private readonly string _reportName;
        private readonly string _worksheet;
        private readonly string _recipients;

        public GenerateActivityTotalsReportJob()
        {
        }

        public GenerateActivityTotalsReportJob(
            string reportPath, 
            string reportName, 
            string worksheet, 
            string recipients)
        {
            _reportPath = reportPath;
            _reportName = reportName;
            _worksheet = worksheet;
            _recipients = recipients;
        }

        public override void Execute(PluginContext context)
        {
            try
            {
                var generator = new ReportGenerator();
                Report report = ReportRepository.Instance.GetAcvityTotalsReport();

                Log.DebugFormat("Creating {0} at {1}", _reportName, _reportPath);

                string reportPath = Path.Combine(_reportPath, _reportName);

                InitializePath(_reportPath);

                generator.GenerateReport(reportPath, _worksheet, report, true, true);

                SendReport(reportPath);

                Log.DebugFormat("GenerateManagementReport Job ended @ {0}", DateTime.Now);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private void SendReport(string reportPath)
        {
            string[] recipients = _recipients.Split(';');
            MailMessage message = CommunicationManager.Instance.BuildMessage(
                recipients, _reportName, "");

            CommunicationManager.Instance.SendAttachment(reportPath, message);
        }

        private void InitializePath(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }
    }
}
