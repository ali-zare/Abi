using System;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    internal sealed class EntityDatabaseReaderSelect<TEntityContext, TEntity, TEntityCallBack> : EntityDatabaseReader where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseReaderSelect()
        {
            fill_setters = new Dictionary<CommandToken<TEntityContext, TEntity, TEntityCallBack>, Action<SqlDataReader, EntitySet<TEntity>>>();
            merge_setters = new Dictionary<CommandToken<TEntityContext, TEntity, TEntityCallBack>, Action<SqlDataReader, EntitySet<TEntity>>>();
            refresh_setters = new Dictionary<CommandToken<TEntityContext, TEntity, TEntityCallBack>, Action<SqlDataReader, EntitySet<TEntity>>>();
        }
        internal EntityDatabaseReaderSelect() { }


        private static Dictionary<CommandToken<TEntityContext, TEntity, TEntityCallBack>, Action<SqlDataReader, EntitySet<TEntity>>> fill_setters;
        private static Dictionary<CommandToken<TEntityContext, TEntity, TEntityCallBack>, Action<SqlDataReader, EntitySet<TEntity>>> merge_setters;
        private static Dictionary<CommandToken<TEntityContext, TEntity, TEntityCallBack>, Action<SqlDataReader, EntitySet<TEntity>>> refresh_setters;


        internal override void Fill(SqlDataReader Reader, EntitySet EntitySet)
        {
            Fill(Reader, (EntitySet<TEntity>)EntitySet);
        }
        internal override void Merge(SqlDataReader Reader, EntitySet EntitySet)
        {
            Merge(Reader, (EntitySet<TEntity>)EntitySet);
        }
        internal override void Refresh(SqlDataReader Reader, EntitySet EntitySet)
        {
            Refresh(Reader, (EntitySet<TEntity>)EntitySet);
        }

        internal override void FillAsync(SqlDataReader Reader, EntitySet EntitySet, CancellationToken CancellationToken)
        {
            FillAsync(Reader, (EntitySet<TEntity>)EntitySet, CancellationToken);
        }
        internal override void MergeAsync(SqlDataReader Reader, EntitySet EntitySet, CancellationToken CancellationToken)
        {
            MergeAsync(Reader, (EntitySet<TEntity>)EntitySet, CancellationToken);
        }
        internal override void RefreshAsync(SqlDataReader Reader, EntitySet EntitySet, CancellationToken CancellationToken)
        {
            RefreshAsync(Reader, (EntitySet<TEntity>)EntitySet, CancellationToken);
        }




        internal static void Fill(SqlDataReader Reader, EntitySet<TEntity> EntitySet)
        {
            CommandToken<TEntityContext, TEntity, TEntityCallBack> token = new CommandToken<TEntityContext, TEntity, TEntityCallBack>(Reader.GetSchemaTable());

            if (!fill_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> fill))
                fill_setters.Add(token, fill = token.Generate_Fill_Setter());

            while (Reader.Read())
                fill(Reader, EntitySet);
        }
        internal static void Merge(SqlDataReader Reader, EntitySet<TEntity> EntitySet)
        {
            CommandToken<TEntityContext, TEntity, TEntityCallBack> token = new CommandToken<TEntityContext, TEntity, TEntityCallBack>(Reader.GetSchemaTable());

            if (!merge_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> merge))
                merge_setters.Add(token, merge = token.Generate_Merge_Setter());

            while (Reader.Read())
                merge(Reader, EntitySet);
        }
        internal static void Refresh(SqlDataReader Reader, EntitySet<TEntity> EntitySet)
        {
            CommandToken<TEntityContext, TEntity, TEntityCallBack> token = new CommandToken<TEntityContext, TEntity, TEntityCallBack>(Reader.GetSchemaTable());

            if (!refresh_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> refresh))
                refresh_setters.Add(token, refresh = token.Generate_Refresh_Setter());

            while (Reader.Read())
                refresh(Reader, EntitySet);
        }

        internal static void FillAsync(SqlDataReader Reader, EntitySet<TEntity> EntitySet, CancellationToken CancellationToken)
        {
            CommandToken<TEntityContext, TEntity, TEntityCallBack> token = new CommandToken<TEntityContext, TEntity, TEntityCallBack>(Reader.GetSchemaTable());

            if (!fill_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> fill))
                fill_setters.Add(token, fill = token.Generate_Fill_Setter());

            while (Reader.ReadAsync(CancellationToken).Result)
                fill(Reader, EntitySet);
        }
        internal static void MergeAsync(SqlDataReader Reader, EntitySet<TEntity> EntitySet, CancellationToken CancellationToken)
        {
            CommandToken<TEntityContext, TEntity, TEntityCallBack> token = new CommandToken<TEntityContext, TEntity, TEntityCallBack>(Reader.GetSchemaTable());

            if (!merge_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> merge))
                merge_setters.Add(token, merge = token.Generate_Merge_Setter());

            while (Reader.ReadAsync(CancellationToken).Result)
                merge(Reader, EntitySet);
        }
        internal static void RefreshAsync(SqlDataReader Reader, EntitySet<TEntity> EntitySet, CancellationToken CancellationToken)
        {
            CommandToken<TEntityContext, TEntity, TEntityCallBack> token = new CommandToken<TEntityContext, TEntity, TEntityCallBack>(Reader.GetSchemaTable());

            if (!refresh_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> refresh))
                refresh_setters.Add(token, refresh = token.Generate_Refresh_Setter());

            while (Reader.ReadAsync(CancellationToken).Result)
                refresh(Reader, EntitySet);
        }




        public override string ToString()
        {
            return $"{typeof(TEntityContext).Name} Context, {typeof(TEntity).Name} Entity Database Reader Select";
        }
    }








    internal sealed class EntityDatabaseReaderSelect<TEntityContext, TEntity> : EntityDatabaseReader where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseReaderSelect()
        {
            fill_setters = new Dictionary<CommandToken<TEntityContext, TEntity>, Action<SqlDataReader, EntitySet<TEntity>>>();
            merge_setters = new Dictionary<CommandToken<TEntityContext, TEntity>, Action<SqlDataReader, EntitySet<TEntity>>>();
            refresh_setters = new Dictionary<CommandToken<TEntityContext, TEntity>, Action<SqlDataReader, EntitySet<TEntity>>>();
        }
        internal EntityDatabaseReaderSelect() { }


        private static Dictionary<CommandToken<TEntityContext, TEntity>, Action<SqlDataReader, EntitySet<TEntity>>> fill_setters;
        private static Dictionary<CommandToken<TEntityContext, TEntity>, Action<SqlDataReader, EntitySet<TEntity>>> merge_setters;
        private static Dictionary<CommandToken<TEntityContext, TEntity>, Action<SqlDataReader, EntitySet<TEntity>>> refresh_setters;


        internal override void Fill(SqlDataReader Reader, EntitySet EntitySet)
        {
            Fill(Reader, (EntitySet<TEntity>)EntitySet);
        }
        internal override void Merge(SqlDataReader Reader, EntitySet EntitySet)
        {
            Merge(Reader, (EntitySet<TEntity>)EntitySet);
        }
        internal override void Refresh(SqlDataReader Reader, EntitySet EntitySet)
        {
            Refresh(Reader, (EntitySet<TEntity>)EntitySet);
        }

        internal override void FillAsync(SqlDataReader Reader, EntitySet EntitySet, CancellationToken CancellationToken)
        {
            FillAsync(Reader, (EntitySet<TEntity>)EntitySet, CancellationToken);
        }
        internal override void MergeAsync(SqlDataReader Reader, EntitySet EntitySet, CancellationToken CancellationToken)
        {
            MergeAsync(Reader, (EntitySet<TEntity>)EntitySet, CancellationToken);
        }
        internal override void RefreshAsync(SqlDataReader Reader, EntitySet EntitySet, CancellationToken CancellationToken)
        {
            RefreshAsync(Reader, (EntitySet<TEntity>)EntitySet, CancellationToken);
        }




        internal static void Fill(SqlDataReader Reader, EntitySet<TEntity> EntitySet)
        {
            CommandToken<TEntityContext, TEntity> token = new CommandToken<TEntityContext, TEntity>(Reader.GetSchemaTable());

            if (!fill_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> fill))
                fill_setters.Add(token, fill = token.Generate_Fill_Setter());

            while (Reader.Read())
                fill(Reader, EntitySet);
        }
        internal static void Merge(SqlDataReader Reader, EntitySet<TEntity> EntitySet)
        {
            CommandToken<TEntityContext, TEntity> token = new CommandToken<TEntityContext, TEntity>(Reader.GetSchemaTable());

            if (!merge_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> merge))
                merge_setters.Add(token, merge = token.Generate_Merge_Setter());

            while (Reader.Read())
                merge(Reader, EntitySet);
        }
        internal static void Refresh(SqlDataReader Reader, EntitySet<TEntity> EntitySet)
        {
            CommandToken<TEntityContext, TEntity> token = new CommandToken<TEntityContext, TEntity>(Reader.GetSchemaTable());

            if (!merge_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> refresh))
                refresh_setters.Add(token, refresh = token.Generate_Refresh_Setter());

            while (Reader.Read())
                refresh(Reader, EntitySet);
        }

        internal static void FillAsync(SqlDataReader Reader, EntitySet<TEntity> EntitySet, CancellationToken CancellationToken)
        {
            CommandToken<TEntityContext, TEntity> token = new CommandToken<TEntityContext, TEntity>(Reader.GetSchemaTable());

            if (!fill_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> fill))
                fill_setters.Add(token, fill = token.Generate_Fill_Setter());

            while (Reader.ReadAsync(CancellationToken).Result)
                fill(Reader, EntitySet);
        }
        internal static void MergeAsync(SqlDataReader Reader, EntitySet<TEntity> EntitySet, CancellationToken CancellationToken)
        {
            CommandToken<TEntityContext, TEntity> token = new CommandToken<TEntityContext, TEntity>(Reader.GetSchemaTable());

            if (!merge_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> merge))
                merge_setters.Add(token, merge = token.Generate_Merge_Setter());

            while (Reader.ReadAsync(CancellationToken).Result)
                merge(Reader, EntitySet);
        }
        internal static void RefreshAsync(SqlDataReader Reader, EntitySet<TEntity> EntitySet, CancellationToken CancellationToken)
        {
            CommandToken<TEntityContext, TEntity> token = new CommandToken<TEntityContext, TEntity>(Reader.GetSchemaTable());

            if (!merge_setters.TryGetValue(token, out Action<SqlDataReader, EntitySet<TEntity>> refresh))
                refresh_setters.Add(token, refresh = token.Generate_Refresh_Setter());

            while (Reader.ReadAsync(CancellationToken).Result)
                refresh(Reader, EntitySet);
        }




        public override string ToString()
        {
            return $"{typeof(TEntityContext).Name} Context, {typeof(TEntity).Name} Entity Database Reader Select";
        }
    }
}
