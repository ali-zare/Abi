using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseContextDelete<TEntityContext, TEntity, TEntityConcurrency> : EntityDatabaseContext<TEntityContext, TEntity>, IEntityDatabaseContext<TEntityContext> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseContextDelete(EntityDatabaseContext<TEntityContext> Context) : base(Context) { }


        public EntityDatabaseContextInsertBuilder<TEntityContext, TEntity> Insert()
        {
            return new EntityDatabaseContextInsertBuilder<TEntityContext, TEntity>(context);
        }
        public EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity> Update()
        {
            return new EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity>(context);
        }
    }








    public sealed class EntityDatabaseContextDeleteBuilder<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseContextDeleteBuilder()
        {
            with_check_concurrency_delete_writers = new Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>>();
        }
        internal EntityDatabaseContextDeleteBuilder(EntityDatabaseContext<TEntityContext> Context) => context = Context;



        private EntityDatabaseContext<TEntityContext> context;

        private static Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>> with_check_concurrency_delete_writers;



        public EntityDatabaseContextDelete<TEntityContext, TEntity, TEntityConcurrency> WithCheckConcurrency<TEntityConcurrency>(Expression<Func<TEntity, TEntityConcurrency>> EntityConcurrency)
        {
            Type typeEntity = typeof(TEntity);
            Type typeEntityConcurrency = typeof(TEntityConcurrency);

            if (!with_check_concurrency_delete_writers.TryGetValue(typeEntityConcurrency, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_with_check_concurrency_delete_writer))
                with_check_concurrency_delete_writers.Add(typeEntityConcurrency, get_with_check_concurrency_delete_writer = generate_with_check_concurrency_delete_writer_creator(typeEntity, typeEntityConcurrency));

            context.delete.Add(typeEntity, get_with_check_concurrency_delete_writer(context.connection));

            return new EntityDatabaseContextDelete<TEntityContext, TEntity, TEntityConcurrency>(context);
        }

        private static Func<EntityDatabaseConnection, EntityDatabaseWriter> generate_with_check_concurrency_delete_writer_creator(Type typeEntity, Type typeEntityConcurrency)
        {
            Type typeContext = typeof(TEntityContext);
            Type typeWriter = typeof(EntityDatabaseWriterDelete<,,>).MakeGenericType(typeContext, typeEntity, typeEntityConcurrency);

            ConstructorInfo ctrWriter = typeWriter.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(EntityDatabaseConnection) }, null);

            ParameterExpression argSqlCon = Expression.Parameter(typeof(EntityDatabaseConnection), "Connection");

            return Expression.Lambda<Func<EntityDatabaseConnection, EntityDatabaseWriter>>(Expression.New(ctrWriter, argSqlCon), argSqlCon).Compile();
        }
    }
}