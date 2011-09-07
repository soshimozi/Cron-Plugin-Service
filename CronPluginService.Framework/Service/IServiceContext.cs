using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CronPluginService.Framework.Service
{
    public interface IServiceContext
    {
        void Start(string [] args);
        void Stop();
        void Pause();
        void Continue();
    }
}
