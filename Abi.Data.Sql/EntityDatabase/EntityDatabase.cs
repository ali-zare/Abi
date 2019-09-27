using System.Data;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabase<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabase(EntityDatabaseConnection Connection) => connection = Connection;

        private EntityDatabaseConnection connection;

        private EntityDatabaseInsert<TEntityContext, TEntity> insert;
        private EntityDatabaseUpdate<TEntityContext, TEntity> update;
        private EntityDatabaseDelete<TEntityContext, TEntity> delete;



        public EntityDatabase<TEntityContext, TEntity> Insert(TEntity Entity)
        {
            if (insert == null)
                insert = new EntityDatabaseInsert<TEntityContext, TEntity>(connection);

            insert.WithoutCallBack()
                  .Write(Entity);

            return this;
        }
        public EntityDatabase<TEntityContext, TEntity> Update(TEntity Entity)
        {
            if (update == null)
                update = new EntityDatabaseUpdate<TEntityContext, TEntity>(connection);

            update.WithoutCallBack()
                  .Write(Entity);

            return this;
        }
        public EntityDatabase<TEntityContext, TEntity> Delete(TEntity Entity)
        {
            if (delete == null)
                delete = new EntityDatabaseDelete<TEntityContext, TEntity>(connection);

            delete.WithoutCheckConcurrency()
                  .Write(Entity);

            return this;
        }



        public EntityDatabase<TEntityContext, TEntity> Insert(params TEntity[] Entities)
        {
            if (insert == null)
                insert = new EntityDatabaseInsert<TEntityContext, TEntity>(connection);

            insert.WithoutCallBack()
                  .Write(Entities);

            return this;
        }
        public EntityDatabase<TEntityContext, TEntity> Update(params TEntity[] Entities)
        {
            if (update == null)
                update = new EntityDatabaseUpdate<TEntityContext, TEntity>(connection);

            update.WithoutCallBack()
                  .Write(Entities);

            return this;
        }
        public EntityDatabase<TEntityContext, TEntity> Delete(params TEntity[] Entities)
        {
            if (delete == null)
                delete = new EntityDatabaseDelete<TEntityContext, TEntity>(connection);

            delete.WithoutCheckConcurrency()
                  .Write(Entities);

            return this;
        }



        public EntityDatabase<TEntityContext, TEntity> Insert(IEnumerable<TEntity> Entities)
        {
            if (insert == null)
                insert = new EntityDatabaseInsert<TEntityContext, TEntity>(connection);

            insert.WithoutCallBack()
                  .Write(Entities);

            return this;
        }
        public EntityDatabase<TEntityContext, TEntity> Update(IEnumerable<TEntity> Entities)
        {
            if (update == null)
                update = new EntityDatabaseUpdate<TEntityContext, TEntity>(connection);

            update.WithoutCallBack()
                  .Write(Entities);

            return this;
        }
        public EntityDatabase<TEntityContext, TEntity> Delete(IEnumerable<TEntity> Entities)
        {
            if (delete == null)
                delete = new EntityDatabaseDelete<TEntityContext, TEntity>(connection);

            delete.WithoutCheckConcurrency()
                  .Write(Entities);

            return this;
        }



        public EntityDatabaseInsert<TEntityContext, TEntity> Insert()
        {
            return new EntityDatabaseInsert<TEntityContext, TEntity>(connection);
        }
        public EntityDatabaseUpdate<TEntityContext, TEntity> Update()
        {
            return new EntityDatabaseUpdate<TEntityContext, TEntity>(connection);
        }
        public EntityDatabaseDelete<TEntityContext, TEntity> Delete()
        {
            return new EntityDatabaseDelete<TEntityContext, TEntity>(connection);
        }

        public EntityDatabaseAdhocSelect<TEntityContext, TEntity> Adhoc()
        {
            return new EntityDatabaseAdhocSelect<TEntityContext, TEntity>(connection);
        }
        public EntityDatabasePreparedSelect<TEntityContext, TEntity> Prepared()
        {
            return new EntityDatabasePreparedSelect<TEntityContext, TEntity>(connection);
        }
        public EntityDatabaseSelect<TEntityContext, TEntity> Select(string commandText)
        {
            return new EntityDatabasePreparedSelect<TEntityContext, TEntity>(connection).Select(commandText);
        }
    }








    public sealed class EntityDatabase<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        private EntityDatabase(EntityDatabaseConnection Connection) => connection = Connection;

        private EntityDatabaseConnection connection;

        public static EntityDatabase<TEntityContext> Connect(EntityDatabaseConnection Connection) => new EntityDatabase<TEntityContext>(Connection);

        public EntityDatabase<TEntityContext, TEntity> Table<TEntity>() where TEntity : Entity<TEntity>
        {
            return new EntityDatabase<TEntityContext, TEntity>(connection);
        }

        public EntityDatabaseAdhoc<TEntityContext> Adhoc()
        {
            return new EntityDatabaseAdhoc<TEntityContext>(connection, IsolationLevel.Unspecified);
        }
        public EntityDatabasePrepared<TEntityContext> Prepared()
        {
            return new EntityDatabasePrepared<TEntityContext>(connection, IsolationLevel.Unspecified);
        }
        public EntityDatabasePreparedSelect<TEntityContext> Select(string commandText)
        {
            return new EntityDatabasePrepared<TEntityContext>(connection, IsolationLevel.Unspecified).Select(commandText);
        }

        public EntityDatabaseContext<TEntityContext> Context(TEntityContext context)
        {
            return new EntityDatabaseContext<TEntityContext>(context, connection);
        }

        public EntityDatabaseContextTransaction<TEntityContext> Transaction(IsolationLevel isolation)
        {
            return new EntityDatabaseContextTransaction<TEntityContext>(connection, isolation);
        }
    }
}
