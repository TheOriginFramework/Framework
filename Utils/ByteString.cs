using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TOF.Framework.Utils
{
    public class ByteString : IComparable<ByteString>, IComparer<ByteString>
    {
        private byte[] _byteData = null;

        public ByteString(byte[] Data)
        {
            this._byteData = Data;
        }

        public ByteString(string ByteString)
        {
            MemoryStream ms = new MemoryStream();

            for (int i = 0; i<ByteString.Length; i += 2)
            {
                byte b = Convert.ToByte(Convert.ToInt16(ByteString.Substring(i, 2), 16));
                ms.WriteByte(b);
            }

            ms.Flush();
            this._byteData = ms.ToArray();
        }

        public byte[] GetBytes()
        {
            return this._byteData;
        }

        public int GetByteLength()
        {
            return this._byteData.Length;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool Upper = false)
        {
            var builder = new StringBuilder();

            foreach (byte b in this._byteData)
                builder.Append((Upper) ? b.ToString("X2") : b.ToString("x2"));

            return builder.ToString();
        }

        public int CompareTo(ByteString other)
        {
            byte[] dy = other.GetBytes();

            if (this._byteData.Length != dy.Length)
                return -1;

            for (int i = 0; i < this._byteData.Length; i++)
            {
                if (this._byteData[i] != dy[i])
                    return -1;
            }

            return 0;
        }

        public int Compare(ByteString x, ByteString y)
        {
            byte[] dx = x.GetBytes();
            byte[] dy = y.GetBytes();

            if (dx.Length != dy.Length)
                return -1;

            for (int i =0; i<dx.Length; i++)
            {
                if (dx[i] != dy[i])
                    return -1;
            }

            return 0;
        }

        public static ByteString CombineByteStrings(params string[] CombineStrings)
        {
            List<ByteString> stringData = new List<ByteString>();

            foreach (string s in CombineStrings)
                stringData.Add(new ByteString(s));

            MemoryStream ms = new MemoryStream();

            foreach (var s in stringData)
                ms.Write(s.GetBytes(), 0, s.GetByteLength());

            ms.Flush();
            return new ByteString(ms.ToArray());
        }

        public static byte[] ConvertByteStringToBytes(string ByteString)
        {
            return (new ByteString(ByteString)).GetBytes();
        }

        public static string ConvertBytesToByteString(byte[] Bytes)
        {
            return (new ByteString(Bytes)).ToString(false);
        }

        public static string ConvertBytesToByteString(byte[] Bytes, bool Upper)
        {
            return (new ByteString(Bytes)).ToString(Upper);
        }
    }
}
