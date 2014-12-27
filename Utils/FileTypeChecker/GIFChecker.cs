using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Utils.FileTypeChecker
{
    public class GIFChecker : IFileTypeChecker
    {
        private byte[] _sig87a = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 };
        private byte[] _sig89a = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 };
        private string _extension = ".gif";

        public bool Verify(string FileName)
        {
            if (Path.GetExtension(FileName).ToLower() != this._extension)
                return false;

            bool isGif87a = FileTypeCheckerUtil.Verify(FileName, this._sig87a, 0);

            if (!isGif87a)
                return FileTypeCheckerUtil.Verify(FileName, this._sig89a, 0);
            else
                return isGif87a;
        }

        public bool Verify(byte[] FileData)
        {
            bool isGif87a = FileTypeCheckerUtil.Verify(FileData, this._sig87a, 0);

            if (!isGif87a)
                return FileTypeCheckerUtil.Verify(FileData, this._sig89a, 0);
            else
                return isGif87a;
        }
    }
}
