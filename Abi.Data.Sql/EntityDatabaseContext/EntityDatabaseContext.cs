using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseContextBuilder<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseContextBuilder(EntityDatabaseContext<TEntityContext> Context) => context = Context;

        private EntityDatabaseContext<TEntityContext> context;



        public EntityDatabaseContextInsertBuilder<TEntityContext, TEntity> Insert()
        {
            return new EntityDatabaseContextInsertBuilder<TEntityContext, TEntity>(context);
        }
        public EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity> Update()
        {
            return new EntityDatabaseContextUpdateBuilder<TEntityContext, TEntity>(context);
        }
        public EntityDatabaseContextDeleteBuilder<TEntityContext, TEntity> Delete()
        {
            return new EntityDatabaseContextDeleteBuilder<TEntityContext, TEntity>(context);
        }
    }








    public class EntityDatabaseContext<TEntityContext, TEntity> : EntityDatabaseContextAgent<TEntityContext> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseContext(EntityDatabaseContext<TEntityContext> Context) : base(Context) { }

        public EntityDatabaseContextEvent<TEntityContext> Event()
        {
            return new EntityDatabaseContextEvent<TEntityContext>(context);
        }
        public EntityDatabaseContextBuilder<TEntityContext, TOtherEntity> Table<TOtherEntity>() where TOtherEntity : Entity<TOtherEntity>
        {
            return new EntityDatabaseContextBuilder<TEntityContext, TOtherEntity>(context);
        }
    }








    public sealed class EntityDatabaseContext<TEntityContext> : IEntityDatabaseContext<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        static EntityDatabaseContext()
        {
            default_insert = new Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>>();
            default_update = new Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>>();
            default_delete = new Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>>();
        }
        internal EntityDatabaseContext(TEntityContext Context, EntityDatabaseConnection Connection) : this(Context, Connection, IsolationLevel.Unspecified)
        {
        }
        internal EntityDatabaseContext(TEntityContext Context, EntityDatabaseConnection Connection, IsolationLevel Isolation)
        {
            context = Context;
            isolation = Isolation;
            connection = Connection;

            insert = new Dictionary<Type, EntityDatabaseWriter>();
            update = new Dictionary<Type, EntityDatabaseWriter>();
            delete = new Dictionary<Type, EntityDatabaseWriter>();
        }

        private static Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>> default_insert;
        private static Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>> default_update;
        private static Dictionary<Type, Func<EntityDatabaseConnection, EntityDatabaseWriter>> default_delete;

        private TEntityContext context;
        private IsolationLevel isolation;
        internal EntityDatabaseConnection connection;

        internal Dictionary<Type, EntityDatabaseWriter> insert;
        internal Dictionary<Type, EntityDatabaseWriter> update;
        internal Dictionary<Type, EntityDatabaseWriter> delete;

        internal event EntityDatabaseContextEntitySavingEventHandler<TEntityContext> entity_saving;
        internal event EntityDatabaseContextEntitySavedEventHandler<TEntityContext> entity_saved;


        private void OnSaving(Entity entity)
        {
            entity_saving?.Invoke(this, new EntitySavingEventArgs<TEntityContext>(context, connection, entity));
        }
        private void OnSaved(Entity entity, EntityState state)
        {
            entity_saved?.Invoke(this, new EntitySavedEventArgs<TEntityContext>(context, connection, entity, state));
        }



        public EntityDatabaseContextEvent<TEntityContext> Event()
        {
            return new EntityDatabaseContextEvent<TEntityContext>(this);
        }
        public EntityDatabaseContextBuilder<TEntityContext, TEntity> Table<TEntity>() where TEntity : Entity<TEntity>
        {
            return new EntityDatabaseContextBuilder<TEntityContext, TEntity>(this);
        }

        public void Save(bool Batch = true, bool Snapshot = true)
        {
            if (context.ChangeTracker.Count == 0) return;

            context.FailOnLock();

            List<Entity> snapshot_of_entities = new List<Entity>();

            try
            {
                context.IsLocked = true;

                if (Snapshot)
                    foreach (Entity entity in context.ChangeTracker)
                    {
                        entity.TakeSnapshot();
                        snapshot_of_entities.Add(entity);
                    }

                if (Batch)
                    SaveBatchInternal();
                else
                    SaveInternal();

                if (Snapshot)
                {
                    foreach (Entity entity in snapshot_of_entities)
                        entity.RemoveSnapshot();

                    snapshot_of_entities.Clear();
                }

                context.IsLocked = false;

            }
            catch (Exception e)
            {
                if (Snapshot)
                {
                    foreach (Entity entity in snapshot_of_entities)
                        entity.RestoreSnapshot();

                    snapshot_of_entities.Clear();
                }

                context.IsLocked = false;

                if (isolation != IsolationLevel.Unspecified && connection.IsOpen())
                    connection.RollbackTransaction();

                throw e;
            }
        }
        private void SaveInternal()
        {
            bool isOpen = connection.IsOpen();

            if (!isOpen)
                connection.Open();

            if (isolation != IsolationLevel.Unspecified)
                connection.BeginTransaction(isolation);

            foreach (Entity entity in context.Changes.ToArray())
            {
                OnSaving(entity);

                EntityState state = entity.State;

                switch (state)
                {
                    case EntityState.Added:
                        if (!insert.TryGetValue(entity.EntityType, out EntityDatabaseWriter insert_writer))
                        {
                            if (!default_insert.TryGetValue(entity.EntityType, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_without_callback_insert_writer))
                                default_insert.Add(entity.EntityType, get_without_callback_insert_writer = generate_without_callback_insert_writer_creator(entity.EntityType));

                            insert.Add(entity.EntityType, insert_writer = get_without_callback_insert_writer(connection));
                        }

                        insert_writer.Write(entity);

                        break;

                    case EntityState.Modified:
                        if (!update.TryGetValue(entity.EntityType, out EntityDatabaseWriter update_writer))
                        {
                            if (!default_update.TryGetValue(entity.EntityType, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_without_callback_update_writer))
                                default_update.Add(entity.EntityType, get_without_callback_update_writer = generate_without_callback_update_writer_creator(entity.EntityType));

                            update.Add(entity.EntityType, update_writer = get_without_callback_update_writer(connection));
                        }

                        update_writer.Write(entity);

                        break;

                    case EntityState.Deleted:
                        if (!delete.TryGetValue(entity.EntityType, out EntityDatabaseWriter delete_writer))
                        {
                            if (!default_delete.TryGetValue(entity.EntityType, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_without_callback_delete_writer))
                                default_delete.Add(entity.EntityType, get_without_callback_delete_writer = generate_without_callback_delete_writer_creator(entity.EntityType));

                            delete.Add(entity.EntityType, delete_writer = get_without_callback_delete_writer(connection));
                        }

                        delete_writer.Write(entity);

                        break;

                    default:
                        throw new EntityDatabaseException($"{typeof(TEntityContext).Name} EntityDatabaseContext Save Failed, {entity} With {entity.State} State Is Not Supported");
                }

                OnSaved(entity, state);
            }

            if (isolation != IsolationLevel.Unspecified)
                connection.CommitTransaction();

            if (!isOpen)
                connection.Close();
        }
        private void SaveBatchInternal()
        {
            bool isOpen = connection.IsOpen();

            if (!isOpen)
                connection.Open();

            if (isolation != IsolationLevel.Unspecified)
                connection.BeginTransaction(isolation);

            Entity[] changed_entities = context.Changes.ToArray();

            SaveBatchCommand batch = new SaveBatchCommand();

            foreach (Entity entity in changed_entities)
            {
                EntityState state = entity.State;

                switch (state)
                {
                    case EntityState.Added:
                        if (!insert.TryGetValue(entity.EntityType, out EntityDatabaseWriter insert_writer))
                        {
                            if (!default_insert.TryGetValue(entity.EntityType, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_without_callback_insert_writer))
                                default_insert.Add(entity.EntityType, get_without_callback_insert_writer = generate_without_callback_insert_writer_creator(entity.EntityType));

                            insert.Add(entity.EntityType, insert_writer = get_without_callback_insert_writer(connection));
                        }

                        insert_writer.Append(entity, batch);

                        break;

                    case EntityState.Modified:
                        if (!update.TryGetValue(entity.EntityType, out EntityDatabaseWriter update_writer))
                        {
                            if (!default_update.TryGetValue(entity.EntityType, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_without_callback_update_writer))
                                default_update.Add(entity.EntityType, get_without_callback_update_writer = generate_without_callback_update_writer_creator(entity.EntityType));

                            update.Add(entity.EntityType, update_writer = get_without_callback_update_writer(connection));
                        }

                        update_writer.Append(entity, batch);

                        break;

                    case EntityState.Deleted:
                        if (!delete.TryGetValue(entity.EntityType, out EntityDatabaseWriter delete_writer))
                        {
                            if (!default_delete.TryGetValue(entity.EntityType, out Func<EntityDatabaseConnection, EntityDatabaseWriter> get_without_callback_delete_writer))
                                default_delete.Add(entity.EntityType, get_without_callback_delete_writer = generate_without_callback_delete_writer_creator(entity.EntityType));

                            delete.Add(entity.EntityType, delete_writer = get_without_callback_delete_writer(connection));
                        }

                        delete_writer.Append(entity, batch);

                        break;

                    default:
                        throw new EntityDatabaseException($"{typeof(TEntityContext).Name} EntityDatabaseContext Save Failed, {entity} With {entity.State} State Is Not Supported");
                }

            }

            using (SqlCommand command = connection.GetCommand(batch.GetCommandText()))
            {
                DatabaseTrace.Append(command.CommandText, Constant.BatchQuery);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    foreach (Entity entity in changed_entities)
                    {
                        EntityState state = entity.State;

                        switch (state)
                        {
                            case EntityState.Added:

                                EntityDatabaseWriter insert_writer = insert[entity.EntityType];

                                insert_writer.Write(entity, batch, reader);

                                break;

                            case EntityState.Modified:

                                EntityDatabaseWriter update_writer = update[entity.EntityType];

                                update_writer.Write(entity, batch, reader);

                                break;

                            case EntityState.Deleted:

                                EntityDatabaseWriter delete_writer = delete[entity.EntityType];

                                delete_writer.Write(entity, batch, reader);

                                break;

                            default:
                                throw new EntityDatabaseException($"{typeof(TEntityContext).Name} EntityDatabaseContext Save Failed, {entity} With {entity.State} State Is Not Supported");
                        }

                        try
                        {
                            // issue 1
                            // in case of occurrence concurrency in updating related entity (in case changing or removing foreign key done unsuccessfully)
                            // deleting foreign entity that dependent to that related entity will be failed, because of SqlException[Num: 547, Message: The DELETE statement conflicted with the REFERENCE constraint ...]
                            reader.NextResult();
                        }
                        catch(Exception e)
                        {
                            if (batch.HasException)
                                throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, Some Entities Had Already Changed Or Removed Before That", e, batch.Exceptions.ToArray());
                            else
                                throw e;
                        }
                    }

                    reader.Close();
                }
            }

            if (batch.HasException)
                throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, Some Entities Had Already Changed Or Removed Before That", batch.Exceptions.ToArray());

            if (isolation != IsolationLevel.Unspecified)
                connection.CommitTransaction();

            if (!isOpen)
                connection.Close();
        }



        private static Func<EntityDatabaseConnection, EntityDatabaseWriter> generate_without_callback_insert_writer_creator(Type typeEntity)
        {
            Type typeContext = typeof(TEntityContext);
            Type typeWriter = typeof(EntityDatabaseWriterInsert<,>).MakeGenericType(typeContext, typeEntity);

            ConstructorInfo ctrWriter = typeWriter.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(EntityDatabaseConnection) }, null);

            ParameterExpression argSqlCon = Expression.Parameter(typeof(EntityDatabaseConnection), "Connection");

            return Expression.Lambda<Func<EntityDatabaseConnection, EntityDatabaseWriter>>(Expression.New(ctrWriter, argSqlCon), argSqlCon).Compile();
        }
        private static Func<EntityDatabaseConnection, EntityDatabaseWriter> generate_without_callback_update_writer_creator(Type typeEntity)
        {
            Type typeContext = typeof(TEntityContext);
            Type typeWriter = typeof(EntityDatabaseWriterUpdate<,>).MakeGenericType(typeContext, typeEntity);

            ConstructorInfo ctrWriter = typeWriter.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(EntityDatabaseConnection) }, null);

            ParameterExpression argSqlCon = Expression.Parameter(typeof(EntityDatabaseConnection), "Connection");

            return Expression.Lambda<Func<EntityDatabaseConnection, EntityDatabaseWriter>>(Expression.New(ctrWriter, argSqlCon), argSqlCon).Compile();
        }
        private static Func<EntityDatabaseConnection, EntityDatabaseWriter> generate_without_callback_delete_writer_creator(Type typeEntity)
        {
            Type typeContext = typeof(TEntityContext);
            Type typeWriter = typeof(EntityDatabaseWriterDelete<,>).MakeGenericType(typeContext, typeEntity);

            ConstructorInfo ctrWriter = typeWriter.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(EntityDatabaseConnection) }, null);

            ParameterExpression argSqlCon = Expression.Parameter(typeof(EntityDatabaseConnection), "Connection");

            return Expression.Lambda<Func<EntityDatabaseConnection, EntityDatabaseWriter>>(Expression.New(ctrWriter, argSqlCon), argSqlCon).Compile();
        }
    }








    public interface IEntityDatabaseContext<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        void Save(bool Batch = true, bool Snapshot = true);
    }
}
