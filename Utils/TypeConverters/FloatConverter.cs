using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class FloatConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return 0.0f;

            return System.Convert.ToSingle(ValueToConvert);
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            return System.Convert.ToSingle(Value1) == System.Convert.ToSingle(Value2);
        }
        
        public Type GetCompatibleType()
        {
            return typeof(float);
        }
    }
}
