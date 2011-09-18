using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CronPluginService.Framework.Utility;
using System.Data.SqlClient;
using System.Data;
using DailyProcessingJobs.Model;
using log4net;
using System.Reflection;

namespace DailyProcessingJobs.Data
{
    class ReportRepository : SingletonBase<ReportRepository>
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ReportRepository()
        {
        }

        public ActivityReport GetActivityReport(string connectionString)
        {
            SqlDataReader reader = null;

            try
            {
                reader = SqlDataHelper.ExecuteReader(connectionString, CommandType.Text, "SELECT * FROM vDailyActivity");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error in GetActivityReport {0}", ex.ToString());
                return null;
            }

            ActivityReport report = new ActivityReport();
            for (int iField = 0; iField < reader.FieldCount; iField++)
            {
                report.AddColumn(reader.GetName(iField));
            }

            while (reader.Read())
            {
                // create a new activity report line
                ReportLine line = new ReportLine();

                for(int iField=0; iField<reader.FieldCount; iField++)
                {
                    line[reader.GetName(iField)] = reader[iField];
                }

                report.ReportLines.Add(line);
            }

            return report;
        }

    }
}
