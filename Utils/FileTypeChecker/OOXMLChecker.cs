using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Utils.FileTypeChecker
{
    public class OOXMLChecker : IFileTypeChecker
    {
        // Common OOXML (for XLSX, DOCX, PPTX format)
        private byte[] _signature = new byte[] { 0x50, 0x4b, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 };
        private string _extension = ".xlsx;.docx;.pptx";
        
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
