using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Utils
{
    public interface IFileTypeChecker
    {
        bool Verify(string FileName);
        bool Verify(byte[] FileData);
    }
}
