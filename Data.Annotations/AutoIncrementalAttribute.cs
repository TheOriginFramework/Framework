using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Data.Annotations
{
    /// <summary>
    /// Mark the property as auto-incremental column in database.
    /// FOR INT/BIGINT/SMALLINT USAGE ONLY.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class AutoIncrementalAttribute : Attribute
    {
    }
}
