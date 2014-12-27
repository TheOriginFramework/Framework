using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class DecimalConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return 0.0m;

            return System.Convert.ToDecimal(ValueToConvert);
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            return System.Convert.ToDecimal(Value1) == System.Convert.ToDecimal(Value2);
        }
        
        public Type GetCompatibleType()
        {
            return typeof(decimal);
        }
    }
}
