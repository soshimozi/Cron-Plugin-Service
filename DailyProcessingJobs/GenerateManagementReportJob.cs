using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using System.Reflection;
using log4net;
using System.IO;
using System.Configuration;
using System.Data;
using System.ComponentModel.Composition;
using CronPluginService.Framework.Plugin;
using DailyProcessingJobs.Model;
using DailyProcessingJobs.Data;
using DocumentFormat.OpenXml.Packaging;

namespace DailyProcessingJobs
{
    [PluginMetaData(JobKey = "DailyActivityReport")]
    public class GenerateManagementReportJob : PluginBase
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public override void Execute(PluginContext context)
        {
            // TODO: Move into app.config or something?
            string path = string.Format("c:\\dev\\ActivityReport_{0:MMM}_{0:yyyy}.xlsx", DateTime.Now);
            Log.DebugFormat("Creating workbook at {0}", path);

            string connectionString =
                     ConfigurationManager.ConnectionStrings["ManagementReporting"].ConnectionString;

            ActivityReport report = ReportRepository.Instance.GetActivityReport(connectionString);
            if (report != null)
            {
                GenerateDailyActivityReport(report, path);
            }

            Log.DebugFormat("GenerateManagementReport Job ended @ {0}", DateTime.Now);
        }

        public void GenerateDailyActivityReport(ActivityReport report, string workbookPath)
        {
            string worksheetName = "DailyActivity_" + DateTime.Now.ToString("MM_dd_yyyy");

            // Open the document for editing.
            using (SpreadsheetDocument spreadSheet = WorkbookHelper.OpenOrCreateWorkbook(workbookPath, worksheetName))
            {
                if (WorkbookHelper.IsWorksheetPresent(spreadSheet.WorkbookPart, worksheetName))
                {
                    WorkbookHelper.DeleteWorksheet(spreadSheet.WorkbookPart, worksheetName);
                }

                // Insert a new worksheet.
                WorksheetPart worksheetPart = WorkbookHelper.InsertWorksheet(spreadSheet.WorkbookPart, worksheetName);

                AddColumns(spreadSheet.WorkbookPart, worksheetPart, report.ColumnNames);
                AddLines(spreadSheet.WorkbookPart, worksheetPart, report);

                WorkbookHelper.CreateTable(
                    spreadSheet.WorkbookPart,
                    worksheetPart,
                    report.ColumnNames.ToArray(),
                    1, 1,
                    report.ColumnNames.Count,
                    report.ReportLines.Count
                );

            }
        }

        private void AddLines(WorkbookPart workbookPart, WorksheetPart worksheetPart, ActivityReport report)
        {
            for (int iRow = 0; iRow < report.ReportLines.Count; iRow++)
            {
                for (int iCol = 0; iCol < report.ColumnNames.Count; iCol++)
                {
                    WorkbookHelper.AddCellText(
                        workbookPart,
                        worksheetPart,
                        report[iRow][iCol].ToString(),
                        (uint)iCol + 1, (uint)iRow + 2);
                }
            }
        }

        private void AddColumns(WorkbookPart workbookPart, WorksheetPart worksheetPart, List<string> columnNames)
        {
            for (int i = 0; i < columnNames.Count; i++)
            {
                string fieldName = columnNames[i];

                WorkbookHelper.AddCellText(
                    workbookPart,
                    worksheetPart,
                    fieldName,
                    (uint)(i + 1), 1U);
            }
        }
    }

}
