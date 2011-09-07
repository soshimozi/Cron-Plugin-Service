using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using System.Diagnostics;
using log4net;
using System.Reflection;

namespace MyReportModule
{
    public class MyReportModuleHandler : IJob
    {
        private static ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region IJob Members

        public void Execute(JobExecutionContext context)
        {
            Log.Debug("Inside MyReportModule.MyReportModuleHandler.Execute");
        }

        #endregion
    }
}
