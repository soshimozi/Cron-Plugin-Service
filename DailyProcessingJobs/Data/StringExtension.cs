using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DailyProcessingJobs.Data
{
    static class StringExtension
    {
        public static T SafeConvert<T>(this string value) where T : IConvertible
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
    }
}
