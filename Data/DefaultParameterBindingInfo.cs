using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultParameterBindingInfo : IParameterBindingInfo
    {
        private PropertyInfo _propertyInfo = null;
        private string _name = null;
        private DbType? _dbType = null;
        private int? _length = null;
        private bool _allowNull = false;

        public DefaultParameterBindingInfo(PropertyInfo PropertyInfo)
        {
            this._propertyInfo = PropertyInfo;
            this._name = this._propertyInfo.Name;
        }

        public PropertyInfo GetPropertyInfo()
        {
            return this._propertyInfo;
        }

        public string GetParameterName()
        {
            return this._name;
        }

        public DbType? GetMapDbType()
        {
            return this._dbType;
        }

        public int? GetLength()
        {
            return this._length;
        }

        public bool IsAllowNull()
        {
            return this._allowNull;
        }

        public IParameterBindingInfo DbName(string Name)
        {
            this._name = Name;
            return this;
        }

        public IParameterBindingInfo MapDbType(DbType Type)
        {
            this._dbType = Type;
            return this;
        }

        public IParameterBindingInfo AllowNull()
        {
            this._allowNull = true;
            return this;
        }

        public IParameterBindingInfo MaxLength(int Length)
        {
            this._length = Length;
            return this;
        }
    }
}
