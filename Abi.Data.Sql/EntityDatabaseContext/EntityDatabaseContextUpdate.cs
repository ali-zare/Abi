using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public class EntityDatabaseContextUpdate<TEntityContext, TEntity, TEntityCallBack> : EntityDatabaseContext<TEntityContext, TEntity>, IEntityDatabaseContext<TEntityContext> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseContextUpdate(EntityDatabaseContext<TEntityContext> Context) : base(Context) { }


        public EntityDatabaseContextInsertBuilder<TEntityContext, TEntity> Insert()
        {
            return new EntityDatabaseContextInsertBuilder<TEntityContext, TEntity>(context);
        }
        public EntityDatabaseContextDeleteBuilder<TEntityContext, TEntity> Delete()
        {
            return new EntityDatabaseContextDeleteBuilder<TEntityContext, TEntity>(context);
        }
    }

    public class EntityDatabaseContextUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency> : EntityDatabaseContextUpdate<TEntityContext, TEntity, TEntityCallBack>, IEntityDatabaseContext<TEntityContext> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseContextUpdate(EntityDatabaseContext<TEntityContext> Context) : base(Context) { }
    }








    public sealed class EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity, TEntityCallBack> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseContextUpdateBuilder()
        {
            with_check_concurrency_update_writers = new Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>>();
        }
        internal EntityDatabaseContextUpdateBuilder(EntityDatabaseContext<TEntityContext> Context) => context = Context;



        private EntityDatabaseContext<TEntityContext> context;

        private static Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>> with_check_concurrency_update_writers;



        public EntityDatabaseContextUpdate<TEntityContext, TEntity, TEntityCallBack> WithoutCheckConcurrency()
        {
            EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity>.SetEntityContext<TEntityCallBack>(context);

            return new EntityDatabaseContextUpdate<TEntityContext, TEntity, TEntityCallBack>(context);
        }
        public EntityDatabaseContextUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency> WithCheckConcurrency<TEntityConcurrency>(Expression<Func<TEntityCallBack, TEntityConcurrency>> EntityConcurrency)
        {
            Type typeEntity = typeof(TEntity);
            Type typeEntityCallBack = typeof(TEntityCallBack);
            Type typeEntityConcurrency = typeof(TEntityConcurrency);

            if (!with_check_concurrency_update_writers.TryGetValue(typeEntityConcurrency, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_with_check_concurrency_update_writer))
                with_check_concurrency_update_writers.Add(typeEntityConcurrency, get_with_check_concurrency_update_writer = generate_with_check_concurrency_update_writer_creator(typeEntity, typeEntityCallBack, typeEntityConcurrency));

            context.update.Add(typeEntity, get_with_check_concurrency_update_writer(context.connection));

            return new EntityDatabaseContextUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency>(context);
        }

        private static Func<EntityDatabaseConnection, EntityDatabaseWriter> generate_with_check_concurrency_update_writer_creator(Type typeEntity, Type typeEntityCallBack, Type typeEntityConcurrency)
        {
            Type typeContext = typeof(TEntityContext);
            Type typeWriter = typeof(EntityDatabaseWriterUpdate<,,,>).MakeGenericType(typeContext, typeEntity, typeEntityCallBack, typeEntityConcurrency);

            ConstructorInfo ctrWriter = typeWriter.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(EntityDatabaseConnection) }, null);

            ParameterExpression argSqlCon = Expression.Parameter(typeof(EntityDatabaseConnection), "Connection");

            return Expression.Lambda<Func<EntityDatabaseConnection, EntityDatabaseWriter>>(Expression.New(ctrWriter, argSqlCon), argSqlCon).Compile();
        }
    }








    public sealed class EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseContextUpdateBuilder()
        {
            with_callBack_update_writers = new Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>>();
        }
        internal EntityDatabaseContextUpdateBuilder(EntityDatabaseContext<TEntityContext> Context) => context = Context;



        private EntityDatabaseContext<TEntityContext> context;

        private static Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>> with_callBack_update_writers;



        public EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity, TEntityCallBack> WithCallBack<TEntityCallBack>(Expression<Func<TEntity, TEntityCallBack>> EntityCallBack)
        {
            // code moved to SetEntityContext

            return new EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity, TEntityCallBack>(context);
        }

        private static Func<EntityDatabaseConnection, EntityDatabaseWriter> generate_with_callback_update_writer_creator(Type typeEntity, Type typeEntityCallBack)
        {
            Type typeContext = typeof(TEntityContext);
            Type typeWriter = typeof(EntityDatabaseWriterUpdate<,,>).MakeGenericType(typeContext, typeEntity, typeEntityCallBack);

            ConstructorInfo ctrWriter = typeWriter.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(EntityDatabaseConnection) }, null);

            ParameterExpression argSqlCon = Expression.Parameter(typeof(EntityDatabaseConnection), "Connection");

            return Expression.Lambda<Func<EntityDatabaseConnection, EntityDatabaseWriter>>(Expression.New(ctrWriter, argSqlCon), argSqlCon).Compile();
        }



        internal static void SetEntityContext<TEntityCallBack>(EntityDatabaseContext<TEntityContext> context)
        {
            Type typeEntity = typeof(TEntity);
            Type typeEntityCallBack = typeof(TEntityCallBack);

            if (!with_callBack_update_writers.TryGetValue(typeEntityCallBack, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_with_callBack_update_writer))
                with_callBack_update_writers.Add(typeEntityCallBack, get_with_callBack_update_writer = generate_with_callback_update_writer_creator(typeEntity, typeEntityCallBack));

            context.update.Add(typeEntity, get_with_callBack_update_writer(context.connection));
        }
    }
}