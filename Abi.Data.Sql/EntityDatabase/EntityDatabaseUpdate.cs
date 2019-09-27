using System;
using System.Linq.Expressions;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseUpdate<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseUpdate(EntityDatabaseConnection Connection) => connection = Connection;


        private EntityDatabaseConnection connection;


        public EntityDatabaseWriterUpdate<TEntityContext, TEntity> WithoutCallBack()
        {
            return new EntityDatabaseWriterUpdate<TEntityContext, TEntity>(connection);
        }
        public EntityDatabaseUpdate<TEntityContext, TEntity, TEntityCallBack> WithCallBack<TEntityCallBack>(Expression<Func<TEntity, TEntityCallBack>> EntityCallBack)
        {
            if (EntityCallBack == null)
                throw new EntityDatabaseException($"{typeof(EntityDatabaseUpdate<TEntityContext, TEntity>).Name} Write, EntityCallBack Argument Is Null");

            return new EntityDatabaseUpdate<TEntityContext, TEntity, TEntityCallBack>(connection);
        }
    }








    public sealed class EntityDatabaseUpdate<TEntityContext, TEntity, TEntityCallBack> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseUpdate(EntityDatabaseConnection Connection) => connection = Connection;


        private EntityDatabaseConnection connection;


        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack> WithoutCheckConcurrency()
        {
            return new EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack>(connection);
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency> WithCheckConcurrency<TEntityConcurrency>(Expression<Func<TEntityCallBack, TEntityConcurrency>> EntityConcurrency)
        {
            if (EntityConcurrency == null)
                throw new EntityDatabaseException($"{typeof(EntityDatabaseUpdate<TEntityContext, TEntity, TEntityCallBack>).Name} Write, EntityConcurrency Argument Is Null");

            return new EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency>(connection);
        }
    }
}
