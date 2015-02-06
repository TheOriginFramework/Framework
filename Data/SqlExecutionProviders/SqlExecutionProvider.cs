using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TOF.Framework.Data.SqlExecutionProviders
{
    public class SqlExecutionProvider : ISqlExecutionProvider, ISqlExecutionTransactionProvider
    {
        private string _connectionString = null;
        private SqlConnection _connection = null;
        private SqlTransaction _transactionContext = null;

        public SqlExecutionProvider(string ConnectionString)
        {
            this._connectionString = ConnectionString;
            this._connection = new SqlConnection(this._connectionString);
        }

        public void Open()
        {
            this._connection.Open();
        }

        public void Close()
        {
            this._connection.Close();
        }

        public void BeginTransaction()
        {
            this._transactionContext = 
                this._connection.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel Level)
        {
            this._transactionContext = 
                this._connection.BeginTransaction(Level);
        }

        public void Commit()
        {
            this._transactionContext.Commit();
        }

        public void Rollback()
        {
            this._transactionContext.Rollback();
        }

        public int Execute(string Statement, IEnumerable<IDbDataParameter> Parameters)
        {
            SqlCommand command = new SqlCommand(Statement, this._connection);

            if (this._transactionContext != null)
                command.Transaction = this._transactionContext;

            foreach (var p in Parameters)
                command.Parameters.Add(p as SqlParameter);

            return command.ExecuteNonQuery();
        }

        public IDataReader ExecuteQuery(string Statement, IEnumerable<IDbDataParameter> Parameters)
        {
            SqlCommand command = new SqlCommand(Statement, this._connection);

            if (this._transactionContext != null)
                command.Transaction = this._transactionContext;

            if (Parameters != null)
            {
                foreach (var p in Parameters)
                    command.Parameters.Add(p as SqlParameter);
            }

            return command.ExecuteReader();
        }

        public object ExecuteQueryGetScalar(string Statement, IEnumerable<IDbDataParameter> Parameters)
        {
            SqlCommand command = new SqlCommand(Statement, this._connection);

            if (this._transactionContext != null)
                command.Transaction = this._transactionContext;

            if (Parameters != null)
            {
                foreach (var p in Parameters)
                    command.Parameters.Add(p as SqlParameter);
            }

            object result = command.ExecuteScalar();
            return result;
        }

        public object ExecuteProcedureQueryGetScalar(string ProcedureName, IEnumerable<IDbDataParameter> Parameters)
        {
            SqlCommand command = new SqlCommand(ProcedureName, this._connection);
            command.CommandType = CommandType.StoredProcedure;

            if (this._transactionContext != null)
                command.Transaction = this._transactionContext;

            foreach (var p in Parameters)
                command.Parameters.Add(p as SqlParameter);

            object result = command.ExecuteScalar();
            return result;
        }


        public int ExecuteProcedure(string ProcedureName, IEnumerable<IDbDataParameter> Parameters)
        {
            SqlCommand command = new SqlCommand(ProcedureName, this._connection);
            command.CommandType = CommandType.StoredProcedure;

            if (this._transactionContext != null)
                command.Transaction = this._transactionContext;

            foreach (var p in Parameters)
                command.Parameters.Add(p as SqlParameter);

            return command.ExecuteNonQuery();
        }

        public IDataReader ExecuteProcedureQuery(string ProcedureName, IEnumerable<IDbDataParameter> Parameters)
        {
            SqlCommand command = new SqlCommand(ProcedureName, this._connection);
            command.CommandType = CommandType.StoredProcedure;

            if (this._transactionContext != null)
                command.Transaction = this._transactionContext;

            if (Parameters != null)
            {
                foreach (var p in Parameters)
                    command.Parameters.Add(p as SqlParameter);
            }

            return command.ExecuteReader();
        }
    }
}
