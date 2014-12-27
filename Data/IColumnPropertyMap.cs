using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IColumnPropertyMap
    {
        IColumnPropertyMap DbName(string Name);
        IColumnPropertyMap AsKey();
        IColumnPropertyMap MapDbType(DbType Type);
        IColumnPropertyMap AllowNull();
        IColumnPropertyMap MaxLength(int Length);
    }
}
