using System;
using CronPluginService.Framework.Utility;
using System.Data.SqlClient;
using System.Data;
using DailyProcessingJobs.Model;
using log4net;
using System.Reflection;
using System.Configuration;

namespace DailyProcessingJobs.Data
{
    class ReportRepository : SingletonBase<ReportRepository>
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _connectionString;
        public ReportRepository()
        {
            _connectionString =
                     ConfigurationManager.ConnectionStrings["Reporting"].ConnectionString;
        }

        public Report GetDailyActivityReport()
        {
            return ReportFromQuery(
                CommandType.Text, 
                "SELECT * FROM vDailyActivity"
            );
        }

        public Report GetReportData(string commandText)
        {
            return ReportFromQuery(CommandType.Text, commandText);
        }

        protected Report ReportFromQuery(CommandType commandType, string commandText)
        {
            SqlDataReader reader;

            var report = new Report();
            try
            {
                reader = SqlDataHelper.ExecuteReader(_connectionString, commandType, commandText);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error in ReportFromQuery {0}", ex);
                reader = null;
            }

            if (reader != null)
            {
                for (int iField = 0; iField < reader.FieldCount; iField++)
                {
                    report.AddColumn(reader.GetName(iField));
                }

                while (reader.Read())
                {
                    // create a new activity report line
                    var line = new ReportLine();

                    for (int iField = 0; iField < reader.FieldCount; iField++)
                    {
                        line[reader.GetName(iField)] = reader[iField];
                    }

                    report.ReportLines.Add(line);
                }
            }

            return report;
        }


        public Report GetAcvityTotalsReport()
        {
            return ReportFromQuery(
                CommandType.StoredProcedure,
                "spr_ReportActivitySummary"
            );
        }
    }
}
