using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Utils.TypeConverters
{
    public static class TypeConverterFactory
    {
        internal class TypeConverterRegistration
        {
            public Type ConverterType { get; set; }
        }

        private static Dictionary<Type, ITypeConverter> TypeConverters = null;

        static TypeConverterFactory()
        {
            TypeConverters = new Dictionary<Type, ITypeConverter>();

            // register built-in type converters.
            TypeConverters.Add(typeof(int), new IntegerConverter());
            TypeConverters.Add(typeof(long), new LongConverter());
            TypeConverters.Add(typeof(short), new ShortConverter());
            TypeConverters.Add(typeof(float), new FloatConverter());
            TypeConverters.Add(typeof(double), new DoubleConverter());
            TypeConverters.Add(typeof(decimal), new DecimalConverter());
            TypeConverters.Add(typeof(bool), new BooleanConverter());
            TypeConverters.Add(typeof(char), new CharConverter());
            TypeConverters.Add(typeof(DateTime), new DateTimeConverter());
            TypeConverters.Add(typeof(string), new StringConverter());
            TypeConverters.Add(typeof(byte[]), new ByteArrayConverter());
            TypeConverters.Add(typeof(Guid), new GuidConverter());
            TypeConverters.Add(typeof(Enum), new EnumConverter());
        }

        public static ITypeConverter GetConvertType<T>()
        {
            if (typeof(T).IsEnum)
                return TypeConverters[typeof(Enum)];

            if (TypeConverters.ContainsKey(typeof(T)))
                return TypeConverters[typeof(T)];

            return null;
        }

        public static ITypeConverter GetConvertType(Type T)
        {
            if (T.IsEnum)
                return TypeConverters[typeof(Enum)];

            if (TypeConverters.ContainsKey(T))
                return TypeConverters[T];

            return null;
        }
    }
}
