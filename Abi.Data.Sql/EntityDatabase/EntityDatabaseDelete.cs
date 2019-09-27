using System;
using System.Linq.Expressions;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseDelete<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseDelete(EntityDatabaseConnection Connection) => connection = Connection;


        private EntityDatabaseConnection connection;


        public EntityDatabaseWriterDelete<TEntityContext, TEntity> WithoutCheckConcurrency()
        {
            return new EntityDatabaseWriterDelete<TEntityContext, TEntity>(connection);
        }
        public EntityDatabaseWriterDelete<TEntityContext, TEntity, TEntityConcurrency> WithCheckConcurrency<TEntityConcurrency>(Expression<Func<TEntity, TEntityConcurrency>> EntityConcurrency)
        {
            if (EntityConcurrency == null)
                throw new EntityDatabaseException($"{typeof(EntityDatabaseDelete<TEntityContext, TEntity>).Name} Write, EntityConcurrency Argument Is Null");

            return new EntityDatabaseWriterDelete<TEntityContext, TEntity, TEntityConcurrency>(connection);
        }
    }
}
