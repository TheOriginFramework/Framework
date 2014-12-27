using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class DateTimeConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return DateTime.MinValue;

            return System.Convert.ToDateTime(ValueToConvert);
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            return System.Convert.ToDateTime(Value1) == System.Convert.ToDateTime(Value2);
        }
        
        public Type GetCompatibleType()
        {
            return typeof(DateTime);
        }
    }
}
