using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data.SqlDataStrategies
{
    public class SqlParameterShortNode : SqlParameterNode
    {
        public SqlParameterShortNode(ISqlParameterParser Parser) : base(Parser) 
        {
        }

        public override IDbDataParameter GetParameter()
        {
            var propBindingInfo = this.Parser.GetPropertyBindingInfo();

            if (propBindingInfo.GetPropertyInfo().PropertyType != typeof(short))
            {
                if (this.Next != null)
                    return this.Next.GetParameter();
            }

            var param = new SqlParameter("@" + propBindingInfo.GetPropertyInfo().Name, null);
            param.DbType = (propBindingInfo.GetMapDbType() == null)
                ? DbType.Int16
                : propBindingInfo.GetMapDbType().Value;
            param.Size = (propBindingInfo.GetLength() == null)
                ? 2
                : propBindingInfo.GetLength().Value;
            return param;
        }
    }
}
