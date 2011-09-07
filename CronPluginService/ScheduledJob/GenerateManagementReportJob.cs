using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using System.Reflection;
using log4net;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using CronPluginService.Framework.Data;
using CronPluginService.Framework.Utility;
using System.Data;

namespace CronPluginService.ScheduledJob
{
    public class GenerateManagementReportJob : IJob
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region IJob Members

        public void Execute(JobExecutionContext context)
        {
            string path = string.Format("c:\\dev\\ActivityReport_{0:MMM}_{0:yyyy}.xlsx",DateTime.Now);
            Log.DebugFormat("Creating workbook at {0}", path);

            if (!File.Exists(path))
            {
                // initialize a worksheet
                WorkbookHelper.InitializeWorkbook(path);
            }

            string sheetName = "ManagementReporting_" + DateTime.Now.ToString("MM_dd_yyyy");
            string connectionString =
                     ConfigurationManager.ConnectionStrings["ManagementReporting"].ConnectionString;

            SqlDataReader reader =
                   SqlDataHelper.ExecuteReader(connectionString, CommandType.Text, "SELECT * FROM v_ManagementReporting");

            // if we have a worksheet for this day already, remove it
            if (WorkbookHelper.IsWorksheetPresent(path, sheetName))
            {
                WorkbookHelper.DeleteWorksheet(path, sheetName);
            }

            WorkbookHelper.AddColumnHeadersFromSqlDataReader(path, sheetName, reader);

            uint row = 2;
            while (reader.Read())
            {
                WorkbookHelper.AddRowToWorksheetFromSqlDataReader(path, sheetName, reader, row++);
            }

            Log.DebugFormat("Executing job @ {0}", DateTime.Now);
        }


        #endregion
    }

}
