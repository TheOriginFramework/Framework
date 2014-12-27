using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryFunctionNode
    {
        bool CheckForHandle(string FunctionName);
        string Parse(Expression FunctionExpressionNode);
    }
}
