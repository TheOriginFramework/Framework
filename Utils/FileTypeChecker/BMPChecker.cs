using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Utils.FileTypeChecker
{
    public class BMPChecker : IFileTypeChecker
    {
        private byte[] _signature = new byte[] { 0x42, 0x4d };
        private string _extension = ".bmp";

        public bool Verify(string FileName)
        {
            if (Path.GetExtension(FileName).ToLower() != this._extension)
                return false;

            return FileTypeCheckerUtil.Verify(FileName, this._signature, 0);
        }

        public bool Verify(byte[] FileData)
        {
            return FileTypeCheckerUtil.Verify(FileData, this._signature, 0);
        }
    }
}
