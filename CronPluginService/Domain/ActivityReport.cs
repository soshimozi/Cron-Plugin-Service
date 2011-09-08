using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CronPluginService.Domain
{
    public class ActivityReport
    {
        public ActivityReport()
        {
            ReportLines = new List<ActivityReportLine>();
        }

        public List<ActivityReportLine> ReportLines
        {
            get;
            private set;
        }
    }
}
