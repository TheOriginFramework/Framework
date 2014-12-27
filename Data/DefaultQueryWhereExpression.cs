using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultQueryWhereExpression : IQueryWhereExpression
    {
        private WhereDirective _whereDirective = WhereDirective.Begin;
        private string _whereCondition = null;

        public IQueryWhereExpression Begin(string WhereCondition)
        {
            this._whereCondition = WhereCondition;
            this._whereDirective = WhereDirective.Begin;
            return this;
        }

        public IQueryWhereExpression And(string WhereCondition)
        {
            this._whereCondition = WhereCondition;
            this._whereDirective = WhereDirective.And;
            return this;
        }

        public IQueryWhereExpression Or(string WhereCondition)
        {
            this._whereCondition = WhereCondition;
            this._whereDirective = WhereDirective.Or;
            return this;
        }

        public IQueryWhereExpression NotAnd(string WhereCondition)
        {
            this._whereCondition = WhereCondition;
            this._whereDirective = WhereDirective.NotAnd;
            return this;
        }

        public IQueryWhereExpression NotOr(string WhereCondition)
        {
            this._whereCondition = WhereCondition;
            this._whereDirective = WhereDirective.NotOr;
            return this;
        }

        public WhereDirective GetWhereDirective()
        {
            return this._whereDirective;
        }

        public virtual string GetWhereExpression()
        {
            switch (this._whereDirective)
            {
                case WhereDirective.And:
                    return "AND (" + this._whereCondition + ")";
                case WhereDirective.Or:
                    return "OR (" + this._whereCondition + ")";
                case WhereDirective.NotAnd:
                    return "AND NOT (" + this._whereCondition + ")";
                case WhereDirective.NotOr:
                    return "OR NOT (" + this._whereCondition + ")";
                case WhereDirective.Begin:
                    return this._whereCondition;
                default:
                    throw new InvalidOperationException("ERROR_NOT_SUPPORTED_WHERE_DIRECTIVE");
            }
        }
    }
}
