using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryPagingConfiguration
    {
        void RenewPagingRowPositions(int StartPosIndex, int EndPosIndex);
    }
}
