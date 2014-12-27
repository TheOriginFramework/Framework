using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Utils.TypeConverters
{
    public interface ITypeConverter
    {
        object Convert(object ValueToConvert);
        bool VerifyEquals(object Value1, object Value2);
        Type GetCompatibleType();
    }
}
