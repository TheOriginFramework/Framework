using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class CharConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return (char)0x0;

            return System.Convert.ToChar(ValueToConvert);
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            return System.Convert.ToChar(Value1) == System.Convert.ToChar(Value2);
        }
        
        public Type GetCompatibleType()
        {
            return typeof(char);
        }
    }
}
