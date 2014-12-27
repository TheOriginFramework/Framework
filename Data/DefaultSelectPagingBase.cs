using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public abstract class DefaultSelectPagingBase : IQueryPagingInfo
    {
        protected int PageSize { get; set; }
        protected int RowCount { get; set; }
        protected int PageCount { get; set; }
        protected int CurrentPageIndex { get; set; }

        public int GetPageCount()
        {
            return this.PageCount;
        }

        public int GetPageSize()
        {
            return this.PageSize;
        }

        public int GetQueryRowCount()
        {
            return this.RowCount;
        }

        public int GetCurrentPageIndex()
        {
            return this.CurrentPageIndex;
        }
    }
}
