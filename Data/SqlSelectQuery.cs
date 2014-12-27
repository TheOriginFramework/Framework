using TOF.Framework.Data;
using TOF.Framework.Data.Exceptions;
using TOF.Framework.Data.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public class SqlSelectQuery<TModel> : SqlStrategyBase<TModel>
        where TModel: class, new()
    {
        private ITable<TModel> _table = null;
        private string _columns = null;
        private string _filters = null;
        private string _joins = null;

        private SqlSelectQuery()
        {
            // default constructor.
        }

        private SqlSelectQuery(ITable<TModel> Table)
        {
            this._table = Table;
        }

        public static SqlSelectQuery<TModel> ToTable(ITable<TModel> Table)
        {
            return new SqlSelectQuery<TModel>(Table);
        }
        
        public SqlSelectQuery<TModel> Select() // query all fields.
        {
            var selectExpression = Expression.Parameter(typeof(TModel), this._table.GetAlias());
            var paramNode = new SqlQueryParameterExpressionRenderColumnsNode();
            this._columns = paramNode.Parse(selectExpression);
            return this;
        }

        public SqlSelectQuery<TModel> Select<TTable>(Expression<Func<TTable, object>> SelectColumnExpression)  // query specified fields
        {
            // check expression type is "New Expression" or "Parameter Expression"
            if (SelectColumnExpression.Body is NewExpression)
            {
                var newExpNode = new SqlQueryNewExpressionRenderColumnsNode();
                this._columns = newExpNode.Parse(SelectColumnExpression.Body);
            }
            else if (SelectColumnExpression.Body is ParameterExpression)
            {
                var paramNode = new SqlQueryParameterExpressionRenderColumnsNode();
                this._columns = paramNode.Parse(SelectColumnExpression.Body);
            }

            return this;
        }

        public SqlSelectQuery<TModel> Select<TTable, TTable2>(
            Expression<Func<TTable, TTable2, object>> SelectColumnExpression)  // query specified fields
        {
            // check expression type is "New Expression" or "Parameter Expression"
            if (SelectColumnExpression.Body is NewExpression)
            {
                var newExpNode = new SqlQueryNewExpressionRenderColumnsNode();
                this._columns = newExpNode.Parse(SelectColumnExpression.Body);
            }
            else if (SelectColumnExpression.Body is ParameterExpression)
            {
                var paramNode = new SqlQueryParameterExpressionRenderColumnsNode();
                this._columns = paramNode.Parse(SelectColumnExpression.Body);
            }

            return this;
        }

        public SqlSelectQuery<TModel> Select<TTable, TTable2, TTable3>(
            Expression<Func<TTable, TTable2, TTable3, object>> SelectColumnExpression)  // query specified fields
        {
            // check expression type is "New Expression" or "Parameter Expression"
            if (SelectColumnExpression.Body is NewExpression)
            {
                var newExpNode = new SqlQueryNewExpressionRenderColumnsNode();
                this._columns = newExpNode.Parse(SelectColumnExpression.Body);
            }
            else if (SelectColumnExpression.Body is ParameterExpression)
            {
                var paramNode = new SqlQueryParameterExpressionRenderColumnsNode();
                this._columns = paramNode.Parse(SelectColumnExpression.Body);
            }

            return this;
        }

        public SqlSelectQuery<TModel> Select<TTable, TTable2, TTable3, TTable4>(
            Expression<Func<TTable, TTable2, TTable3, TTable4, object>> SelectColumnExpression)  // query specified fields
        {
            // check expression type is "New Expression" or "Parameter Expression"
            if (SelectColumnExpression.Body is NewExpression)
            {
                var newExpNode = new SqlQueryNewExpressionRenderColumnsNode();
                this._columns = newExpNode.Parse(SelectColumnExpression.Body);
            }
            else if (SelectColumnExpression.Body is ParameterExpression)
            {
                var paramNode = new SqlQueryParameterExpressionRenderColumnsNode();
                this._columns = paramNode.Parse(SelectColumnExpression.Body);
            }

            return this;
        }

        public SqlSelectQuery<TModel> Where<TTable>(Expression<Func<TTable, bool>> FilterExpressions)
        {
            IQueryExpressionParser sqlQueryParser = new SqlQueryExpressionParser();
            var expressionBody = FilterExpressions.Body as BinaryExpression;
            this._filters = sqlQueryParser.Parse(expressionBody, this.ModelStrategy);

            return this;
        }

        public SqlSelectQuery<TModel> Where<TTable, TTable2>(Expression<Func<TTable, TTable2, bool>> FilterExpressions)
        {
            IQueryExpressionParser sqlQueryParser = new SqlQueryExpressionParser();
            var expressionBody = FilterExpressions.Body as BinaryExpression;
            this._filters = sqlQueryParser.Parse(expressionBody, this.ModelStrategy);

            return this;
        }

        public SqlSelectQuery<TModel> Where<TTable, TTable2, TTable3>(Expression<Func<TTable, TTable2, TTable3, bool>> FilterExpressions)
        {
            IQueryExpressionParser sqlQueryParser = new SqlQueryExpressionParser();
            var expressionBody = FilterExpressions.Body as BinaryExpression;
            this._filters = sqlQueryParser.Parse(expressionBody, this.ModelStrategy);

            return this;
        }

        public SqlSelectQuery<TModel> Where<TTable, TTable2, TTable3, TTable4>(
            Expression<Func<TTable, TTable2, TTable3, TTable4, bool>> FilterExpressions)
        {
            IQueryExpressionParser sqlQueryParser = new SqlQueryExpressionParser();
            var expressionBody = FilterExpressions.Body as BinaryExpression;
            this._filters = sqlQueryParser.Parse(expressionBody, this.ModelStrategy);

            return this;
        }

        public SqlSelectQuery<TModel> Join<TJoinModel>(
            ITable<TJoinModel> TableNameForJoin, TableJoinDirection JoinDirection, Expression<Func<ITable<TModel>, ITable<TJoinModel>, bool>> FilterExpressions)
            where TJoinModel: class, new()
        {
            string joinPattern = "{JOIN_INDICATOR} {TABLE_JOIN} ON {JOIN_FILTER}";

            switch (JoinDirection)
            {
                case TableJoinDirection.InnerJoin:
                    joinPattern = joinPattern.Replace("{JOIN_INDICATOR}", "INNER JOIN");
                    break;
                case TableJoinDirection.LeftJoin:
                    joinPattern = joinPattern.Replace("{JOIN_INDICATOR}", "LEFT OUTER JOIN");
                    break;
                case TableJoinDirection.RightJoin:
                    joinPattern = joinPattern.Replace("{JOIN_INDICATOR}", "RIGHT OUTER JOIN");
                    break;
                case TableJoinDirection.FullJoin:
                    joinPattern = joinPattern.Replace("{JOIN_INDICATOR}", "FULL OUTER JOIN");
                    break;
            }

            var sqlQueryParser = new SqlQueryExpressionParser();
            var expressionBody = FilterExpressions.Body as BinaryExpression;
            joinPattern = joinPattern.Replace(
                "{TABLE_JOIN}", string.Format("{0} {1}", TableNameForJoin.GetName(), TableNameForJoin.GetAlias()));
            joinPattern = joinPattern.Replace(
                "{JOIN_FILTER}", sqlQueryParser.Parse(expressionBody, this.ModelStrategy));

            this._joins = joinPattern;

            return this;
        }

        public override string RenderQuery()
        {
            string sql = "SELECT {COLUMNS} FROM {TABLE} {TABLE_JOIN} {FILTER_INDICATOR} {FILTER}";
            
            if (!string.IsNullOrEmpty(this._filters))
                sql = sql.Replace("{FILTER_INDICATOR}", "WHERE");
            else
                sql = sql.Replace("{FILTER_INDICATOR}", "").Replace("{FILTER}", "");

            sql = sql.Replace("{COLUMNS}", this._columns);
            sql = sql.Replace("{TABLE}",
                string.Format("{0} {1}", this._table.GetName(), this._table.GetAlias()));

            if (!string.IsNullOrEmpty(this._joins))
                sql = sql.Replace("{TABLE_JOIN}", this._joins);
            else
                sql = sql.Replace("{TABLE_JOIN}", "");

            return sql.Trim();
        }
    }
}
