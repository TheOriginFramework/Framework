using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOF.Framework.Utils;

namespace TOF.Framework.Utils.TypeConverters
{
    public class ByteArrayConverter : ITypeConverter
    {
        public object Convert(object ValueToConvert)
        {
            if (ValueToConvert == null || ValueToConvert == DBNull.Value)
                return new byte[] { };

            return (byte[])ValueToConvert;
        }

        public bool VerifyEquals(object Value1, object Value2)
        {
            byte[] data1 = (byte[])Value1;
            byte[] data2 = (byte[])Value2;

            if (data1.Length != data2.Length)
                return false;

            for (int i = 0; i < data1.Length; i++)
            {
                if (data1[i] != data1[2])
                    return false;
            }

            return true;
        }

        public Type GetCompatibleType()
        {
            return typeof(byte[]);
        }
    }
}
