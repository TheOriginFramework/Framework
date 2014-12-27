using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryWhereExpression
    {
        IQueryWhereExpression Begin(string WhereCondition);
        IQueryWhereExpression And(string WhereCondition);
        IQueryWhereExpression Or(string WhereCondition);
        IQueryWhereExpression NotAnd(string WhereCondition);
        IQueryWhereExpression NotOr(string WhereCondition);
        WhereDirective GetWhereDirective();
        string GetWhereExpression();
    }
}
