using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class BooleanConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return false;

            if (string.IsNullOrEmpty(ValueToConvert.ToString()))
                return false;
            else if (ValueToConvert.ToString() == "0")
                return false;
            else if (ValueToConvert.ToString() == "1")
                return true;
            else
                return System.Convert.ToBoolean(ValueToConvert);
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            return System.Convert.ToBoolean(Value1) == System.Convert.ToBoolean(Value2);
        }

        public Type GetCompatibleType()
        {
            return typeof(bool);
        }
    }
}
