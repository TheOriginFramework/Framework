using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using TOF.Framework.Data.Expressions;

namespace TOF.Framework.Data
{
    public class ColumnPropertyMap<TModel> where TModel: class, new()
    {
        public Dictionary<PropertyInfo, IColumnPropertyMap> PropertyMaps { get; private set; }

        public ColumnPropertyMap()
        {
            this.PropertyMaps = new Dictionary<PropertyInfo, IColumnPropertyMap>();
            this.Initialize();            
        }

        private void Initialize()
        {
            // load model's properties and generate default column property map.
            var properties = typeof(TModel).GetProperties().Where(c => c.DeclaringType == typeof(TModel));

            foreach (var property in properties)
            {
                IColumnPropertyMap propertyMap = new ColumnPropertyMap(property);
                this.PropertyMaps.Add(property, propertyMap);
            }
        }

        public IColumnPropertyMap Map(Expression<Func<TModel, object>> PropertyMapSpecifier)
        {
            var memberAccessExp = new SqlQueryMemberAccessExpressionNode();
            var property = memberAccessExp.Parse(PropertyMapSpecifier);
            
            // split with property name.
            var propQuery = this.PropertyMaps.Where(
                p => string.Equals(p.Key.Name, property.Split('.')[1], 
                    StringComparison.InvariantCultureIgnoreCase));

            if (!propQuery.Any())
                throw new KeyNotFoundException("ERROR_PROPERTY_NOT_FOUND");

            return this.PropertyMaps[propQuery.First().Key];
        }
    }
}
