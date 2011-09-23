using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DailyProcessingJobs.Model;
using DocumentFormat.OpenXml.Packaging;

namespace DailyProcessingJobs
{
    public class ReportGenerator
    {
        public void GenerateReport(
            string workbookPath,
            string worksheetName,
            Report report,
            bool showColumns,
            bool createTable
        )
        {
            // Open the document for editing.
            using (SpreadsheetDocument spreadSheet = WorkbookHelper.OpenOrCreateWorkbook(workbookPath, worksheetName))
            {
                if (WorkbookHelper.IsWorksheetPresent(spreadSheet.WorkbookPart, worksheetName))
                {
                    WorkbookHelper.DeleteWorksheet(spreadSheet.WorkbookPart, worksheetName);
                }

                // Insert a new worksheet.
                WorksheetPart worksheetPart = WorkbookHelper.InsertWorksheet(spreadSheet.WorkbookPart, worksheetName);

                if (showColumns)
                {
                    AddReportColumns(spreadSheet.WorkbookPart, worksheetPart, report.ColumnNames);
                }

                AddReportLines(spreadSheet.WorkbookPart, worksheetPart, report, showColumns ? 1U : 0);

                if (createTable && report.ReportLines.Count > 0)
                {
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

        }

        private void AddReportColumns(WorkbookPart workbookPart, WorksheetPart worksheetPart, List<string> columnNames)
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

        private void AddReportLines(WorkbookPart workbookPart, WorksheetPart worksheetPart, Report report, uint startRow)
        {
            for (int iRow = 0; iRow < report.ReportLines.Count; iRow++)
            {
                for (int iCol = 0; iCol < report.ColumnNames.Count; iCol++)
                {
                    WorkbookHelper.AddCellText(
                        workbookPart,
                        worksheetPart,
                        report[iRow][iCol].ToString(),
                        (uint)iCol + 1, (uint)iRow + startRow + 1);
                }
            }
        }

    }
}
