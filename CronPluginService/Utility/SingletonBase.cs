using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CronPluginService.Utility
{
    public class SingletonBase<T> where T : new()
    {
        private static T _instance = default(T);

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    }
}
