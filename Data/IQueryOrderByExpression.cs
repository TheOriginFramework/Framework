using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IQueryOrderByExpression
    {
        IQueryOrderByExpression OrderBy(string OrderByName);
        IQueryOrderByExpression OrderByDesc(string OrderByName);
        OrderByDirection GetOrderByDirection();        
        string GetOrderByExpression();
    }
}
