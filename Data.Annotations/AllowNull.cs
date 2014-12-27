using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AllowNullAttribute : Attribute
    {
        public bool AllowNull { get; private set; }

        public AllowNullAttribute()
        {
            this.AllowNull = true;
        }
        public AllowNullAttribute(bool AllowNull)
        {
            this.AllowNull = AllowNull;
        }
    }
}
