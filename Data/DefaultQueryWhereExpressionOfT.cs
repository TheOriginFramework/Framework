using TOF.Framework.Data.Exceptions;
using TOF.Framework.Data.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultQueryWhereExpression<TModel> : IQueryWhereExpression<TModel> where TModel: class, new()
    {
        private Expression<Func<TModel, object>> _whereConditionSpecifier = null;
        private string _whereCondition = null;
        private WhereDirective _whereDirective = WhereDirective.Begin;
        private IModelStrategy _modelStrategy = null;

        public DefaultQueryWhereExpression(IModelStrategy ModelStrategy)
        {
            this._modelStrategy = ModelStrategy;
        }

        public IQueryWhereExpression Begin(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._whereConditionSpecifier = WhereConditionSpecifier;
            this._whereDirective = WhereDirective.Begin;
            return this;
        }

        public IQueryWhereExpression And(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._whereConditionSpecifier = WhereConditionSpecifier;
            this._whereDirective = WhereDirective.And;
            return this;
        }

        public IQueryWhereExpression Or(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._whereConditionSpecifier = WhereConditionSpecifier;
            this._whereDirective = WhereDirective.Or;
            return this;
        }

        public IQueryWhereExpression NotAnd(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._whereConditionSpecifier = WhereConditionSpecifier;
            this._whereDirective = WhereDirective.NotAnd;
            return this;
        }

        public IQueryWhereExpression NotOr(Expression<Func<TModel, object>> WhereConditionSpecifier)
        {
            this._whereConditionSpecifier = WhereConditionSpecifier;
            this._whereDirective = WhereDirective.NotOr;
            return this;
        }

        public IQueryWhereExpression Begin(string WhereCondition)
        {
            var propQuery = typeof(TModel).GetProperties().Where(c =>
                string.Compare(WhereCondition, c.Name, StringComparison.InvariantCulture) == 0);

            if (propQuery.Any())
                this._whereCondition = WhereCondition;
            else
                throw new ArgumentException("ERROR_WHERE_COLUMN_NOT_FOUND");

            this._whereDirective = WhereDirective.Begin;

            return this;
        }

        public IQueryWhereExpression And(string WhereCondition)
        {
            var propQuery = typeof(TModel).GetProperties().Where(c =>
                string.Compare(WhereCondition, c.Name, StringComparison.InvariantCulture) == 0);

            if (propQuery.Any())
                this._whereCondition = WhereCondition;
            else
                throw new ArgumentException("ERROR_WHERE_COLUMN_NOT_FOUND");

            this._whereDirective = WhereDirective.And;

            return this;
        }

        public IQueryWhereExpression Or(string WhereCondition)
        {
            var propQuery = typeof(TModel).GetProperties().Where(c =>
                string.Compare(WhereCondition, c.Name, StringComparison.InvariantCulture) == 0);

            if (propQuery.Any())
                this._whereCondition = WhereCondition;
            else
                throw new ArgumentException("ERROR_WHERE_COLUMN_NOT_FOUND");

            this._whereDirective = WhereDirective.Or;

            return this;
        }

        public IQueryWhereExpression NotAnd(string WhereCondition)
        {
            var propQuery = typeof(TModel).GetProperties().Where(c =>
                string.Compare(WhereCondition, c.Name, StringComparison.InvariantCulture) == 0);

            if (propQuery.Any())
                this._whereCondition = WhereCondition;
            else
                throw new ArgumentException("ERROR_WHERE_COLUMN_NOT_FOUND");

            this._whereDirective = WhereDirective.NotAnd;

            return this;
        }

        public IQueryWhereExpression NotOr(string WhereCondition)
        {
            var propQuery = typeof(TModel).GetProperties().Where(c =>
                string.Compare(WhereCondition, c.Name, StringComparison.InvariantCulture) == 0);

            if (propQuery.Any())
                this._whereCondition = WhereCondition;
            else
                throw new ArgumentException("ERROR_WHERE_COLUMN_NOT_FOUND");

            this._whereDirective = WhereDirective.NotOr;

            return this;
        }

        public WhereDirective GetWhereDirective()
        {
            return this._whereDirective;
        }

        public string GetWhereExpression()
        {
            if (string.IsNullOrEmpty(this._whereCondition))
                this._whereCondition = this.ParseExpression(this._whereConditionSpecifier);

            switch (this._whereDirective)
            {
                case WhereDirective.And:
                    return "AND (" + this._whereCondition + ") ";
                case WhereDirective.Or:
                    return "OR (" + this._whereCondition + ") ";
                case WhereDirective.NotAnd:
                    return "AND NOT (" + this._whereCondition + ") ";
                case WhereDirective.NotOr:
                    return "OR NOT (" + this._whereCondition + ") ";
                case WhereDirective.Begin:
                    return "(" + this._whereCondition + ") ";
                default:
                    throw new InvalidOperationException("ERROR_NOT_SUPPORTED_WHERE_DIRECTIVE");
            }
        }

        private string ParseExpression(Expression<Func<TModel, object>> FilterExpressions)
        {
            IQueryExpressionParser sqlQueryParser = new SqlQueryExpressionParser();

            switch (FilterExpressions.Body.NodeType)
            {
                case ExpressionType.Not:
                    return sqlQueryParser.Parse((FilterExpressions.Body as UnaryExpression), this._modelStrategy);

                case ExpressionType.Assign:
                case ExpressionType.AndAlso:
                case ExpressionType.Equal:
                    return sqlQueryParser.Parse((FilterExpressions.Body as BinaryExpression), this._modelStrategy);

                case ExpressionType.Convert:
                    return sqlQueryParser.Parse((FilterExpressions.Body as UnaryExpression), this._modelStrategy);

                default:
                    throw new NotSupportedException("ERROR_GIVEN_EXPRESSION_NOT_SUPPORTED");
            }
        }
    }
}
