using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Contracts.DataValidation
{
    public interface IValidateAttribute
    {
        bool IsValid(object Value);
    }
}
