using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class DoubleConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return 0.0d;

            return System.Convert.ToDouble(ValueToConvert);
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            return System.Convert.ToDouble(Value1) == System.Convert.ToDouble(Value2);
        }
        
        public Type GetCompatibleType()
        {
            return typeof(double);
        }
    }
}
