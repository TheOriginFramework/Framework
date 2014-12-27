using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DataColumnNameAttribute : Attribute
    {
        public string DataColumnName { get; private set; }
        public DataColumnNameAttribute(string DataColumnName)
        {
            this.DataColumnName = DataColumnName;
        }
    }
}
