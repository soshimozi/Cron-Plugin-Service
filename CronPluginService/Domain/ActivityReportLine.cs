using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CronPluginService.Domain
{
    public class ActivityReportLine
    {
        public string Company
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }

        public string Product
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public string City
        {
            get;
            set;
        }

        public int Zip
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }

        public string CreateDate
        {
            get;
            set;
        }
    }
}
