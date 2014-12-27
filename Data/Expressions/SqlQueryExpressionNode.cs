using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public abstract class SqlQueryExpressionNode : IQueryExpressionNode
    {
        public IModelStrategy ModelStrategy { get; set; }

        public abstract string Parse(Expression ExpressionNode);
    }
}
