using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryOrderByExpression<TModel> : IQueryOrderByExpression where TModel: class, new()
    {
        IQueryOrderByExpression OrderBy(Expression<Func<TModel, object>> OrderBySpecifier);
        IQueryOrderByExpression OrderByDesc(Expression<Func<TModel, object>> OrderBySpecifier);
    }
}
