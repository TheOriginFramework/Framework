using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public enum SqlSelectAggregateMode
    {
        Average,
        Sum,
        Count,
        CountBig,
        Max,
        Min,
        Var,
        VarP,
        StdDev,
        StdDevP
    }
}
