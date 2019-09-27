namespace Abi.Data.Sql
{
    public class DatabaseAdhoc<TParameter>
    {
        internal DatabaseAdhoc(EntityDatabaseConnection Connection, TParameter Parameter)
        {
            parameter = Parameter;
            connection = Connection;
        }


        private TParameter parameter;
        private EntityDatabaseConnection connection;



        public DatabaseSelect Select(string commandText)
        {
            return new DatabaseAdhocSelect<TParameter>(connection, parameter).Select(commandText);
        }
        public DatabaseQuery Query(string commandText)
        {
            return new DatabaseAdhocQuery<TParameter>(connection, parameter).Query(commandText);
        }
    }








    public class DatabaseAdhoc
    {
        internal DatabaseAdhoc(EntityDatabaseConnection Connection) => connection = Connection;

        private EntityDatabaseConnection connection;



        public DatabaseAdhoc<TParameter> WithParameters<TParameter>(TParameter parameter)
        {
            return new DatabaseAdhoc<TParameter>(connection, parameter);
        }



        public DatabaseSelect Select(string commandText)
        {
            return new DatabaseAdhocSelect(connection).Select(commandText);
        }
        public DatabaseQuery Query(string commandText)
        {
            return new DatabaseAdhocQuery(connection).Query(commandText);
        }
    }
}
