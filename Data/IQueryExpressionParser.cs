using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryExpressionParser
    {
        string Parse(Expression QueryExpressionRoot, IModelStrategy ModelStrategy);
    }
}
