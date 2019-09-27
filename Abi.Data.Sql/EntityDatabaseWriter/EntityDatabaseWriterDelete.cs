using System;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseWriterDelete<TEntityContext, TEntity, TEntityConcurrency> : EntityDatabaseWriter where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseWriterDelete()
        {
        }
        internal EntityDatabaseWriterDelete(EntityDatabaseConnection Connection) : base(Connection) { }


        private static (string CommandTextFixPart, Action<StringBuilder, TEntity> GetCommandTextVariablePart) CommandTextBuilder;
        private static (string CommandTextFixPart, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>> GetCommandTextVariablePart) BatchCommandTextBuilder;


        private static string GetCommandText(TEntity Entity)
        {
            if (CommandTextBuilder == default)
                CommandTextBuilder = GetCommandTextBuilder();

            StringBuilder builder = new StringBuilder();

            builder.Append(CommandTextBuilder.CommandTextFixPart);

            CommandTextBuilder.GetCommandTextVariablePart(builder, Entity);

            builder.Append(";");

            return builder.ToString();
        }
        private static (string, Action<StringBuilder, TEntity>) GetCommandTextBuilder()
        {
            Type typeEntity = typeof(TEntity);

            ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
            ParameterDefinition[] cncrny_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityConcurrency>().DerivativeProperties.Definitions;

            #region Generate Command Fix Part

            // Generate Command Fix Part

            string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

            StringBuilder builder = new StringBuilder();

            builder.Append($"execute sp_executesql");

            builder.AppendLine()
                   .Append($"       N'");

            builder.Append("set nocount on;");

            builder.AppendLine()
                   .Append($"         delete from {tableName}");

            builder.AppendLine();
            builder.Append($"         where {key_param.Name} = @{key_param.Name}");

            foreach (ParameterDefinition param in cncrny_params)
                builder.Append($" and {param.Name} = @{param.Name}");

            builder.Append(";");

            builder.AppendLine()
                   .AppendLine()
                   .Append($"         select @@rowcount;");

            builder.Append("'");

            builder.AppendLine()
                   .AppendLine()
                   .Append("     , N'");

            builder.Append($"@{key_param.Name} {key_param.ParameterType}");

            foreach (ParameterDefinition param in cncrny_params)
                builder.Append($", @{param.Name} {param.ParameterType}");

            builder.Append("'")
                   .AppendLine()
                   .AppendLine()
                   .Append("     ");

            string commandTextFixPart = builder.ToString();

            #endregion Generate Command Fix Part

            #region Generate Command Variable Part

            // Generate Command Variable Part

            #region Expression Tree
#if ET
            int counter = 0;

            Type typeData = EntityProxy<TEntity>.DataType;
            Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

            MethodInfo mthdAppend = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });
            MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            FieldInfo fldItem = typeProxyData.GetField("item");

            ParameterExpression sb = Expression.Parameter(typeof(StringBuilder), "sb");
            ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

            Expression[] expressions = new Expression[(cncrny_params.Length + 1) * 2];

            expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{key_param.Name} = "));
            expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(key_param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key_param.Property.Name))));

            foreach (ParameterDefinition param in cncrny_params)
            {
                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{param.Name} = "));
                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(param.Property.Name))));
            }

            Action<StringBuilder, TEntity> getCommandTextVariablePart = Expression.Lambda<Action<StringBuilder, TEntity>>(Expression.Block(expressions), sb, e).Compile();
#endif
            #endregion Expression Tree

            #region IL
