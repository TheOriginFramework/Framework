using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data
{
    public interface ISqlParameterParser
    {
        IPropertyBindingInfo GetPropertyBindingInfo();
        IDbDataParameter GetParameter();
    }
}
