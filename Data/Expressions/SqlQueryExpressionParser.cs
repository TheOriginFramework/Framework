using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data.Expressions
{
    public class SqlQueryExpressionParser : IQueryExpressionParser
    {
        public string Parse(Expression ExpressionNode, IModelStrategy ModelStrategy)
        {
            var queryNode = SqlQueryExpressionFactory.GetNodeParser(ExpressionNode, ModelStrategy);

            if (queryNode == null)
                throw new NotSupportedException("ERROR_NODE_TYPE_NOT_SUPPORTED");

            return queryNode.Parse(ExpressionNode);
        }
    }
}
