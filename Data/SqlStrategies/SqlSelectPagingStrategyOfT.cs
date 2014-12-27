using TOF.Framework.Data.Exceptions;
using TOF.Framework.Data.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.SqlStrategies
{
    public class SqlSelectPagingStrategy<TModel> : SqlStrategyBase<TModel>, IQueryPagingConfiguration where TModel : class, new()
    {
        private IModelStrategy _modelStrategy = null;
        private string _columns = null, _filters = null, _orderByColumns = null;
        private int _startIndex = 0, _endIndex = 0;
        private Dictionary<string, object> _selectQueryParams = new Dictionary<string, object>();

        public SqlSelectPagingStrategy(
            IModelStrategy ModelStrategy,
            Expression<Func<TModel, object>> Selector = null,
            IEnumerable<IQueryWhereExpression> FilterExpressions = null,
            IEnumerable<IQueryOrderByExpression> PagingOrderByExpressions = null)
            : base(ModelStrategy)
        {
            this._modelStrategy = ModelStrategy;

            if (Selector != null)
                this._columns = this.ParseSelectorToColumns(Selector);
            if (FilterExpressions != null)
                this._filters = this.ParseFilters(FilterExpressions);
            if (PagingOrderByExpressions.Any())
                this._orderByColumns = this.ParseOrderByColumns(PagingOrderByExpressions);
        }
        
        public override IEnumerable<IDbDataParameter> RenderParameters()
        {
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();

            if (this._filters != null)
                parameters = new List<IDbDataParameter>(base.RenderParameters());

            parameters.Add(new SqlParameter("@p__s__index", this._startIndex));
            parameters.Add(new SqlParameter("@p__e__index", this._endIndex));

            return parameters;
        }

        public override string RenderQuery()
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append("SELECT * FROM (");
            sqlBuilder.Append("SELECT ");

            if (string.IsNullOrEmpty(this._columns))
                sqlBuilder.Append("*");
            else
                sqlBuilder.Append(this._columns);

            sqlBuilder.Append(", ROW_NUMBER() OVER (ORDER BY ");

            // add ROW_NUMBER
            if (string.IsNullOrEmpty(this._orderByColumns))
            {
                var keyProperties = this._modelStrategy.GetModelPropertyBindings().Where(c => c.IsKey());
                var keyColumnBuilder = new StringBuilder();

                if (!keyProperties.Any())
                    throw new InvalidOperationException("ERROR_ENTITY_KEY_NOT_FOUND");
                else
                {
                    foreach (var keyProperty in keyProperties)
                    {
                        if (keyColumnBuilder.Length == 0)
                            keyColumnBuilder.Append(keyProperty.GetParameterName());
                        else
                            keyColumnBuilder.Append(", ").Append(keyProperty.GetParameterName());
                    }

                    sqlBuilder.Append(keyColumnBuilder.ToString());
                }
            }
            else
            {
                sqlBuilder.Append(this._orderByColumns);
            }

            sqlBuilder.Append(")  AS RowPagingIndex");

            sqlBuilder.Append(" ");
            sqlBuilder.Append("FROM ");
            sqlBuilder.Append(this._modelStrategy.GetTableName());
            sqlBuilder.Append(" ");
            sqlBuilder.Append(this._modelStrategy.GetTableAlias());
            sqlBuilder.Append(" ");

            if (!string.IsNullOrEmpty(this._filters))
            {
                sqlBuilder.Append("WHERE ");
                sqlBuilder.Append(this._filters);
            }

            // not necessary
            //if (!string.IsNullOrEmpty(this._orderByColumns))
            //{
            //    sqlBuilder.Append("ORDER BY ");
            //    sqlBuilder.Append(this._orderByColumns);
            //}

            sqlBuilder.Append(") AS pagingQuery WHERE pagingQuery.RowPagingIndex >= @p__s__index AND pagingQuery.RowPagingIndex <= @p__e__index");
            return sqlBuilder.ToString();
        }

        public void RenewPagingRowPositions(int StartPosIndex, int EndPosIndex)
        {
            this._startIndex = StartPosIndex;
            this._endIndex = EndPosIndex;
        }

        private string ParseSelectorToColumns(Expression<Func<TModel, object>> Selector)
        {
            // check expression type is "New Expression" or "Parameter Expression"
            if (Selector.Body is NewExpression)
            {
                var newExpNode = new SqlQueryNewExpressionRenderColumnsNode();
                newExpNode.ModelStrategy = this._modelStrategy;
                return newExpNode.Parse(Selector.Body);
            }
            else if (Selector.Body is ParameterExpression)
            {
                var paramNode = new SqlQueryParameterExpressionRenderColumnsNode();
                paramNode.ModelStrategy = this._modelStrategy;
                return paramNode.Parse(Selector.Body);
            }
            else
                throw new DbEnvironmentException(
                    "ERROR_EXPRESSION_NOT_SUPPORTED");
        }

        private string ParseFilters(IEnumerable<IQueryWhereExpression> WhereExpressions)
        {
            var whereConditionBuilder = new StringBuilder();

            foreach (var whereExpression in WhereExpressions)
            {
                if (whereConditionBuilder.Length == 0)
                    whereConditionBuilder.Append(whereExpression.GetWhereExpression());
                else
                    whereConditionBuilder.Append(" ").Append(whereExpression.GetWhereExpression());
            }

            return whereConditionBuilder.ToString();
        }

        private string ParseOrderByColumns(IEnumerable<IQueryOrderByExpression> PagingOrderByExpressions)
        {
            var orderbyBuilder = new StringBuilder();

            foreach (var orderByExpression in PagingOrderByExpressions)
            {
                if (orderbyBuilder.Length == 0)
                    orderbyBuilder.Append(orderByExpression.GetOrderByExpression());
                else
                    orderbyBuilder.Append(" ").Append(orderByExpression.GetOrderByExpression());
            }

            return orderbyBuilder.ToString();
        }
    }
}
