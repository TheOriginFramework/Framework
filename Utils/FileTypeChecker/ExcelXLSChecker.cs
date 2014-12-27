using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Utils.FileTypeChecker
{
    public class ExcelXLSChecker : IFileTypeChecker
    {
        // Common OLE header.
        private byte[] _signature = new byte[] { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 };
        private byte[] _subheader = new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 };
        private string _extension = ".xls";
        
        public bool Verify(string FileName)
        {
            if (Path.GetExtension(FileName).ToLower() != this._extension)
                return false;

            bool foundOLEHeader = FileTypeCheckerUtil.Verify(FileName, this._signature, 0);
            bool foundXLSHeader = FileTypeCheckerUtil.Verify(FileName, this._subheader, 512);

            return foundOLEHeader && foundXLSHeader;
        }

        public bool Verify(byte[] FileData)
        {
            bool foundOLEHeader = FileTypeCheckerUtil.Verify(FileData, this._signature, 0);
            bool foundXLSHeader = FileTypeCheckerUtil.Verify(FileData, this._subheader, 512);

            return foundOLEHeader && foundXLSHeader;
        }
    }
}
