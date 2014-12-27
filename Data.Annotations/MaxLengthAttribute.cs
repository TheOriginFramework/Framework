using TOF.Framework.Contracts.DataValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MaxLengthAttribute : Attribute, IValidateAttribute
    {
        public int Length { get; private set; }
        public MaxLengthAttribute(int Length)
        {
            this.Length = Length;
        }

        public bool IsValid(object Value)
        {
            if (Value == null)
                throw new ArgumentNullException("E_VALIDATE_VALUE_IS_NULL");

            string val = Value.ToString();
            return val.Length <= this.Length;
        }
    }
}
