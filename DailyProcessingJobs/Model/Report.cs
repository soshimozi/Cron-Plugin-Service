using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DailyProcessingJobs.Model
{
    public class Report
    {
        private List<string> _columNames = new List<string>();

        public void AddColumn(string columnName)
        {
            _columNames.Add(columnName);
        }

        public List<string> ColumnNames
        {
            get { return _columNames.ToList(); }
        }

        public Report()
        {
            ReportLines = new List<ReportLine>();
        }

        public ReportLine this[int index]
        {
            get
            {
                return ReportLines[index];
            }
        }
        
        public List<ReportLine> ReportLines
        {
            get;
            private set;
        }
    }
}
