using TOF.Framework.Contracts.DataValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOF.Framework.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RangeAttribute : Attribute, IValidateAttribute
    {
        public object Max { get; private set; }
        public object Min { get; private set; }
        public Type DataType { get; private set; }
        public Func<object, object> ValueConverter { get; private set; }

        public RangeAttribute(double Max, double Min)
        {
            this.Max = Max;
            this.Min = Min;
            this.DataType = typeof(double);
        }

        public RangeAttribute(int Max, int Min)
        {
            this.Max = Max;
            this.Min = Min;
            this.DataType = typeof(int);
        }

        public RangeAttribute(Type DataType, string Max, string Min)
        {
            this.Max = Max;
            this.Min = Min;
            this.DataType = DataType;
        }

        public bool IsValid(object Value)
        {
            this.GetConverters();

            if (Value == null)
                throw new ArgumentNullException("E_VALIDATE_VALUE_IS_NULL");

            string str = Value as string;

            if ((str != null) && string.IsNullOrEmpty(str))
                return true;

            object obj2 = null;

            try
            {
                obj2 = this.ValueConverter(Value);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }

            IComparable minComparer = (IComparable)this.Min;
            IComparable maxComparer = (IComparable)this.Max;

            return ((minComparer.CompareTo(obj2) <= 0) && (maxComparer.CompareTo(obj2) >= 0));
        }

        private void GetConverters()
        {
            Type t1 = this.DataType;

            if (t1 == null)
                throw new InvalidOperationException("E_RANGE_DATATYPE_NOT_FOUND");

            Type t2 = typeof(IComparable);

            if (t2.IsAssignableFrom(t1))
                throw new InvalidOperationException("E_RANGE_TARGET_NOT_COMPARE");

            TypeConverter converter = TypeDescriptor.GetConverter(t1);
            IComparable maxConverter = (IComparable)converter.ConvertFromString(this.Max.ToString());
            IComparable minConverter = (IComparable)converter.ConvertFromString(this.Min.ToString());

            this.ValueConverter = delegate(object value)
            {
                if ((value != null) && (value.GetType() == t1))
                {
                    return value;
                }
                return converter.ConvertFrom(value);
            };

            this.Max = maxConverter;
            this.Min = minConverter;
        }
    }
}
