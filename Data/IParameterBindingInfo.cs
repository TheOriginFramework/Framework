using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data
{
    public interface IParameterBindingInfo
    {
        PropertyInfo GetPropertyInfo();
        string GetParameterName();
        DbType? GetMapDbType();
        int? GetLength();
        bool IsAllowNull();
        IParameterBindingInfo DbName(string Name);
        IParameterBindingInfo MapDbType(DbType Type);
        IParameterBindingInfo AllowNull();
        IParameterBindingInfo MaxLength(int Length);
    }
}