#if IL
#endif
            #endregion IL

            #endregion Generate Command Variable Part

            return (commandTextFixPart, getCommandTextVariablePart);
        }
        private static (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>) GetBatchCommandTextBuilder()
        {
            Type typeEntity = typeof(TEntity);

            ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
            ParameterDefinition[] cncrny_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityConcurrency>().DerivativeProperties.Definitions;

            #region Generate Command Fix Part

            // Generate Command Fix Part

            string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

            StringBuilder builder = new StringBuilder();

            builder.Append($"execute sp_executesql");

            builder.AppendLine()
                   .Append($"       N'");

            builder.Append("set nocount on;");

            builder.AppendLine()
                   .Append($"         delete from {tableName}");

            builder.AppendLine();
            builder.Append($"         where {key_param.Name} = @{key_param.Name}");

            foreach (ParameterDefinition param in cncrny_params)
                builder.Append($" and {param.Name} = @{param.Name}");

            builder.Append(";");

            builder.AppendLine()
                   .AppendLine()
                   .Append($"         select @@rowcount;");

            builder.Append("'");

            builder.AppendLine()
                   .AppendLine()
                   .Append("     , N'");

            builder.Append($"@{key_param.Name} {key_param.ParameterType}");

            foreach (ParameterDefinition param in cncrny_params)
                builder.Append($", @{param.Name} {param.ParameterType}");

            builder.Append("'")
                   .AppendLine()
                   .AppendLine()
                   .Append("     ");

            string commandTextFixPart = builder.ToString();

            #endregion Generate Command Fix Part

            #region Generate Command Variable Part

            // Generate Command Variable Part

            #region Expression Tree
#if ET
            int counter = 0;

            Type typeData = EntityProxy<TEntity>.DataType;
            Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

            MethodInfo mthdAppend = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });
            MethodInfo propItemGetMthd = typeof(Dictionary<string, ParameterIdentity>).GetProperty("Item").GetGetMethod();
            MethodInfo propNameGetMthd = typeof(ParameterIdentity).GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            MethodInfo propIsDclGetMthd = typeof(ParameterIdentity).GetProperty("IsDeclarative", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            FieldInfo fldItem = typeProxyData.GetField("item");

            ParameterExpression sb = Expression.Parameter(typeof(StringBuilder), "sb");
            ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");
            ParameterExpression pi = Expression.Parameter(typeof(Dictionary<string, ParameterIdentity>), "pi");

            Expression[] expressions = new Expression[(cncrny_params.Length + 1) * 2];

            expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{key_param.Name} = "));
            expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(key_param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key_param.Property.Name))));

            foreach (ParameterDefinition param in cncrny_params)
            {
                Expression is_declarative = Expression.Call(Expression.Call(pi, propItemGetMthd, Expression.Constant(param.Property.Name)), propIsDclGetMthd);
                Expression declarative_param_value = Expression.Call(sb, mthdAppend, Expression.Call(Expression.Call(pi, propItemGetMthd, Expression.Constant(param.Property.Name)), propNameGetMthd));
                Expression non_declarative_param_value = Expression.Call(sb, mthdAppend, Expression.Call(param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(param.Property.Name))));

                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{param.Name} = "));
                expressions[counter++] = Expression.IfThenElse(is_declarative, declarative_param_value, non_declarative_param_value);
            }

            Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>> getCommandTextVariablePart = Expression.Lambda<Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>>(Expression.Block(expressions), sb, e, pi).Compile();
#endif
            #endregion Expression Tree

            #region IL
