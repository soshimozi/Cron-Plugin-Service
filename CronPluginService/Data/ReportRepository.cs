using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CronPluginService.Framework.Utility;
using CronPluginService.Domain;
using System.Data.SqlClient;
using System.Data;
using CronPluginService.Data;

namespace CronPluginService.Data
{
    class ReportRepository : SingletonBase<ReportRepository>
    {
        private readonly string _connectionString;
        public ReportRepository()
        {
            _connectionString = "";
        }

        public ActivityReport GetActivityReport()
        {
            SqlDataReader reader = SqlDataHelper.ExecuteReader(_connectionString, CommandType.Text, "SELECT * FROM vActivityReport");

            ActivityReport report = new ActivityReport();
            while (reader.Read())
            {
                // create a new activity report line
                ActivityReportLine line = new ActivityReportLine()
                {
                    Company = reader["Company"].ToString(),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Address = reader["Address"].ToString(),
                    Product = reader["Product"].ToString(),
                    Status = reader["Status"].ToString(),
                    City = reader["City"].ToString(),
                    CreateDate = reader["CreateDate"].ToString(),
                    CreatedBy = reader["CreatedBy"].ToString(),
                    Zip = reader["Zip"].ToString().SafeConvert<int>()
                };

                report.ReportLines.Add(line);
            }

            return report;
        }

    }
}
