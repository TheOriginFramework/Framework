using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryPagingInfo
    {
        int GetPageCount();
        int GetPageSize();
        int GetQueryRowCount();
        int GetCurrentPageIndex();
    }
}
