using System;
using System.Linq;
using System.Data;
using System.Threading;
using System.Reflection;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseContextSelect<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        static EntityDatabaseContextSelect()
        {
            entitySets = new Dictionary<Type, Func<TEntityContext, EntitySet>>();

            without_callback_select = new Dictionary<Type, EntityDatabaseReader>();
            with_callback_select = new Dictionary<(Type, Type), EntityDatabaseReader>();

            fill = generate_fill();
            merge = generate_merge();
            refresh = generate_refresh();

            fill_async = generate_fill_async();
            merge_async = generate_merge_async();
            refresh_async = generate_refresh_async();
        }
        internal EntityDatabaseContextSelect(EntityDatabaseConnection Connection, IsolationLevel Isolation, string CommandText, TEntityContext Context)
        {
            context = Context;
            isolation = Isolation;
            connection = Connection;
            commandText = CommandText;

            selects = new List<(Type, EntityDatabaseReader, Select)>();
        }


        private static Dictionary<Type, Func<TEntityContext, EntitySet>> entitySets;

        private static Dictionary<Type, EntityDatabaseReader> without_callback_select;
        private static Dictionary<(Type, Type), EntityDatabaseReader> with_callback_select;

        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet> fill;
        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet> merge;
        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet> refresh;

        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet, CancellationToken> fill_async;
        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet, CancellationToken> merge_async;
        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet, CancellationToken> refresh_async;

        private string commandText;
        private TEntityContext context;
        private IsolationLevel isolation;
        private EntityDatabaseConnection connection;
        private List<(Type, EntityDatabaseReader, Select)> selects;


        public EntityDatabaseContextSelect<TEntityContext> Fill<TEntity>() where TEntity : Entity<TEntity>
        {
            Type typeEntity = typeof(TEntity);

            if (!without_callback_select.TryGetValue(typeEntity, out EntityDatabaseReader callback_select))
                without_callback_select.Add(typeEntity, callback_select = generate_without_callback_select_creator(typeEntity)());

            selects.Add((typeEntity, callback_select, Select.Fill));

            return this;
        }
        public EntityDatabaseContextSelect<TEntityContext> Merge<TEntity>() where TEntity : Entity<TEntity>
        {
            Type typeEntity = typeof(TEntity);

            if (!without_callback_select.TryGetValue(typeEntity, out EntityDatabaseReader callback_select))
                without_callback_select.Add(typeEntity, callback_select = generate_without_callback_select_creator(typeEntity)());

            selects.Add((typeEntity, callback_select, Select.Merge));

            return this;
        }
        public EntityDatabaseContextSelect<TEntityContext> Refresh<TEntity>() where TEntity : Entity<TEntity>
        {
            Type typeEntity = typeof(TEntity);

            if (!without_callback_select.TryGetValue(typeEntity, out EntityDatabaseReader callback_select))
                without_callback_select.Add(typeEntity, callback_select = generate_without_callback_select_creator(typeEntity)());

            selects.Add((typeEntity, callback_select, Select.Refresh));

            return this;
        }


        public EntityDatabaseContextSelect<TEntityContext> Fill<TEntity>(Expression<Func<TEntity, object>> EntityCallBack) where TEntity : Entity<TEntity>
        {
            Type typeEntity = typeof(TEntity);
            Type typeEntityCallBack = ((NewExpression)EntityCallBack.Body).Type;

            if (!with_callback_select.TryGetValue((typeEntity, typeEntityCallBack), out EntityDatabaseReader callback_select))
                with_callback_select.Add((typeEntity, typeEntityCallBack), callback_select = generate_with_callback_select_creator(typeEntity, typeEntityCallBack)());

            selects.Add((typeEntity, callback_select, Select.Fill));

            return this;
        }
        public EntityDatabaseContextSelect<TEntityContext> Merge<TEntity>(Expression<Func<TEntity, object>> EntityCallBack) where TEntity : Entity<TEntity>
        {
            Type typeEntity = typeof(TEntity);
            Type typeEntityCallBack = ((NewExpression)EntityCallBack.Body).Type;

            if (!with_callback_select.TryGetValue((typeEntity, typeEntityCallBack), out EntityDatabaseReader callback_select))
                with_callback_select.Add((typeEntity, typeEntityCallBack), callback_select = generate_with_callback_select_creator(typeEntity, typeEntityCallBack)());

            selects.Add((typeEntity, callback_select, Select.Merge));

            return this;
        }
        public EntityDatabaseContextSelect<TEntityContext> Refresh<TEntity>(Expression<Func<TEntity, object>> EntityCallBack) where TEntity : Entity<TEntity>
        {
            Type typeEntity = typeof(TEntity);
            Type typeEntityCallBack = ((NewExpression)EntityCallBack.Body).Type;

            if (!with_callback_select.TryGetValue((typeEntity, typeEntityCallBack), out EntityDatabaseReader callback_select))
                with_callback_select.Add((typeEntity, typeEntityCallBack), callback_select = generate_with_callback_select_creator(typeEntity, typeEntityCallBack)());

            selects.Add((typeEntity, callback_select, Select.Refresh));

            return this;
        }


        public void Execute()
        {
            bool isOpen = connection.IsOpen();

            if (!isOpen)
                connection.Open();

            if (isolation != IsolationLevel.Unspecified)
                connection.BeginTransaction(isolation);

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    foreach ((Type typeEntity, EntityDatabaseReader entity_reader, Select type) select in selects)
                    {
                        if (!entitySets.TryGetValue(select.typeEntity, out Func<TEntityContext, EntitySet> get_entityset))
                            entitySets.Add(select.typeEntity, get_entityset = generate_get_entityset(select.typeEntity));

                        EntitySet entitySet = get_entityset(context);

                        switch (select.type)
                        {
                            case Select.Fill:
                                fill(select.entity_reader, reader, entitySet);
                                break;
                            case Select.Merge:
                                merge(select.entity_reader, reader, entitySet);
                                break;
                            case Select.Refresh:
                                refresh(select.entity_reader, reader, entitySet);
                                break;
                        }

                        reader.NextResult();
                    }

                    reader.Close();
                }
            }

            if (connection.HasTransaction)
                connection.CommitTransaction();

            if (!isOpen)
                connection.Close();
        }
        public Task ExecuteAsync()
        {
            return ExecuteAsync(CancellationToken.None);
        }
        public Task ExecuteAsync(CancellationToken CancellationToken)
        {
            bool isOpen = connection.IsOpen();

            if (!isOpen)
                connection.OpenAsync(CancellationToken).Wait();

            if (isolation != IsolationLevel.Unspecified)
                connection.BeginTransaction(isolation);

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                return command.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken)
                              .ContinueWith(task =>
                              {
                                  using (SqlDataReader reader = task.Result)
                                  {
                                      foreach ((Type typeEntity, EntityDatabaseReader entity_reader, Select type) select in selects)
                                      {
                                          if (!entitySets.TryGetValue(select.typeEntity, out Func<TEntityContext, EntitySet> get_entityset))
                                              entitySets.Add(select.typeEntity, get_entityset = generate_get_entityset(select.typeEntity));

                                          EntitySet entitySet = get_entityset(context);

                                          switch (select.type)
                                          {
                                              case Select.Fill:
                                                  fill_async(select.entity_reader, reader, entitySet, CancellationToken);
                                                  break;
                                              case Select.Merge:
                                                  merge_async(select.entity_reader, reader, entitySet, CancellationToken);
                                                  break;
                                              case Select.Refresh:
                                                  refresh_async(select.entity_reader, reader, entitySet, CancellationToken);
                                                  break;
                                          }

                                          reader.NextResultAsync(CancellationToken).Wait();
                                      }

                                      reader.Close();
                                  }

                                  if (connection.HasTransaction)
                                      connection.CommitTransaction();

                                  if (!isOpen)
                                      connection.Close();
                              });

            }
        }


        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet> generate_fill()
        {
            ParameterExpression edr = Expression.Parameter(typeof(EntityDatabaseReader), "EntityDatabaseReader");
            ParameterExpression sdr = Expression.Parameter(typeof(SqlDataReader), "SqlDataReader");
            ParameterExpression es = Expression.Parameter(typeof(EntitySet), "EntitySet");

            MethodInfo mthdFill = typeof(EntityDatabaseReader).GetMethod("Fill", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(SqlDataReader), typeof(EntitySet) }, null);

            return Expression.Lambda<Action<EntityDatabaseReader, SqlDataReader, EntitySet>>(Expression.Call(edr, mthdFill, sdr, es), edr, sdr, es).Compile();
        }
        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet> generate_merge()
        {
            ParameterExpression edr = Expression.Parameter(typeof(EntityDatabaseReader), "EntityDatabaseReader");
            ParameterExpression sdr = Expression.Parameter(typeof(SqlDataReader), "SqlDataReader");
            ParameterExpression es = Expression.Parameter(typeof(EntitySet), "EntitySet");

            MethodInfo mthdMerge = typeof(EntityDatabaseReader).GetMethod("Merge", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(SqlDataReader), typeof(EntitySet) }, null);

            return Expression.Lambda<Action<EntityDatabaseReader, SqlDataReader, EntitySet>>(Expression.Call(edr, mthdMerge, sdr, es), edr, sdr, es).Compile();
        }
        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet> generate_refresh()
        {
            ParameterExpression edr = Expression.Parameter(typeof(EntityDatabaseReader), "EntityDatabaseReader");
            ParameterExpression sdr = Expression.Parameter(typeof(SqlDataReader), "SqlDataReader");
            ParameterExpression es = Expression.Parameter(typeof(EntitySet), "EntitySet");

            MethodInfo mthdRefresh = typeof(EntityDatabaseReader).GetMethod("Refresh", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(SqlDataReader), typeof(EntitySet) }, null);

            return Expression.Lambda<Action<EntityDatabaseReader, SqlDataReader, EntitySet>>(Expression.Call(edr, mthdRefresh, sdr, es), edr, sdr, es).Compile();
        }

        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet, CancellationToken> generate_fill_async()
        {
            ParameterExpression edr = Expression.Parameter(typeof(EntityDatabaseReader), "EntityDatabaseReader");
            ParameterExpression sdr = Expression.Parameter(typeof(SqlDataReader), "SqlDataReader");
            ParameterExpression es = Expression.Parameter(typeof(EntitySet), "EntitySet");
            ParameterExpression ct = Expression.Parameter(typeof(CancellationToken), "CancellationToken");

            MethodInfo mthdFill = typeof(EntityDatabaseReader).GetMethod("FillAsync", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(SqlDataReader), typeof(EntitySet), typeof(CancellationToken) }, null);

            return Expression.Lambda<Action<EntityDatabaseReader, SqlDataReader, EntitySet, CancellationToken>>(Expression.Call(edr, mthdFill, sdr, es, ct), edr, sdr, es, ct).Compile();
        }
        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet, CancellationToken> generate_merge_async()
        {
            ParameterExpression edr = Expression.Parameter(typeof(EntityDatabaseReader), "EntityDatabaseReader");
            ParameterExpression sdr = Expression.Parameter(typeof(SqlDataReader), "SqlDataReader");
            ParameterExpression es = Expression.Parameter(typeof(EntitySet), "EntitySet");
            ParameterExpression ct = Expression.Parameter(typeof(CancellationToken), "CancellationToken");

            MethodInfo mthdMerge = typeof(EntityDatabaseReader).GetMethod("MergeAsync", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(SqlDataReader), typeof(EntitySet), typeof(CancellationToken) }, null);

            return Expression.Lambda<Action<EntityDatabaseReader, SqlDataReader, EntitySet, CancellationToken>>(Expression.Call(edr, mthdMerge, sdr, es, ct), edr, sdr, es, ct).Compile();
        }
        private static Action<EntityDatabaseReader, SqlDataReader, EntitySet, CancellationToken> generate_refresh_async()
        {
            ParameterExpression edr = Expression.Parameter(typeof(EntityDatabaseReader), "EntityDatabaseReader");
            ParameterExpression sdr = Expression.Parameter(typeof(SqlDataReader), "SqlDataReader");
            ParameterExpression es = Expression.Parameter(typeof(EntitySet), "EntitySet");
            ParameterExpression ct = Expression.Parameter(typeof(CancellationToken), "CancellationToken");

            MethodInfo mthdRefresh = typeof(EntityDatabaseReader).GetMethod("RefreshAsync", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(SqlDataReader), typeof(EntitySet), typeof(CancellationToken) }, null);

            return Expression.Lambda<Action<EntityDatabaseReader, SqlDataReader, EntitySet, CancellationToken>>(Expression.Call(edr, mthdRefresh, sdr, es, ct), edr, sdr, es, ct).Compile();
        }

        private static Func<EntityDatabaseReader> generate_without_callback_select_creator(Type typeEntity)
        {
            Type typeContext = typeof(TEntityContext);
            Type typeReader = typeof(EntityDatabaseReaderSelect<,>).MakeGenericType(typeContext, typeEntity);

            ConstructorInfo ctrReader = typeReader.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);

            return Expression.Lambda<Func<EntityDatabaseReader>>(Expression.New(ctrReader)).Compile();
        }
        private static Func<EntityDatabaseReader> generate_with_callback_select_creator(Type typeEntity, Type typeEntityCallBack)
        {
            Type typeContext = typeof(TEntityContext);
            Type typeReader = typeof(EntityDatabaseReaderSelect<,,>).MakeGenericType(typeContext, typeEntity, typeEntityCallBack);

            ConstructorInfo ctrReader = typeReader.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);

            return Expression.Lambda<Func<EntityDatabaseReader>>(Expression.New(ctrReader)).Compile();
        }

        private static Func<TEntityContext, EntitySet> generate_get_entityset(Type typeEntity)
        {
            Type typeContext = typeof(TEntityContext);

            PropertyInfo propEntitySet = typeContext.GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(EntitySet))
                                                                             && p.PropertyType.GenericTypeArguments[0] == typeEntity).First();

            ParameterExpression expContext = Expression.Parameter(typeContext, "Context");

            return Expression.Lambda<Func<TEntityContext, EntitySet>>(Expression.Call(expContext, propEntitySet.GetGetMethod()), expContext).Compile();
        }
    }
}
