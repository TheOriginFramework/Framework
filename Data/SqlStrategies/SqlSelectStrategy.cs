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
    public class SqlSelectStrategy : SqlStrategyBase
    {
        private IModelStrategy _modelStrategy = null;
        private string _columns = null, _filters = null, _orderByColumns = null;
            
        public SqlSelectStrategy(
            Type ModelType,
            IModelStrategy ModelStrategy) : base(ModelType)
        {
            this._modelStrategy = ModelStrategy;
        }

        public SqlSelectStrategy PrepareColumnSelector<TModel>(Expression<Func<TModel, object>> Selector)
        {
            this._columns = this.ParseSelectorToColumns<TModel>(Selector);
            return this;
        }

        public SqlSelectStrategy PrepareFilters<TModel>(IEnumerable<IQueryWhereExpression> WhereExpressions)
        {
            this._filters = this.ParseFilters<TModel>(WhereExpressions);
            return this;
        }

        public SqlSelectStrategy PrepareOrderByColumns(IEnumerable<IQueryOrderByExpression> PagingOrderByExpressions)
        {
            this._orderByColumns = this.ParseOrderByColumns(PagingOrderByExpressions);
            return this;
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

        private string ParseSelectorToColumns<TModel>(Expression<Func<TModel, object>> Selector)
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

        private string ParseFilters<TModel>(IEnumerable<IQueryWhereExpression> WhereExpressions)
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
