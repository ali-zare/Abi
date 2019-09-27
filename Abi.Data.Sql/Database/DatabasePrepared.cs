namespace Abi.Data.Sql
{
    public class DatabasePrepared<TParameter>
    {
        internal DatabasePrepared(EntityDatabaseConnection Connection, TParameter Parameter)
        {
            parameter = Parameter;
            connection = Connection;
        }


        private TParameter parameter;
        private EntityDatabaseConnection connection;



        public DatabaseSelect Select(string commandText)
        {
            return new DatabasePreparedSelect<TParameter>(connection, parameter).Select(commandText);
        }
        public DatabaseQuery Query(string commandText)
        {
            return new DatabasePreparedQuery<TParameter>(connection, parameter).Query(commandText);
        }
    }








    public class DatabasePrepared
    {
        internal DatabasePrepared(EntityDatabaseConnection Connection) => connection = Connection;

        private EntityDatabaseConnection connection;



        public DatabasePrepared<TParameter> WithParameters<TParameter>(TParameter parameter)
        {
            return new DatabasePrepared<TParameter>(connection, parameter);
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
