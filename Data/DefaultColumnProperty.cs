using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data
{
    public class ColumnPropertyMap : IColumnPropertyMap
    {
        public PropertyInfo ColumnPropertyInfo { get; private set; }
        public string ColumnDbName { get; private set; }
        public DbType ColumnDbType { get; private set; }
        public bool ColumnAllowNull { get; private set; }
        public bool ColumnIsKey { get; private set; }
        public int ColumnMaxLength { get; private set; }

        public ColumnPropertyMap(PropertyInfo ColumnPropertyInfo)
        {
            this.ColumnPropertyInfo = ColumnPropertyInfo;
            this.ColumnAllowNull = false;
            this.ColumnIsKey = false;
            this.ColumnDbName = ColumnPropertyInfo.Name;
            this.ColumnDbType = DbType.String;
        }

        public IColumnPropertyMap DbName(string Name)
        {
            this.ColumnDbName = Name;
            return this;
        }

        public IColumnPropertyMap AsKey()
        {
            this.ColumnIsKey = true;
            return this;
        }

        public IColumnPropertyMap MapDbType(System.Data.DbType Type)
        {
            this.ColumnDbType = Type;
            return this;
        }

        public IColumnPropertyMap AllowNull()
        {
            this.ColumnAllowNull = true;
            return this;
        }

        public IColumnPropertyMap MaxLength(int Length)
        {
            this.ColumnMaxLength = Length;
            return this;
        }
    }
}
