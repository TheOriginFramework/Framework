using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryExpressionNode
    {
        string Parse(Expression ExpressionNode);
    }
}
