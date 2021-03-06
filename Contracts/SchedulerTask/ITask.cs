﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Contracts.SchedulerTask
{
    public interface ITask : IDisposable
    {
        string GetName();
        void Prepare();
        void Execute();
    }
}
