using System.Data;

namespace Abi.Data.Sql
{
    public class EntityDatabaseContextTransaction<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal EntityDatabaseContextTransaction(EntityDatabaseConnection Connection, IsolationLevel Isolation)
        {
            isolation = Isolation;
            connection = Connection;
        }

        private IsolationLevel isolation;
        internal EntityDatabaseConnection connection;


        public EntityDatabaseAdhoc<TEntityContext> Adhoc()
        {
            return new EntityDatabaseAdhoc<TEntityContext>(connection, isolation);
        }
        public EntityDatabasePrepared<TEntityContext> Prepared()
        {
            return new EntityDatabasePrepared<TEntityContext>(connection, isolation);
        }
        public EntityDatabasePreparedSelect<TEntityContext> Select(string commandText)
        {
            return new EntityDatabasePrepared<TEntityContext>(connection, isolation).Select(commandText);
        }


        public EntityDatabaseContext<TEntityContext> Context(TEntityContext context)
        {
            return new EntityDatabaseContext<TEntityContext>(context, connection, isolation);
        }

    }
}
