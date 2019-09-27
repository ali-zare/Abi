using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Abi.Data.Sql
{
    public class EntityDatabaseConnection : IDisposable
    {
        public EntityDatabaseConnection(string ConnectionString, int CommandTimeout = 0)
        {
            SetProvider(ConnectionString, CommandTimeout);
        }
        public EntityDatabaseConnection(string ConnectionString, SqlCredential Credential, int CommandTimeout = 0)
        {
            SetProvider(ConnectionString, CommandTimeout, Credential);
        }



        private int CommandTimeout { get; set; }
        private SqlCommand Command { get; set; }
        private bool IsMultiCommand { get; set; }
        internal SqlConnection Provider { get; private set; }
        internal SqlTransaction Transaction { get; private set; }
        public bool HasTransaction => Transaction != null;



        private void SetProvider(string ConnectionString, int CommandTimeout, SqlCredential Credential = null)
        {
            this.CommandTimeout = CommandTimeout;

            if (Credential == null)
                Provider = new SqlConnection(ConnectionString);
            else
                Provider = new SqlConnection(ConnectionString, Credential);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);

            bool MultipleActiveResultSets = builder.MultipleActiveResultSets;

            if (MultipleActiveResultSets)
                IsMultiCommand = true;
            else
                Command = Provider.CreateCommand();

            if (Command != null) Command.CommandTimeout = CommandTimeout;
        }
        internal SqlCommand GetCommand(string CommandText)
        {
            SqlCommand GetCurrentCommand() { Command.CommandText = CommandText; return Command; }
            SqlCommand GetNewCommand() => new SqlCommand(CommandText, Provider) { CommandTimeout = CommandTimeout };

            return IsMultiCommand ? GetNewCommand() : GetCurrentCommand();
        }

        public void Open() { Provider.Open(); DatabaseTrace.Append($"Connection Opened"); }
        public void Close() { Provider.Close(); DatabaseTrace.Append($"Connection Closed"); }
        public bool IsOpen() => Provider.State == ConnectionState.Open;
        public Task OpenAsync() => OpenAsync(CancellationToken.None);
        public Task OpenAsync(CancellationToken CancellationToken) => Provider.OpenAsync(CancellationToken);

        public void BeginTransaction(IsolationLevel Isolation) { Transaction = Provider.BeginTransaction(Isolation); DatabaseTrace.Append($"{Isolation} Transaction Begin"); }
        public void CommitTransaction() { Transaction.Commit(); DatabaseTrace.Append($"Transaction Committd"); }
        public void RollbackTransaction() { Transaction.Rollback(); DatabaseTrace.Append($"Transaction Rollbacked"); }


        public void Dispose() { Command?.Dispose(); Transaction?.Dispose(); Provider.Dispose(); }
    }
}
