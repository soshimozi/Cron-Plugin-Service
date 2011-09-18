using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DailyProcessingJobs.Model
{
    public class ReportLine
    {
        List<string> Columns
        {
            get
            {
                return _values.Keys.ToList();
            }
        }

        private Dictionary<string, object> _values = new Dictionary<string,object>();

        public object this[int index]
        {
            get
            {
                if (Columns.Count < index)
                {
                    throw new ArgumentOutOfRangeException("Index must be less than the size of the collection.");
                }
                else
                {
                    return _values[Columns[index]];
                }
            }

            set
            {
                if (Columns.Count < index)
                {
                    throw new ArgumentOutOfRangeException("Index must be less than the size of the collection.");
                }
                else
                {
                    _values[Columns[index]] = value;
                }
            }
        }

        public object this[string key]
        {
            get
            {
                if (_values.ContainsKey(key))
                {
                    return _values[key];
                }
                else return null;
            }

            set
            {
                if (_values.ContainsKey(key))
                {
                    _values[key] = value;
                }
                else
                {
                    _values.Add(key, value);
                }
            }
        }

        //public string Company
        //{
        //    get;
        //    set;
        //}

        //public string OrderID
        //{
        //    get;
        //    set;
        //}

        //public string LoanID
        //{
        //    get;
        //    set;
        //}

        //public string FirstName
        //{
        //    get;
        //    set;
        //}

        //public string LastName
        //{
        //    get;
        //    set;
        //}

        //public string Status
        //{
        //    get;
        //    set;
        //}

        //public string Product
        //{
        //    get;
        //    set;
        //}

        //public string Street
        //{
        //    get;
        //    set;
        //}

        //public string City
        //{
        //    get;
        //    set;
        //}

        //public string State
        //{
        //    get;
        //    set;
        //}

        //public int Zip
        //{
        //    get;
        //    set;
        //}

        //public string CreatedBy
        //{
        //    get;
        //    set;
        //}

        //public string OrderDate
        //{
        //    get;
        //    set;
        //}
    }
}
