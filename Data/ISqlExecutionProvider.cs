﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data
{
    public interface ISqlExecutionProvider
    {
        void Open();
        void Close();
        int Execute(string Statement, IEnumerable<IDbDataParameter> Parameters);
        int ExecuteProcedure(string ProcedureName, IEnumerable<IDbDataParameter> Parameters);
        IDataReader ExecuteGetReader(string Statement, IEnumerable<IDbDataParameter> Parameters);
        IDataReader ExecuteProcedureGetReader(string ProcedureName, IEnumerable<IDbDataParameter> Parameters);
    }
}