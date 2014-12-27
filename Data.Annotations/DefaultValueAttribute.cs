using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DefaultValueAttribute : Attribute
    {
        public object DefaultValue { get; private set; }
        public DefaultValueAttribute(object DefaultValue)
        {
            this.DefaultValue = DefaultValue;
        }
    }
}
