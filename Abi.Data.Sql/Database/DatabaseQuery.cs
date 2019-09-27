using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Abi.Data.Sql
{
    public sealed class DatabaseQuery
    {
        internal DatabaseQuery(EntityDatabaseConnection Connection, string CommandText)
        {
            connection = Connection;
            commandText = CommandText;
        }

        private string commandText;
        private EntityDatabaseConnection connection;


        public int Execute()
        {
            SqlCommand command = connection.GetCommand(commandText);

            DatabaseTrace.Append(command.CommandText, Constant.Query);

            if (connection.HasTransaction)
                command.Transaction = connection.Transaction;

            return command.ExecuteNonQuery();
        }

        public Task<int> ExecuteAsync()
        {
            return ExecuteAsync(CancellationToken.None);
        }
        public Task<int> ExecuteAsync(CancellationToken CancellationToken)
        {
            SqlCommand command = connection.GetCommand(commandText);

            DatabaseTrace.Append(command.CommandText, Constant.Query);

            if (connection.HasTransaction)
                command.Transaction = connection.Transaction;

            return command.ExecuteNonQueryAsync(CancellationToken);
        }
    }
}
