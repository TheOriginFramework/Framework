using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TOF.Framework.Data
{
    public class SqlParameterParser : ISqlParameterParser
    {
        private ISqlParameterChainNode NodeStart { get; set; }
        private IPropertyBindingInfo PropertyBindingInfo { get; set; }

        public SqlParameterParser()
        {
            this.NodeStart = null;
            this.Initialize();
        }

        private void Initialize()
        {
            this.NodeStart = this.PrepareParameterChains();
        }

        private ISqlParameterChainNode PrepareParameterChains()
        {
            List<ISqlParameterChainNode> nodes = new List<ISqlParameterChainNode>()
            {
                new SqlDataStrategies.SqlParameterStringNode(this),
                new SqlDataStrategies.SqlParameterIntegerNode(this),
                new SqlDataStrategies.SqlParameterDateTimeNode(this),
                new SqlDataStrategies.SqlParameterByteNode(this),
                new SqlDataStrategies.SqlParameterBytesNode(this),
                new SqlDataStrategies.SqlParameterLongNode(this),
                new SqlDataStrategies.SqlParameterUniqueidentifierNode(this),
                new SqlDataStrategies.SqlParameterBooleanNode(this),
                new SqlDataStrategies.SqlParameterCharNode(this),
                new SqlDataStrategies.SqlParameterDecimalNode(this),
                new SqlDataStrategies.SqlParameterShortNode(this),
                new SqlDataStrategies.SqlParameterDoubleNode(this),
                new SqlDataStrategies.SqlParameterSingleNode(this)
            };

            for (var i = 0; i < nodes.Count - 1; i++ )
            {
                (nodes[i] as SqlParameterNode).NextNode(nodes[i + 1]);
            }

            return nodes.First();
        }

        public SqlParameterParser ForProperty(IPropertyBindingInfo PropertyBindingInfo)
        {
            this.PropertyBindingInfo = PropertyBindingInfo;
            return this;
        }

        public IDbDataParameter GetParameter()
        {
            return this.NodeStart.GetParameter();
        }

        public IPropertyBindingInfo GetPropertyBindingInfo()
        {
            return this.PropertyBindingInfo;
        }
    }
}
