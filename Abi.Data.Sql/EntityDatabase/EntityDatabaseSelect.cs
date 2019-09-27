using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseSelect<TEntityContext, TEntity, TEntityCallBack> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseSelect(EntityDatabaseConnection Connection, string CommandText)
        {
            connection = Connection;
            commandText = CommandText;
        }


        private string commandText;
        private EntityDatabaseConnection connection;


        public void Fill(EntitySet<TEntity> EntitySet)
        {
            if (EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {EntitySet} Context Must Be Type Of {typeof(TEntityContext)}");

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    EntityDatabaseReaderSelect<TEntityContext, TEntity, TEntityCallBack>.Fill(reader, EntitySet);

                    reader.Close();
                }
            }
        }
        public void Merge(EntitySet<TEntity> EntitySet)
        {
            if (EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {EntitySet} Context Must Be Type Of {typeof(TEntityContext)}");

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    EntityDatabaseReaderSelect<TEntityContext, TEntity, TEntityCallBack>.Merge(reader, EntitySet);

                    reader.Close();
                }
            }
        }
        public void Refresh(EntitySet<TEntity> EntitySet)
        {
            if (EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {EntitySet} Context Must Be Type Of {typeof(TEntityContext)}");

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    EntityDatabaseReaderSelect<TEntityContext, TEntity, TEntityCallBack>.Refresh(reader, EntitySet);

                    reader.Close();
                }
            }
        }
    }








    public sealed class EntityDatabaseSelect<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal EntityDatabaseSelect(EntityDatabaseConnection Connection, string CommandText)
        {
            connection = Connection;
            commandText = CommandText;
        }


        private string commandText;
        private EntityDatabaseConnection connection;


        public void Fill(EntitySet<TEntity> EntitySet)
        {
            if (EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {EntitySet} Context Must Be Type Of {typeof(TEntityContext)}");

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    EntityDatabaseReaderSelect<TEntityContext, TEntity>.Fill(reader, EntitySet);

                    reader.Close();
                }
            }
        }
        public void Merge(EntitySet<TEntity> EntitySet)
        {
            if (EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {EntitySet} Context Must Be Type Of {typeof(TEntityContext)}");

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    EntityDatabaseReaderSelect<TEntityContext, TEntity>.Merge(reader, EntitySet);

                    reader.Close();
                }
            }
        }
        public void Refresh(EntitySet<TEntity> EntitySet)
        {
            if (EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {EntitySet} Context Must Be Type Of {typeof(TEntityContext)}");

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    EntityDatabaseReaderSelect<TEntityContext, TEntity>.Refresh(reader, EntitySet);

                    reader.Close();
                }
            }
        }


        public EntityDatabaseSelect<TEntityContext, TEntity, TEntityCallBack> WithCallBack<TEntityCallBack>(Expression<Func<TEntity, TEntityCallBack>> EntityCallBack)
        {
            if (EntityCallBack == null)
                throw new EntityDatabaseException($"{typeof(EntityDatabase<TEntityContext, TEntity>).Name} Write, EntityCallBack Argument Is Null");

            return new EntityDatabaseSelect<TEntityContext, TEntity, TEntityCallBack>(connection, commandText);
        }
    }
}
