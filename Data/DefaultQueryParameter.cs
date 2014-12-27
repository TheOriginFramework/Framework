using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public class DefaultQueryParameter : IQueryParameter
    {
        public string ParameterName { get; private set; }
        public DbType ParameterDbType { get; private set; }
        public int Length { get; private set; }
        public bool IsInputParameter { get; private set; }
        public bool IsOuputParameter { get; private set; }
        public bool IsReturnValueParameter { get; private set; }
        public object ParameterValue { get; private set; }

        public IQueryParameter Name(string Name)
        {
            this.ParameterName = Name;
            return this;
        }

        public IQueryParameter MapToDbType(DbType Type)
        {
            this.ParameterDbType = Type;
            return this;
        }

        public IQueryParameter ForDataLength(int Length)
        {
            this.Length = Length;
            return this;
        }

        public IQueryParameter AsIn()
        {
            this.IsInputParameter = true;
            return this;
        }

        public IQueryParameter AsOut()
        {
            this.IsOuputParameter = true;
            return this;
        }

        public IQueryParameter AsReturnValue()
        {
            this.IsReturnValueParameter = true;
            return this;
        }

        public IQueryParameter Value<T>(T ParameterValue)
        {
            this.ParameterValue = ParameterValue;
            return this;
        }
    }
}
