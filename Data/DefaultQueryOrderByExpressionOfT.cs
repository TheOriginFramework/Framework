using TOF.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultQueryOrderByExpression<TModel> : IQueryOrderByExpression<TModel> where TModel : class, new()
    {
        private Expression<Func<TModel, object>> _orderBySpecifier = null;
        private string _orderbyName = null;
        private OrderByDirection _direction = OrderByDirection.Asc;

        public IQueryOrderByExpression OrderBy(Expression<Func<TModel, object>> OrderBySpecifier)
        {
            this._orderBySpecifier = OrderBySpecifier;
            this._direction = OrderByDirection.Asc;
            return this;
        }

        public IQueryOrderByExpression OrderByDesc(Expression<Func<TModel, object>> OrderBySpecifier)
        {
            this._orderBySpecifier = OrderBySpecifier;
            this._direction = OrderByDirection.Desc;
            return this;
        }

        public IQueryOrderByExpression OrderBy(string OrderByName)
        {
            var propQuery = typeof(TModel).GetProperties().Where(c =>
                string.Compare(OrderByName, c.Name, StringComparison.InvariantCulture) == 0);

            if (propQuery.Any())
                this._orderbyName = OrderByName;
            else
                throw new ArgumentException("ERROR_ORDER_BY_COLUMN_NOT_FOUND");

            this._direction = OrderByDirection.Asc;

            return this;
        }

        public IQueryOrderByExpression OrderByDesc(string OrderByName)
        {
            var propQuery = typeof(TModel).GetProperties().Where(c =>
                string.Compare(OrderByName, c.Name, StringComparison.InvariantCulture) == 0);

            if (propQuery.Any())
                this._orderbyName = OrderByName;
            else
                throw new ArgumentException("ERROR_ORDER_BY_COLUMN_NOT_FOUND");

            this._direction = OrderByDirection.Desc;

            return this;
        }

        public string GetOrderByExpression()
        {
            if (this._orderBySpecifier != null)
                return ExpressionUtil.GetMapPropertyName<TModel>(this._orderBySpecifier)
                     + ((this._direction == OrderByDirection.Desc) ? " DESC" : "");
            else
                return this._orderbyName + ((this._direction == OrderByDirection.Desc) ? " DESC" : "");
        }


        public OrderByDirection GetOrderByDirection()
        {
            return this._direction;
        }
    }
}
