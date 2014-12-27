using TOF.Framework.Data;
using TOF.Framework.Data.Exceptions;
using TOF.Framework.Data.Expressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.SqlStrategies
{
    public class SqlSelectStrategy<TModel> : SqlStrategyBase<TModel>
        where TModel : class, new()
    {
        private IModelStrategy _modelStrategy = null;
        private string _columns = null, _filters = null, _orderByColumns = null;
        private Dictionary<string, object> _selectQueryParams = new Dictionary<string, object>();
            
        public SqlSelectStrategy(
            IModelStrategy ModelStrategy,
            Expression<Func<TModel, object>> Selector = null,
            IEnumerable<IQueryWhereExpression> FilterExpressions = null,
            IEnumerable<IQueryOrderByExpression> OrderByExpressions = null) : base()
        {
            this._modelStrategy = ModelStrategy;

            if (Selector != null)
                this._columns = this.ParseSelectorToColumns(Selector);
            if (FilterExpressions != null)
                this._filters = this.ParseFilters(FilterExpressions);
            if (OrderByExpressions.Any())
                this._orderByColumns = this.ParseOrderByColumns(OrderByExpressions);
        }
        
        public override string RenderQuery()
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append("SELECT ");

            if (string.IsNullOrEmpty(this._columns))
                sqlBuilder.Append("*");
            else
                sqlBuilder.Append(this._columns);

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

            if (!string.IsNullOrEmpty(this._orderByColumns))
            {
                sqlBuilder.Append("ORDER BY ");
                sqlBuilder.Append(this._orderByColumns);
            }

            return sqlBuilder.ToString();
        }

        public override IEnumerable<IDbDataParameter> RenderParameters()
        {
            return base.RenderParameters();
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
                throw new DbEnvironmentException("ERROR_EXPRESSION_NOT_SUPPORTED");
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

        private string ParseOrderByColumns(IEnumerable<IQueryOrderByExpression> OrderByExpressions)
        {
            var orderbyBuilder = new StringBuilder();

            foreach (var orderByExpression in OrderByExpressions)
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
