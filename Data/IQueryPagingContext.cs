using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryPagingContext : IQueryPagingInfo
    {
        IEnumerable<dynamic> FirstPage();
        IEnumerable<dynamic> LastPage();
        IEnumerable<dynamic> NextPage();
        IEnumerable<dynamic> PreviousPage();
        IEnumerable<dynamic> ToPage(int PageIndex);
    }
}
