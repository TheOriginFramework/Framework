using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Utils.FileTypeChecker
{
    internal static class FileTypeCheckerUtil
    {
        public static bool Verify(string FileName, byte[] Signature, int Offset)
        {
            if (!File.Exists(FileName))
                throw new FileNotFoundException();

            FileStream fileToVerify = File.OpenRead(FileName);
            byte[] data = new byte[Signature.Length];

            fileToVerify.Read(data, Offset, data.Length);
            fileToVerify.Close();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != Signature[i])
                    return false;
            }

            return true;
        }

        public static bool Verify(byte[] Data, byte[] Signature, int Offset)
        {
            if (Signature == null || Signature.Length == 0)
                throw new ArgumentException("E_SIGNATURE_NOT_FOUND");
            if (Offset >= Data.Length)
                throw new ArgumentException("E_SIGNAUTRE_OFFSET_OVERFLOW");
            if (Data == null || Data.Length == 0 || Data.Length < Signature.Length)
                throw new ArgumentException("E_FILE_TYPE_DATA_INVALID");

            MemoryStream dataToVerify = new MemoryStream(Data);
            dataToVerify.Position = 0;
            byte[] data = new byte[Signature.Length];

            dataToVerify.Read(data, Offset, data.Length);
            dataToVerify.Close();

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != Signature[i])
                    return false;
            }

            return true;
        }
    }
}
