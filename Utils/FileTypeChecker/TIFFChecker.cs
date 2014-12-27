using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Utils.FileTypeChecker
{
    public class TIFFChecker : IFileTypeChecker
    {
        private byte[] _signature = new byte[] { 0x49, 0x49, 0x2a, 0x00 };
        private string _extension = ".tif;.tiff";

        public bool Verify(string FileName)
        {
            if (!this._extension.Split(';').Any(ext => ext == Path.GetExtension(FileName).ToLower()))
                return false;

            return FileTypeCheckerUtil.Verify(FileName, this._signature, 0);
        }

        public bool Verify(byte[] FileData)
        {
            return FileTypeCheckerUtil.Verify(FileData, this._signature, 0);
        }
    }
}
