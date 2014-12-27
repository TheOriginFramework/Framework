using TOF.Framework.Data.SqlStrategies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultSelectPagingContext<TModel> : DefaultSelectPagingBase, IQueryPagingContext<TModel> where TModel: class, new()
    {
        private ITable<TModel> _queryTable = null;
        private IEnumerable<IQueryWhereExpression> _filters = null;
        private IEnumerable<IQueryOrderByExpression> _orderByExpressions = null;
        private SqlSelectPagingStrategy<TModel> _selectPaggingStrategy = null;
        private string _lastExecuteStmt = null;

        public DefaultSelectPagingContext(
            ITable<TModel> TableForQuery, 
            int PageSize,
            IEnumerable<IQueryWhereExpression> FilterExpressions = null,
            IEnumerable<IQueryOrderByExpression> OrderByExpressions = null)
        {
            this._queryTable = TableForQuery;
            this.PageSize = PageSize;
            this.CurrentPageIndex = 0;
            this._filters = FilterExpressions;
            this._orderByExpressions = OrderByExpressions;
            this._selectPaggingStrategy = new SqlSelectPagingStrategy<TModel>(
                this._queryTable.GetTableModelStrategy(), null, this._filters, this._orderByExpressions);

            this.CalculatePageParams();
        }

        public IEnumerable<TModel> FirstPage()
        {
            this.CurrentPageIndex = 0;
            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        public IEnumerable<TModel> LastPage()
        {
            this.CurrentPageIndex = this.PageCount - 1;
            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        public IEnumerable<TModel> NextPage()
        {
            if (this.PageCount > this.CurrentPageIndex + 1)
                this.CurrentPageIndex++;

            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        public IEnumerable<TModel> PreviousPage()
        {
            if (this.CurrentPageIndex > 0)
                this.CurrentPageIndex--;

            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        public IEnumerable<TModel> ToPage(int PageIndex)
        {
            if (PageIndex < 0 || PageIndex > this.PageCount)
                throw new ArgumentOutOfRangeException("ERROR_PAGE_INDEX_OUT_OF_RANGE");

            this.CurrentPageIndex = PageIndex;

            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        private void CalculatePageParams()
        {
            this.RowCount = this._queryTable.Count();

            if (this.RowCount > 0)
            {
                if (this.RowCount % this.PageSize > 0)
                    this.PageCount = (this.RowCount / this.PageSize) + 1;
                else
                    this.PageCount = this.RowCount / this.PageSize;
            }
        }

        private Tuple<int, int> CalculateRowIndexRangeByPageIndex(int PageIndex)
        {
            if (PageIndex < 0 || PageIndex >= this.PageCount)
                throw new ArgumentOutOfRangeException("ERROR_PAGE_INDEX_OUT_OF_RANGE");
            if (PageIndex == 0)
                return new Tuple<int, int>(1, this.PageSize);
            else
            {
                int startIndex = (this.PageSize * PageIndex) + 1;
                int endIndex = (this.PageSize * PageIndex) + this.PageSize;

                if (endIndex > this.RowCount)
                    endIndex = this.RowCount;

                return new Tuple<int, int>(startIndex, endIndex);
            }
        }

        private IEnumerable<TModel> Select(int StartIndex, int EndIndex)
        {
            (this._selectPaggingStrategy as IQueryPagingConfiguration).RenewPagingRowPositions(StartIndex, EndIndex);

            string sql = this._selectPaggingStrategy.RenderQuery();
            var parameters = this._selectPaggingStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();
            var propertyBindings = this._queryTable.GetTableModelStrategy().GetModelPropertyBindings();

            this._lastExecuteStmt = sql;

            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider();
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;

            executionProvider.Open();
            IDataReader reader = executionProvider.ExecuteGetReader(sql, parameters);

            List<TModel> items = new List<TModel>();
            var records = Utils.GetDataRecords(reader);

            foreach (var record in records)
                items.Add(Utils.BindingDataRecordToModel<TModel>(record, this._queryTable.GetTableModelStrategy().GetModelPropertyBindings()));

            reader.Close();
            executionProvider.Close();

            return items;
        }
    }
}
