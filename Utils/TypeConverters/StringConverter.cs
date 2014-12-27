using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class StringConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return string.Empty;

            return ValueToConvert.ToString();
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            if (Value1 == null && Value2 == null)
                return true;
            else if (Value1 == null || Value2 == null)
                return false;
            else
                return Value1.ToString() == Value2.ToString();
        }

        public Type GetCompatibleType()
        {
            return typeof(string);
        }
    }
}
