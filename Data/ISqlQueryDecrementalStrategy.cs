using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface ISqlQueryDecrementalStrategy
    {
        bool CanSupportDecrementalOperation();
        string RenderDecreaseQuery();
        IEnumerable<IDbDataParameter> RenderDecreaseParameters();
    }
}
