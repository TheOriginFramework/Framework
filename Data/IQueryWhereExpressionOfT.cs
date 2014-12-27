using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryWhereExpression<TModel> : IQueryWhereExpression where TModel: class, new()
    {
        IQueryWhereExpression Begin(Expression<Func<TModel, object>> WhereConditionSpecifier);
        IQueryWhereExpression And(Expression<Func<TModel, object>> WhereConditionSpecifier);
        IQueryWhereExpression Or(Expression<Func<TModel, object>> WhereConditionSpecifier);
        IQueryWhereExpression NotAnd(Expression<Func<TModel, object>> WhereConditionSpecifier);
        IQueryWhereExpression NotOr(Expression<Func<TModel, object>> WhereConditionSpecifier);
    }
}