#if IL
#endif
            #endregion IL

            #endregion Generate Command Variable Part

            return (commandTextFixPart, getCommandTextVariablePart);
        }

        internal override void Append(Entity Entity, SaveBatchCommand Batch)
        {
            TEntity entity = (TEntity)Entity;

            if (entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!entity.IsDeleted())
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Deleted");

            Dictionary<string, ParameterIdentity> param_identities = new Dictionary<string, ParameterIdentity>();

            ParameterDefinition[] cncrny_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityConcurrency>().DerivativeProperties.Definitions;

            foreach (ParameterDefinition param in cncrny_params)
                param_identities.Add(param.Property.Name, Batch.GetParameter(entity, param.Property));



            if (BatchCommandTextBuilder == default)
                BatchCommandTextBuilder = GetBatchCommandTextBuilder();

            StringBuilder builder = new StringBuilder();

            builder.Append(BatchCommandTextBuilder.CommandTextFixPart);

            BatchCommandTextBuilder.GetCommandTextVariablePart(builder, entity, param_identities);

            builder.Append(";");

            string text = builder.ToString();

            EntityCommandDelete command = new EntityCommandDelete(this, entity, text);

            Batch.Add(command);
        }
        internal override void Append(Entity Entity, EntityCommand Command, ParameterIdentity Parameter)
        {
            throw new NotImplementedException();
        }

        internal override void Write(Entity Entity)
        {
            Write((TEntity)Entity);
        }
        internal override void Write(Entity Entity, SaveBatchCommand Batch, SqlDataReader Reader)
        {
            TEntity entity = (TEntity)Entity;

            Reader.Read(); // reader must has row, result of 'select @@rowcount'

            int affected_row_count = Reader.GetInt32(0); // select @@rowcount

            if (affected_row_count != 1)
                Batch.Exceptions.Add(entity);

            if (Batch.HasException)
                return;

            entity.State |= EntityState.Busy;

            entity.EntitySet.EntityKeyManager.Remove(entity);
            entity.EntitySet.Context.ChangeTracker.Remove(entity); // this line of code must be executed first, before state change

            entity.Detached();
        }
        public EntityDatabaseWriterDelete<TEntityContext, TEntity, TEntityConcurrency> Write(TEntity Entity)
        {
            if (Entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!Entity.IsDeleted())
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Deleted");

            using (SqlCommand Command = Connection.GetCommand(GetCommandText(Entity)))
            {
                DatabaseTrace.Append(Command.CommandText, Constant.Query);

                if (Connection.HasTransaction)
                    Command.Transaction = Connection.Transaction;

                try
                {
                    int affected_row_count = (int)Command.ExecuteScalar();

                    if (affected_row_count != 1)
                        throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, {Entity} Has Already Changed Or Removed Before That", Entity);
                }
                catch (Exception e)
                {
                    throw e;
                }

                Entity.State |= EntityState.Busy;

                Entity.EntitySet.EntityKeyManager.Remove(Entity);
                Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                Entity.Detached();
            }

            return this;
        }
        public EntityDatabaseWriterDelete<TEntityContext, TEntity, TEntityConcurrency> Write(params TEntity[] Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterDelete<TEntityContext, TEntity, TEntityConcurrency> Write(IEnumerable<TEntity> Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterDelete<TEntityContext, TEntity, TEntityConcurrency> Write<TSource>(IEnumerable<TSource> Items, Func<TSource, TEntity> Predicate)
        {
            StringBuilder builder = new StringBuilder();

            foreach (TSource Item in Items)
            {
                TEntity Entity = Predicate(Item);

                if (Entity.IsNull())
                    throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

                if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

                if (!Entity.IsDeleted())
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Deleted");

                builder.Append(GetCommandText(Entity));

                builder.AppendLine()
                       .AppendLine();
            }

            string CommandText = builder.ToString();

            using (SqlCommand Command = Connection.GetCommand(CommandText))
            {
                DatabaseTrace.Append(Command.CommandText, Constant.Query);

                if (Connection.HasTransaction)
                    Command.Transaction = Connection.Transaction;

                using (SqlDataReader Reader = Command.ExecuteReader(CommandBehavior.Default))
                {
                    List<Entity> concurrency_conflicts = new List<Entity>();

                    foreach (TSource Item in Items)
                    {
                        try
                        {
                            TEntity Entity = Predicate(Item);

                            Reader.Read(); // reader must has row, result of 'select @@rowcount'

                            int affected_row_count = Reader.GetInt32(0); // select @@rowcount

                            if (affected_row_count != 1)
                                concurrency_conflicts.Add(Entity);

                            if (concurrency_conflicts.Count > 0)
                            {
                                Reader.NextResult();
                                continue;
                            }

                            Entity.State |= EntityState.Busy;

                            Entity.EntitySet.EntityKeyManager.Remove(Entity);
                            Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                            Entity.Detached();

                            Reader.NextResult();
                        }
                        catch (Exception e)
                        {
                            if (concurrency_conflicts.Count > 0)
                                throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, Some Entities Had Already Changed Or Removed Before That", e, concurrency_conflicts.ToArray());
                            else
                                throw e;
                        }
                    }

                    if (concurrency_conflicts.Count > 0)
                        throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, Some Entities Had Already Changed Or Removed Before That", concurrency_conflicts.ToArray());

                    Reader.Close();
                }
            }

            return this;
        }


        public override string ToString()
        {
            return $"{typeof(TEntityContext).Name} Context, {typeof(TEntity).Name} Entity Database Writer Delete";
        }
    }








    public sealed class EntityDatabaseWriterDelete<TEntityContext, TEntity> : EntityDatabaseWriter where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseWriterDelete()
        {
        }
        internal EntityDatabaseWriterDelete(EntityDatabaseConnection Connection) : base(Connection) { }


        private static (string CommandTextFixPart, Action<StringBuilder, TEntity> GetCommandTextVariablePart) CommandTextBuilder;


        private static string GetCommandText(TEntity Entity)
        {
            if (CommandTextBuilder == default)
                CommandTextBuilder = GetCommandTextBuilder();

            StringBuilder builder = new StringBuilder();

            builder.Append(CommandTextBuilder.CommandTextFixPart);

            CommandTextBuilder.GetCommandTextVariablePart(builder, Entity);

            builder.Append(";");

            return builder.ToString();
        }
        private static (string, Action<StringBuilder, TEntity>) GetCommandTextBuilder()
        {
            Type typeEntity = typeof(TEntity);

            ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;

            #region Generate Command Fix Part

            // Generate Command Fix Part

            string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

            StringBuilder builder = new StringBuilder();

            builder.Append($"execute sp_executesql");

            builder.AppendLine()
                   .Append($"       N'");

            builder.Append("set nocount on;");

            builder.AppendLine()
                   .Append($"         delete from {tableName}");

            builder.AppendLine();
            builder.Append($"         where {key_param.Name} = @{key_param.Name}");

            builder.Append(";");

            builder.AppendLine()
                   .AppendLine()
                   .Append($"         select @@rowcount;");

            builder.Append("'");

            builder.AppendLine()
                   .AppendLine()
                   .Append("     , N'");

            builder.Append($"@{key_param.Name} {key_param.ParameterType}");

            builder.Append("'")
                   .AppendLine()
                   .AppendLine()
                   .Append("     ");

            string commandTextFixPart = builder.ToString();

            #endregion Generate Command Fix Part

            #region Generate Command Variable Part

            // Generate Command Variable Part

            #region Expression Tree
#if ET
            Type typeData = EntityProxy<TEntity>.DataType;
            Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

            MethodInfo mthdAppend = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });
            MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            FieldInfo fldItem = typeProxyData.GetField("item");

            ParameterExpression sb = Expression.Parameter(typeof(StringBuilder), "sb");
            ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

            Expression[] expressions = new Expression[2];

            expressions[0] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{key_param.Name} = "));
            expressions[1] = Expression.Call(sb, mthdAppend, Expression.Call(key_param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key_param.Property.Name))));

            Action<StringBuilder, TEntity> getCommandTextVariablePart = Expression.Lambda<Action<StringBuilder, TEntity>>(Expression.Block(expressions), sb, e).Compile();
