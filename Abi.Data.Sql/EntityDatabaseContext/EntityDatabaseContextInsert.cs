using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public class EntityDatabaseContextInsert<TEntityContext, TEntity, TEntityCallBack> : EntityDatabaseContext<TEntityContext, TEntity>, IEntityDatabaseContext<TEntityContext> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseContextInsert(EntityDatabaseContext<TEntityContext> Context) : base(Context) { }


        public EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity> Update()
        {
            return new EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity>(context);
        }
        public EntityDatabaseContextDeleteBuilder<TEntityContext, TEntity> Delete()
        {
            return new EntityDatabaseContextDeleteBuilder<TEntityContext, TEntity>(context);
        }
    }








    public sealed class EntityDatabaseContextInsertBuilder<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseContextInsertBuilder()
        {
            with_callBack_insert_writers = new Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>>();
        }
        internal EntityDatabaseContextInsertBuilder(EntityDatabaseContext<TEntityContext> Context) => context = Context;



        private EntityDatabaseContext<TEntityContext> context;

        private static Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>> with_callBack_insert_writers;



        public EntityDatabaseContextInsert<TEntityContext, TEntity, TEntityCallBack> WithCallBack<TEntityCallBack>(Expression<Func<TEntity, TEntityCallBack>> EntityCallBack)
        {
            Type typeEntity = typeof(TEntity);
            Type typeEntityCallBack = typeof(TEntityCallBack);

            if (!with_callBack_insert_writers.TryGetValue(typeEntityCallBack, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_with_callBack_insert_writer))
                with_callBack_insert_writers.Add(typeEntityCallBack, get_with_callBack_insert_writer = generate_with_callback_insert_writer_creator(typeEntity, typeEntityCallBack));

            context.insert.Add(typeEntity, get_with_callBack_insert_writer(context.connection));

            return new EntityDatabaseContextInsert<TEntityContext, TEntity, TEntityCallBack>(context);
        }

        private static Func<EntityDatabaseConnection, EntityDatabaseWriter> generate_with_callback_insert_writer_creator(Type typeEntity, Type typeEntityCallBack)
        {
            Type typeContext = typeof(TEntityContext);
            Type typeWriter = typeof(EntityDatabaseWriterInsert<,,>).MakeGenericType(typeContext, typeEntity, typeEntityCallBack);

            ConstructorInfo ctrWriter = typeWriter.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(EntityDatabaseConnection) }, null);

            ParameterExpression argSqlCon = Expression.Parameter(typeof(EntityDatabaseConnection), "Connection");

            return Expression.Lambda<Func<EntityDatabaseConnection, EntityDatabaseWriter>>(Expression.New(ctrWriter, argSqlCon), argSqlCon).Compile();
        }
    }
}