using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DbTypeAttribute : Attribute
    {
        public DbType DbType { get; private set; }

        public DbTypeAttribute(DbType Type)
        {
            this.DbType = Type;
        }
    }
}