#endif
            #endregion Expression Tree

            #region IL
#if IL
#endif
            #endregion IL

            #endregion Generate Command Variable Part

            return (commandTextFixPart, getCommandTextVariablePart);
        }

        internal override void Append(Entity Entity, SaveBatchCommand Batch)
        {
            TEntity entity = (TEntity)Entity;

            if (entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!entity.IsDeleted())
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Deleted");



            if (CommandTextBuilder == default)
                CommandTextBuilder = GetCommandTextBuilder();

            StringBuilder builder = new StringBuilder();

            builder.Append(CommandTextBuilder.CommandTextFixPart);

            CommandTextBuilder.GetCommandTextVariablePart(builder, entity);

            builder.Append(";");

            string text = builder.ToString();

            EntityCommandDelete command = new EntityCommandDelete(this, entity, text);

            Batch.Add(command);
        }
        internal override void Append(Entity Entity, EntityCommand Command, ParameterIdentity Parameter)
        {
            throw new NotImplementedException();
        }

        internal override void Write(Entity Entity)
        {
            Write((TEntity)Entity);
        }
        internal override void Write(Entity Entity, SaveBatchCommand Batch, SqlDataReader Reader)
        {
            TEntity entity = (TEntity)Entity;

            Reader.Read(); // reader must has row, result of 'select @@rowcount'

            int affected_row_count = Reader.GetInt32(0); // select @@rowcount

            if (affected_row_count != 1)
                Batch.Exceptions.Add(entity);

            if (Batch.HasException)
                return;

            entity.State |= EntityState.Busy;

            entity.EntitySet.EntityKeyManager.Remove(entity);
            entity.EntitySet.Context.ChangeTracker.Remove(entity); // this line of code must be executed first, before state change

            entity.Detached();
        }
        public EntityDatabaseWriterDelete<TEntityContext, TEntity> Write(TEntity Entity)
        {
            if (Entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!Entity.IsDeleted())
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Deleted");

            using (SqlCommand Command = Connection.GetCommand(GetCommandText(Entity)))
            {
                DatabaseTrace.Append(Command.CommandText, Constant.Query);

                if (Connection.HasTransaction)
                    Command.Transaction = Connection.Transaction;

                try
                {
                    int affected_row_count = (int)Command.ExecuteScalar();

                    if (affected_row_count != 1)
                        throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, {Entity} Has Already Changed Or Removed Before That", Entity);
                }
                catch (Exception e)
                {
                    throw e;
                }

                Entity.State |= EntityState.Busy;

                Entity.EntitySet.EntityKeyManager.Remove(Entity);
                Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                Entity.Detached();
            }

            return this;
        }
        public EntityDatabaseWriterDelete<TEntityContext, TEntity> Write(params TEntity[] Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterDelete<TEntityContext, TEntity> Write(IEnumerable<TEntity> Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterDelete<TEntityContext, TEntity> Write<TSource>(IEnumerable<TSource> Items, Func<TSource, TEntity> Predicate)
        {
            StringBuilder builder = new StringBuilder();

            foreach (TSource Item in Items)
            {
                TEntity Entity = Predicate(Item);

                if (Entity.IsNull())
                    throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

                if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

                if (!Entity.IsDeleted())
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Deleted");

                builder.Append(GetCommandText(Entity));

                builder.AppendLine()
                       .AppendLine();
            }

            string CommandText = builder.ToString();

            using (SqlCommand Command = Connection.GetCommand(CommandText))
            {
                DatabaseTrace.Append(Command.CommandText, Constant.Query);

                if (Connection.HasTransaction)
                    Command.Transaction = Connection.Transaction;

                using (SqlDataReader Reader = Command.ExecuteReader(CommandBehavior.Default))
                {
                    List<Entity> concurrency_conflicts = new List<Entity>();

                    foreach (TSource Item in Items)
                    {
                        try
                        {
                            TEntity Entity = Predicate(Item);

                            Reader.Read(); // reader must has row, result of 'select @@rowcount'

                            int affected_row_count = Reader.GetInt32(0); // select @@rowcount

                            if (affected_row_count != 1)
                                concurrency_conflicts.Add(Entity);

                            if (concurrency_conflicts.Count > 0)
                            {
                                Reader.NextResult();
                                continue;
                            }

                            Entity.State |= EntityState.Busy;

                            Entity.EntitySet.EntityKeyManager.Remove(Entity);
                            Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                            Entity.Detached();

                            Reader.NextResult();
                        }
                        catch (Exception e)
                        {
                            if (concurrency_conflicts.Count > 0)
                                throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, Some Entities Had Already Changed Or Removed Before That", e, concurrency_conflicts.ToArray());
                            else
                                throw e;
                        }
                    }

                    if (concurrency_conflicts.Count > 0)
                        throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, Some Entities Had Already Changed Or Removed Before That", concurrency_conflicts.ToArray());

                    Reader.Close();
                }
            }

            return this;
        }


        public override string ToString()
        {
            return $"{typeof(TEntityContext).Name} Context, {typeof(TEntity).Name} Entity Database Writer Delete";
        }
    }
}
