using System;
using System.Linq.Expressions;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseInsert<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseInsert(EntityDatabaseConnection Connection) => connection = Connection;


        private EntityDatabaseConnection connection;


        public EntityDatabaseWriterInsert<TEntityContext, TEntity> WithoutCallBack()
        {
            return new EntityDatabaseWriterInsert<TEntityContext, TEntity>(connection);
        }
        public EntityDatabaseWriterInsert<TEntityContext, TEntity, TEntityCallBack> WithCallBack<TEntityCallBack>(Expression<Func<TEntity, TEntityCallBack>> EntityCallBack)
        {
            if (EntityCallBack == null)
                throw new EntityDatabaseException($"{typeof(EntityDatabaseInsert<TEntityContext, TEntity>).Name} Write, EntityCallBack Argument Is Null");

            return new EntityDatabaseWriterInsert<TEntityContext, TEntity, TEntityCallBack>(connection);
        }
    }
}
