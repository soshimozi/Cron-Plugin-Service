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
using System.Configuration;

namespace DailyProcessingJobs.Data
{
    class ReportRepository : SingletonBase<ReportRepository>
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        public Report GetReportData(string CommandText)
        {
            return ReportFromQuery(CommandType.Text, CommandText);
        }

        protected Report ReportFromQuery(CommandType commandType, string commandText)
        {
            SqlDataReader reader = null;

            Report report = new Report();
            try
            {
                reader = SqlDataHelper.ExecuteReader(_connectionString, commandType, commandText);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error in ReportFromQuery {0}", ex.ToString());
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
                    ReportLine line = new ReportLine();

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
