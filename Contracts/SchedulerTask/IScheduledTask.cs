using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Contracts.SchedulerTask
{
    public interface IScheduledTask : IPeriodicTask
    {
        DateTime GetTaskBeginDate();
        DateTime GetTaskEndDate();
    }
}
