using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public abstract class SqlParameterNode : ISqlParameterChainNode
    {
        protected ISqlParameterChainNode Next { get; set; }
        protected ISqlParameterParser Parser { get; set; }

        public SqlParameterNode(ISqlParameterParser Parser)
        {
            this.Parser = Parser;
        }

        public abstract IDbDataParameter GetParameter();
        public ISqlParameterChainNode NextNode(ISqlParameterChainNode NodeNext)
        {
            this.Next = NodeNext;
            return this;
        }
    }
}
