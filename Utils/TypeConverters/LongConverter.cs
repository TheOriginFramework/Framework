using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class LongConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return 0;

            return System.Convert.ToInt64(ValueToConvert);
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            return System.Convert.ToInt64(Value1) == System.Convert.ToInt64(Value2);
        }

        public Type GetCompatibleType()
        {
            return typeof(long);
        }
    }
}
