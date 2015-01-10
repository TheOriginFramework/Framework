using TOF.Framework.Data.Expressions;
using TOF.Framework.Data.SqlStrategies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultSelectPagingContextForDynamic<TModel> : DefaultSelectPagingBase, IQueryPagingContext where TModel: class, new()
    {
        private ITable<TModel> _queryTable = null;
        private Expression<Func<TModel, object>> _selector = null;
        private IEnumerable<IQueryWhereExpression> _filters = null;
        private IEnumerable<IQueryOrderByExpression> _orderByExpressions = null;
        private SqlSelectPagingStrategy<TModel> _selectPaggingStrategy = null;
        private string _lastExecuteStmt = null;

        public DefaultSelectPagingContextForDynamic(
            ITable<TModel> TableForQuery, 
            int PageSize,
            Expression<Func<TModel, object>> Selector = null,
            IEnumerable<IQueryWhereExpression> FilterExpressions = null,
            IEnumerable<IQueryOrderByExpression> OrderByExpressions = null) 
        {
            this._queryTable = TableForQuery;
            this.PageSize = PageSize;
            this.CurrentPageIndex = 0;
            this._filters = FilterExpressions;
            this._selector = Selector;
            this._orderByExpressions = OrderByExpressions;
            this._selectPaggingStrategy = new SqlSelectPagingStrategy<TModel>(
                this._queryTable.GetTableModelStrategy(), this._selector, this._filters, this._orderByExpressions);

            this.CalculatePageParams();
        }

        public IEnumerable<dynamic> FirstPage()
        {
            this.CurrentPageIndex = 0;
            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        public IEnumerable<dynamic> LastPage()
        {
            this.CurrentPageIndex = this.PageCount - 1;
            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        public IEnumerable<dynamic> NextPage()
        {
            if (this.PageCount > this.CurrentPageIndex + 1)
                this.CurrentPageIndex++;

            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        public IEnumerable<dynamic> PreviousPage()
        {
            if (this.CurrentPageIndex > 0)
                this.CurrentPageIndex--;

            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        public IEnumerable<dynamic> ToPage(int PageIndex)
        {
            if (PageIndex < 0 || PageIndex > this.PageCount)
                throw new ArgumentOutOfRangeException("ERROR_PAGE_INDEX_OUT_OF_RANGE");

            this.CurrentPageIndex = PageIndex;

            var positions = this.CalculateRowIndexRangeByPageIndex(this.CurrentPageIndex);
            return this.Select(positions.Item1, positions.Item2);
        }

        private IEnumerable<dynamic> Select(int StartIndex, int EndIndex)
        {
            (this._selectPaggingStrategy as IQueryPagingConfiguration).RenewPagingRowPositions(StartIndex, EndIndex);

            string sql = this._selectPaggingStrategy.RenderQuery();
            var parameters = this._selectPaggingStrategy.RenderParameters();
            var properties = typeof(TModel).GetProperties();
            this._lastExecuteStmt = sql;

            ISqlExecutionProvider executionProvider = Utils.GetDbExecutionProvider();
            ISqlExecutionTransactionProvider transactionProvider = executionProvider as ISqlExecutionTransactionProvider;
            SqlQueryNewTypeRenderColumnNode selectorParser = new Expressions.SqlQueryNewTypeRenderColumnNode(this._queryTable.GetTableModelStrategy());
            IDictionary<string, string> columns = selectorParser.Parse(this._selector.Body);

            executionProvider.Open();
            var records = executionProvider.ExecuteQuery(sql, parameters);

            List<dynamic> items = new List<dynamic>();

            foreach (var record in records)
            {
                dynamic item = new ExpandoObject();
                IDictionary<string, object> itemProps = item as IDictionary<string, object>;

                foreach (var column in columns)
                {
                    try
                    {
                        if (record.GetOrdinal(column.Value.Trim()) >= 0)
                            ((IDictionary<string, object>)item).Add(column.Key.Trim(), record.GetValue(record.GetOrdinal(column.Value.Trim())));
                        else
                            ((IDictionary<string, object>)item).Add(column.Key.Trim(), DBNull.Value);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        ((IDictionary<string, object>)item).Add(column.Key.Trim(), DBNull.Value);
                    }
                }

                items.Add(item);
            }

            executionProvider.Close();

            return items;
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
                return new Tuple<int, int>(0, this.PageSize);
            else
            {
                int startIndex = (this.PageSize * PageIndex) + 1;
                int endIndex = (this.PageSize * PageIndex) + this.PageSize;

                if (endIndex > this.RowCount)
                    endIndex = this.RowCount;

                return new Tuple<int, int>(startIndex, endIndex);
            }
        }
    }
}
