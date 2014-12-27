using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultQueryOrderByExpression : IQueryOrderByExpression
    {
        private string _orderbyName = null;
        private OrderByDirection _direction = OrderByDirection.Asc;

        public virtual IQueryOrderByExpression OrderBy(string OrderByName)
        {
            this._orderbyName = OrderByName;
            this._direction = OrderByDirection.Asc;
            return this;
        }
        public IQueryOrderByExpression OrderByDesc(string OrderByName)
        {
            this._orderbyName = OrderByName;
            this._direction = OrderByDirection.Desc;
            return this;
        }

        public OrderByDirection GetOrderByDirection()
        {
            return this._direction;
        }

        public virtual string GetOrderByExpression()
        {
            return this._orderbyName + ((this._direction == OrderByDirection.Desc) ? " DESC" : "");
        }


    }
}
