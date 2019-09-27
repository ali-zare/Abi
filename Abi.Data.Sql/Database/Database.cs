namespace Abi.Data.Sql
{
    public sealed class Database 
    {
        private Database(EntityDatabaseConnection Connection) => connection = Connection;

        private EntityDatabaseConnection connection;

        public static Database Connect(EntityDatabaseConnection Connection) => new Database(Connection);

        public DatabaseAdhoc Adhoc()
        {
            return new DatabaseAdhoc(connection);
        }
        public DatabasePrepared Prepared()
        {
            return new DatabasePrepared(connection);
        }
        public DatabaseSelect Select(string commandText)
        {
            return new DatabasePreparedSelect(connection).Select(commandText);
        }
        public DatabaseQuery Query(string commandText)
        {
            return new DatabasePreparedQuery(connection).Query(commandText);
        }
    }
}
