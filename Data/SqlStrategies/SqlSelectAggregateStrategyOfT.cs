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
    public class SqlSelectAggregateStrategy<TModel> : SqlStrategyBase<TModel> where TModel: class, new()
    {
        private SqlSelectAggregateMode _aggregMode = SqlSelectAggregateMode.Count;
        private IModelStrategy _modelStrategy = null;
        private string _columns = null, _filters = null, _groupBySelectors = null;

        public SqlSelectAggregateStrategy(
            SqlSelectAggregateMode AggregateMode,
            IModelStrategy ModelStrategy,
            Expression<Func<TModel, object>> Selector = null,
            IEnumerable<IQueryWhereExpression> FilterExpressions = null,
            Expression<Func<TModel, object>>[] GroupBySelector = null) : base()
        {
            this._modelStrategy = ModelStrategy;
            this._aggregMode = AggregateMode;
            
            if (Selector != null)
                this._columns = this.ParseSelectorToColumns(Selector);
            if (FilterExpressions != null)
                this._filters = this.ParseFilters(FilterExpressions);
            if (GroupBySelector != null)
                this._groupBySelectors = this.ParseGroupByColumns(GroupBySelector);
        }

        public override IEnumerable<IDbDataParameter> RenderParameters()
        {
            return base.RenderParameters();
        }

        public override string RenderQuery()
        {
            switch (this._aggregMode)
            {
                case SqlSelectAggregateMode.Count:
                    return ParseCountQuery(IsBigCount: false);
                case SqlSelectAggregateMode.CountBig:
                    return ParseCountQuery(IsBigCount: true);
                case SqlSelectAggregateMode.Average:
                    return ParseAverageQuery();
                case SqlSelectAggregateMode.Sum:
                    return ParseSumQuery();
                case SqlSelectAggregateMode.Max:
                    return ParseMaxQuery();
                case SqlSelectAggregateMode.Min:
                    return ParseMinQuery();
                case SqlSelectAggregateMode.Var:
                    return ParseVarQuery(IsForPopulation: false);
                case SqlSelectAggregateMode.VarP:
                    return ParseVarQuery(IsForPopulation: true);
                case SqlSelectAggregateMode.StdDev:
                    return ParseStdDevQuery(IsForPopulation: false);
                case SqlSelectAggregateMode.StdDevP:
                    return ParseStdDevQuery(IsForPopulation: true);
                default:
                    throw new NotSupportedException("ERROR_UNKNOWN_AGGREGATE_MODE");
            }
        }

        private string ParseCountQuery(bool IsBigCount = false)
        {
            var sqlBuilder = new StringBuilder();
            var aggOp = (IsBigCount) ? "COUNT_BIG" : "COUNT";
            
            if (!string.IsNullOrEmpty(this._groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {1}, ISNULL({0}({2}), 0) AS {1}", aggOp, this._groupBySelectors, this._columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL({0}(*), 0)", aggOp));

            sqlBuilder.Append(this.GenerateSelectBodyStmt());

            return sqlBuilder.ToString();
        }

        private string ParseAverageQuery()
        {
            var sqlBuilder = new StringBuilder();

            if (this._columns.Split(',').Length > 1)
                throw new InvalidOperationException("ERROR_AVERAGE_SUPPORTS_ONE_COLUMN_ONLY");

            if (!string.IsNullOrEmpty(this._groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {0}, ISNULL(AVG({1}), 0.0) AS {1}", this._groupBySelectors, this._columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL(AVG({0}), 0.0) As {0}", this._columns));

            sqlBuilder.Append(this.GenerateSelectBodyStmt());

            return sqlBuilder.ToString();
        }

        private string ParseSumQuery()
        {
            var sqlBuilder = new StringBuilder();

            if (this._columns.Split(',').Length > 1)
                throw new InvalidOperationException("ERROR_AVERAGE_SUPPORTS_ONE_COLUMN_ONLY");

            if (!string.IsNullOrEmpty(this._groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {0}, ISNULL(SUM({1}), 0.0) AS {1}", this._groupBySelectors, this._columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL(SUM({0}), 0.0) AS {0}", this._columns));

            sqlBuilder.Append(this.GenerateSelectBodyStmt());

            return sqlBuilder.ToString();
        }

        private string ParseMaxQuery()
        {
            var sqlBuilder = new StringBuilder();

            if (this._columns.Split(',').Length > 1)
                throw new InvalidOperationException("ERROR_MAX_SUPPORTS_ONE_COLUMN_ONLY");

            if (!string.IsNullOrEmpty(this._groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {0}, ISNULL(MAX({1}), 0.0) AS {1}", this._groupBySelectors, this._columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL(MAX({0}), 0.0) AS {0}", this._columns));

            sqlBuilder.Append(this.GenerateSelectBodyStmt());

            return sqlBuilder.ToString();
        }


        private string ParseMinQuery()
        {
            var sqlBuilder = new StringBuilder();

            if (this._columns.Split(',').Length > 1)
                throw new InvalidOperationException("ERROR_MIN_SUPPORTS_ONE_COLUMN_ONLY");

            if (!string.IsNullOrEmpty(this._groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {0}, ISNULL(MIN({1}), 0.0) AS {1}", this._groupBySelectors, this._columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL(MIN({0}), 0.0) AS {0}", this._columns));

            sqlBuilder.Append(this.GenerateSelectBodyStmt());

            return sqlBuilder.ToString();
        }

        private string ParseVarQuery(bool IsForPopulation = false)
        {
            var sqlBuilder = new StringBuilder();
            var aggOp = (IsForPopulation) ? "VARP" : "VAR";

            if (!string.IsNullOrEmpty(this._groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {1}, ISNULL({0}({2}), 0) AS {1}", aggOp, this._groupBySelectors, this._columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL({0}(*), 0)", aggOp));

            sqlBuilder.Append(this.GenerateSelectBodyStmt());

            return sqlBuilder.ToString();
        }

        private string ParseStdDevQuery(bool IsForPopulation = false)
        {
            var sqlBuilder = new StringBuilder();
            var aggOp = (IsForPopulation) ? "STDEVP" : "STDEV";

            if (!string.IsNullOrEmpty(this._groupBySelectors))
                sqlBuilder.Append(string.Format("SELECT {1}, ISNULL({0}({2}), 0) AS {1}", aggOp, this._groupBySelectors, this._columns));
            else
                sqlBuilder.Append(string.Format("SELECT ISNULL({0}(*), 0)", aggOp));

            sqlBuilder.Append(this.GenerateSelectBodyStmt());

            return sqlBuilder.ToString();
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
            else if (Selector.Body is UnaryExpression)
            {
                var paramNode = SqlQueryExpressionFactory.GetNodeParser(Selector.Body, this._modelStrategy);
                return paramNode.Parse(Selector.Body);
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

        private string ParseGroupByColumns(Expression<Func<TModel, object>>[] GroupBySelectors)
        {
            if (GroupBySelectors == null || GroupBySelectors.Length == 0)
                return string.Empty;

            var groupByColumnsBuilder = new StringBuilder();

            foreach (var GroupBySelector in GroupBySelectors)
            {
                // check expression type is "New Expression" or "Parameter Expression"
                if (GroupBySelector.Body is NewExpression)
                    throw new NotSupportedException("ERROR_GROUP_BY_NOT_SUPPORT_NEW_EXPRESSION");
                else if (GroupBySelector.Body is UnaryExpression)
                {
                    var paramNode = SqlQueryExpressionFactory.GetNodeParser(GroupBySelector.Body, this._modelStrategy);
                    var name = paramNode.Parse(GroupBySelector.Body);

                    if (groupByColumnsBuilder.Length > 0)
                        groupByColumnsBuilder.Append(", " + name);
                    else
                        groupByColumnsBuilder.Append(name);
                }
                else if (GroupBySelector.Body is ParameterExpression)
                {
                    var paramNode = new SqlQueryParameterExpressionRenderColumnsNode();
                    paramNode.ModelStrategy = this._modelStrategy;
                    var name = paramNode.Parse(GroupBySelector.Body);

                    if (groupByColumnsBuilder.Length > 0)
                        groupByColumnsBuilder.Append(", " + name);
                    else
                        groupByColumnsBuilder.Append(name);
                }
                else
                    throw new DbEnvironmentException(
                        "ERROR_EXPRESSION_NOT_SUPPORTED");
            }

            return groupByColumnsBuilder.ToString();
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

        private string GenerateSelectBodyStmt()
        {
            var sqlBuilder = new StringBuilder();

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

            if (!string.IsNullOrEmpty(this._groupBySelectors))
            {
                sqlBuilder.Append("GROUP BY ");
                sqlBuilder.Append(this._groupBySelectors);
            }

            return sqlBuilder.ToString();
        }
    }
}
