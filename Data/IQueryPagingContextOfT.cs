using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryPagingContext<TModel> : IQueryPagingInfo
    {
        IEnumerable<TModel> FirstPage();
        IEnumerable<TModel> LastPage();
        IEnumerable<TModel> NextPage();
        IEnumerable<TModel> PreviousPage();
        IEnumerable<TModel> ToPage(int PageIndex);
    }
}
