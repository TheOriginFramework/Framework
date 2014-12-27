using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Utils.FileTypeChecker
{
    public class JPGChecker : IFileTypeChecker
    {
        // common JPEG header.
        private byte[] _signature = new byte[] { 0xff, 0xd8, 0xff }; 
        private string _extension = ".jpg;.jpeg";

        public bool Verify(string FileName)
        {            
            if (!this._extension.Split(';').All(ext => ext == Path.GetExtension(FileName).ToLower()))
                return false;

            return FileTypeCheckerUtil.Verify(FileName, this._signature, 0);
        }

        public bool Verify(byte[] FileData)
        {
            return FileTypeCheckerUtil.Verify(FileData, this._signature, 0);
        }
    }
}
