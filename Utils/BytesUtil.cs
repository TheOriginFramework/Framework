using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TOF.Framework.Utils
{
    public class BytesUtil
    {
        public static byte[] Combine(byte[] data1, byte[] data2)
        {
            byte[] c = new byte[data1.Length + data2.Length];
            Buffer.BlockCopy(data1, 0, c, 0, data1.Length);
            Buffer.BlockCopy(data2, 0, c, data1.Length, data2.Length);
            return c; 
        }
    }
}
