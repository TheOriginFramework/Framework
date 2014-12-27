using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class EnumConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            throw new NotImplementedException();
        }

        public object Convert(Type EnumType, object ValueToConvert)
        {
            if (!EnumType.IsEnum)
                throw new InvalidOperationException("ERROR_TYPE_IS_NOT_ENUMERATION");

            return System.Convert.ChangeType(Enum.Parse(EnumType, ValueToConvert.ToString()), EnumType);
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Type EnumType, object Value1, object Value2)
        {
            if (!EnumType.IsEnum)
                throw new InvalidOperationException("ERROR_TYPE_IS_NOT_ENUMERATION");
            
            return
                System.Convert.ChangeType(Enum.Parse(EnumType, Value1.ToString()), EnumType) ==
                System.Convert.ChangeType(Enum.Parse(EnumType, Value2.ToString()), EnumType); 
        }
        
        public Type GetCompatibleType()
        {
            return typeof(Enum);
        }
    }
}
