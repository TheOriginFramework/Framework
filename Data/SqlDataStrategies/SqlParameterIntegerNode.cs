using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data.SqlDataStrategies
{
    public class SqlParameterIntegerNode : SqlParameterNode
    {
        public SqlParameterIntegerNode(ISqlParameterParser Parser) : base(Parser) 
        {
        }

        public override IDbDataParameter GetParameter()
        {
            var propBindingInfo = this.Parser.GetPropertyBindingInfo();

            if (propBindingInfo.GetPropertyInfo().PropertyType != typeof(int))
            {
                if (this.Next != null)
                    return this.Next.GetParameter();
            }

            var param = new SqlParameter("@" + propBindingInfo.GetPropertyInfo().Name, null);
            param.DbType = (propBindingInfo.GetMapDbType() == null)
                ? DbType.Int32
                : propBindingInfo.GetMapDbType().Value;
            param.Size = (propBindingInfo.GetLength() == null)
                ? 4
                : propBindingInfo.GetLength().Value;
            return param;
        }
    }
}
