using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency> : EntityDatabaseWriter where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseWriterUpdate()
        {
            CommandTextBuilders = new Dictionary<int, (string, Action<StringBuilder, TEntity>)>();
            BatchCommandTextBuilders = new Dictionary<int, (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>)>();

            EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();

            Type typeEntity = typeof(TEntity);
            Type typeEntityCallBack = typeof(TEntityCallBack);

            PropertyInfo[] entityCallBackProperties = typeEntityCallBack.GetEntityAcceptableDataProperties().ToArray();

            #region Generate CallBackData Setter

            #region Expression Tree
#if ET
            int counter = 0;
            int index = 0;
            int resize = 0;

            Type typeData = EntityProxy<TEntity>.DataType;
            Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

            Type typeEntitySet = typeof(EntitySet<>).MakeGenericType(typeEntity);

            MethodInfo propEntitySetGetMthd = typeEntity.GetProperty("EntitySet", typeEntitySet).GetGetMethod(true);
            MethodInfo mthdOnPropChng = typeEntity.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
            MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            FieldInfo fldItem = typeProxyData.GetField("item");

            ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");
            ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

            Expression[] expressions = new Expression[entityCallBackProperties.Length * 2];

            foreach (PropertyInfo propCallBack in entityCallBackProperties)
            {
                PropertyInfo propEntity = typeEntity.GetProperty(propCallBack.Name);
                EntityRelation entityRelation = configuration.EntityRelations.GetEntityRelation(propEntity);

                if (entityRelation == null)
                {
                    expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(propCallBack.Name)), Expression.Call(propCallBack.GetDataReaderGetValueMethod(), dr, Expression.Constant(index++)));
                    expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(propCallBack.Name));
                }
                else
                {
                    Type typeEditedEntity = typeof(EditedEntity<,>).MakeGenericType(typeEntity, propCallBack.PropertyType);
                    Type typeEditedProperty = typeof(EditedProperty<>).MakeGenericType(propCallBack.PropertyType);

                    ConstructorInfo ctrEdtdProp = typeEditedProperty.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(PropertyInfo), propCallBack.PropertyType, propCallBack.PropertyType }, null);
                    ConstructorInfo ctrEdtdEnty = typeEditedEntity.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeEditedProperty, typeof(TrakMode) }, null);

                    Expression expNewEdtdProp = Expression.New(ctrEdtdProp, new Expression[] { Expression.Constant(propEntity), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(propCallBack.Name)), Expression.Call(propCallBack.GetDataReaderGetValueMethod(), dr, Expression.Constant(index++)) });
                    Expression expNewEdtdEnty = Expression.New(ctrEdtdEnty, new Expression[] { expNewEdtdProp, Expression.Constant(TrakMode.None) });

                    MethodInfo mthdCallBack = typeof(EntitySet<TEntity>).GetMethod("CallBack", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(propCallBack.PropertyType);

                    expressions[counter++] = Expression.Call(Expression.Call(e, propEntitySetGetMthd), mthdCallBack, new[] { e, expNewEdtdEnty });

                    resize++;
                }
            }

            if (resize > 0)
                Array.Resize(ref expressions, expressions.Length - resize);

            SetCallBackData = Expression.Lambda<Action<SqlDataReader, TEntity>>(Expression.Block(expressions), dr, e).Compile();
#endif
            #endregion Expression Tree

            #region IL
#if IL
#endif
            #endregion IL

            #endregion Generate CallBackData Setter

        }
        internal EntityDatabaseWriterUpdate(EntityDatabaseConnection Connection) : base(Connection) { }


        private static Action<SqlDataReader, TEntity> SetCallBackData;
        private static Dictionary<int, (string, Action<StringBuilder, TEntity>)> CommandTextBuilders;
        private static Dictionary<int, (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>)> BatchCommandTextBuilders;


        private static string GetCommandText(TEntity Entity)
        {
            StringBuilder builder = new StringBuilder();

            (string CommandTextFixPart, Action<StringBuilder, TEntity> GetCommandTextVariablePart) commandTextBuilder = GetCommandTextBuilder(Entity);

            builder.Append(commandTextBuilder.CommandTextFixPart);

            commandTextBuilder.GetCommandTextVariablePart(builder, Entity);

            builder.Append(";");

            return builder.ToString();
        }
        private static (string, Action<StringBuilder, TEntity>) GetCommandTextBuilder(TEntity Entity)
        {
            int patternID = Entity.EntitySet.EntityChangeTracker.GetPatternID(Entity);

            if (!CommandTextBuilders.TryGetValue(patternID, out (string, Action<StringBuilder, TEntity>) commandTextBuilder))
            {
                Type typeEntity = typeof(TEntity);

                ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
                ParameterDefinition[] update_set_params = CommandParameter<TEntityContext, TEntity>.Modified(Entity).ModifiedProperties.Definitions;
                ParameterDefinition[] calbck_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityCallBack>().DerivativeProperties.Definitions;
                ParameterDefinition[] cncrny_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityConcurrency>().DerivativeProperties.Definitions;

                #region Generate Command Fix Part

                // Generate Command Fix Part

                string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

                StringBuilder builder = new StringBuilder();

                builder.Append($"execute sp_executesql");

                builder.AppendLine()
                       .Append($"       N'");

                builder.Append($"set nocount on;");

                builder.AppendLine()
                       .Append($"         update {tableName}");

                builder.AppendLine()
                       .Append("           set ");

                int index = update_set_params.Length;

                foreach (ParameterDefinition param in update_set_params)
                {
                    builder.Append($"{param.Name} = @{param.Name}");

                    if (--index > 0)
                        builder.Append(", ");
                }

                builder.AppendLine()
                       .Append($"             output ");

                foreach (ParameterDefinition param in calbck_params)
                    builder.Append("inserted.").Append($"{param.Name}").Append(", ");

                builder.Remove(builder.Length - 2, 2);

                builder.AppendLine();
                builder.Append($"         where {key_param.Name} = @{key_param.Name}");

                foreach (ParameterDefinition param in cncrny_params)
                    builder.Append($" and {param.Name} = @{param.Name}");

                builder.Append(";");

                builder.Append("'");

                builder.AppendLine()
                       .AppendLine()
                       .Append("     , N'");

                builder.Append($"@{key_param.Name} {key_param.ParameterType}");

                foreach (ParameterDefinition param in cncrny_params)
                    builder.Append($", @{param.Name} {param.ParameterType}");

                foreach (ParameterDefinition param in update_set_params)
                    if (!cncrny_params.Any(p => p == param))
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

                Expression[] expressions = new Expression[(cncrny_params.Length + update_set_params.Length + 1) * 2];

                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{key_param.Name} = "));
                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(key_param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key_param.Property.Name))));

                foreach (ParameterDefinition param in cncrny_params)
                {
                    expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{param.Name} = "));
                    expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(param.Property.Name))));
                }

                foreach (ParameterDefinition param in update_set_params)
                    if (!cncrny_params.Any(p => p == param))
                    {
                        expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{param.Name} = "));
                        expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(param.Property.Name))));
                    }
                    else
                        Array.Resize(ref expressions, expressions.Length - 2);

                Action<StringBuilder, TEntity> getCommandTextVariablePart = Expression.Lambda<Action<StringBuilder, TEntity>>(Expression.Block(expressions), sb, e).Compile();
#endif
                #endregion Expression Tree

                #region IL
#if IL
            {
            }
#endif
                #endregion IL

                #endregion Generate Command Variable Part

                CommandTextBuilders.Add(patternID, commandTextBuilder = (commandTextFixPart, getCommandTextVariablePart));
            }

            return commandTextBuilder;
        }
        private static (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>) GetBatchCommandTextBuilder(TEntity Entity)
        {
            int patternID = Entity.EntitySet.EntityChangeTracker.GetPatternID(Entity);

            if (!BatchCommandTextBuilders.TryGetValue(patternID, out (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>) commandTextBuilder))
            {
                Type typeEntity = typeof(TEntity);

                ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
                ParameterDefinition[] update_set_params = CommandParameter<TEntityContext, TEntity>.Modified(Entity).ModifiedProperties.Definitions;
                ParameterDefinition[] calbck_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityCallBack>().DerivativeProperties.Definitions;
                ParameterDefinition[] cncrny_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityConcurrency>().DerivativeProperties.Definitions;

                #region Generate Command Fix Part

                // Generate Command Fix Part

                string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

                StringBuilder builder = new StringBuilder();

                builder.Append($"execute sp_executesql");

                builder.AppendLine()
                       .Append($"       N'");

                builder.Append($"set nocount on;");

                builder.AppendLine()
                       .Append($"         update {tableName}");

                builder.AppendLine()
                       .Append("           set ");

                int index = update_set_params.Length;

                foreach (ParameterDefinition param in update_set_params)
                {
                    builder.Append($"{param.Name} = @{param.Name}");

                    if (--index > 0)
                        builder.Append(", ");
                }

                builder.AppendLine()
                       .Append($"             output ");

                foreach (ParameterDefinition param in calbck_params)
                    builder.Append("inserted.").Append($"{param.Name}").Append(", ");

                builder.Remove(builder.Length - 2, 2);

                builder.AppendLine();
                builder.Append($"         where {key_param.Name} = @{key_param.Name}");

                foreach (ParameterDefinition param in cncrny_params)
                    builder.Append($" and {param.Name} = @{param.Name}");

                builder.Append(";");

                builder.Append("'");

                builder.AppendLine()
                       .AppendLine()
                       .Append("     , N'");

                builder.Append($"@{key_param.Name} {key_param.ParameterType}");

                foreach (ParameterDefinition param in cncrny_params)
                    builder.Append($", @{param.Name} {param.ParameterType}");

                foreach (ParameterDefinition param in update_set_params)
                    if (!cncrny_params.Any(p => p == param))
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

                Expression[] expressions = new Expression[(cncrny_params.Length + update_set_params.Length + 1) * 2];

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

                foreach (ParameterDefinition param in update_set_params)
                    if (!cncrny_params.Any(p => p == param))
                    {
                        Expression is_declarative = Expression.Call(Expression.Call(pi, propItemGetMthd, Expression.Constant(param.Property.Name)), propIsDclGetMthd);
                        Expression declarative_param_value = Expression.Call(sb, mthdAppend, Expression.Call(Expression.Call(pi, propItemGetMthd, Expression.Constant(param.Property.Name)), propNameGetMthd));
                        Expression non_declarative_param_value = Expression.Call(sb, mthdAppend, Expression.Call(param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(param.Property.Name))));

                        expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{param.Name} = "));
                        expressions[counter++] = Expression.IfThenElse(is_declarative, declarative_param_value, non_declarative_param_value);
                    }
                    else
                        Array.Resize(ref expressions, expressions.Length - 2);

                Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>> getCommandTextVariablePart = Expression.Lambda<Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>>(Expression.Block(expressions), sb, e, pi).Compile();
#endif
                #endregion Expression Tree

                #region IL
#if IL
            {
            }
#endif
                #endregion IL

                #endregion Generate Command Variable Part

                BatchCommandTextBuilders.Add(patternID, commandTextBuilder = (commandTextFixPart, getCommandTextVariablePart));
            }

            return commandTextBuilder;
        }

        internal override void Append(Entity Entity, SaveBatchCommand Batch)
        {
            TEntity entity = (TEntity)Entity;

            if (entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!entity.HasModified())
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} State Must Be Modified");

            Dictionary<string, ParameterIdentity> param_identities = new Dictionary<string, ParameterIdentity>();

            ParameterDefinition[] update_set_params = CommandParameter<TEntityContext, TEntity>.Modified(entity).ModifiedProperties.Definitions;
            ParameterDefinition[] cncrny_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityConcurrency>().DerivativeProperties.Definitions;

            foreach (ParameterDefinition param in update_set_params)
                param_identities.Add(param.Property.Name, Batch.GetParameter(entity, param.Property));

            foreach (ParameterDefinition param in cncrny_params)
                if (!param_identities.ContainsKey(param.Property.Name))
                    param_identities.Add(param.Property.Name, Batch.GetParameter(entity, param.Property));


            StringBuilder builder = new StringBuilder();

            (string CommandTextFixPart, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>> GetCommandTextVariablePart) commandTextBuilder = GetBatchCommandTextBuilder(entity);

            builder.Append(commandTextBuilder.CommandTextFixPart);

            commandTextBuilder.GetCommandTextVariablePart(builder, entity, param_identities);

            builder.Append(";");

            string text = builder.ToString();

            EntityCommandUpdate command = new EntityCommandUpdate(this, entity, text);

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

            if (!Reader.HasRows)
            {
                try
                {
                    if (!Reader.Read())
                        Batch.Exceptions.Add(entity);
                }
                catch (Exception e)
                {
                    if (Batch.HasException)
                        throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, Some Entities Had Already Changed Or Removed Before That", e, Batch.Exceptions.ToArray());
                    else
                        throw e;
                }
            }

            if (Batch.HasException)
                return;

            Reader.Read();

            entity.State |= EntityState.Busy;

            SetCallBackData(Reader, entity);

            entity.EntitySet.NotifyAllRelated(entity);

            entity.EntitySet.EntityChangeTracker.Remove(entity);
            entity.EntitySet.Context.ChangeTracker.Remove(entity); // this line of code must be executed first, before state change

            entity.State = EntityState.Unchanged;
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency> Write(TEntity Entity)
        {
            if (Entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!Entity.HasModified())
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Modified");

            using (SqlCommand Command = Connection.GetCommand(GetCommandText(Entity)))
            {
                DatabaseTrace.Append(Command.CommandText, Constant.Query);

                if (Connection.HasTransaction)
                    Command.Transaction = Connection.Transaction;

                using (SqlDataReader Reader = Command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!Reader.HasRows)
                    {
                        try
                        {
                            if (!Reader.Read()) // if reader haven't exception, it can read but return false (Reader.Read() returns true if there are more rows; otherwise false)
                                throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, {Entity} Has Already Changed Or Removed Before That", Entity);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }

                    Reader.Read();

                    Entity.State |= EntityState.Busy;

                    SetCallBackData(Reader, Entity);

                    Entity.EntitySet.NotifyAllRelated(Entity);

                    Entity.EntitySet.EntityChangeTracker.Remove(Entity);
                    Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                    Entity.State = EntityState.Unchanged;

                    Reader.Close();
                }
            }

            return this;
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency> Write(params TEntity[] Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency> Write(IEnumerable<TEntity> Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack, TEntityConcurrency> Write<TSource>(IEnumerable<TSource> Items, Func<TSource, TEntity> Predicate)
        {
            StringBuilder builder = new StringBuilder();

            foreach (TSource Item in Items)
            {
                TEntity Entity = Predicate(Item);

                if (Entity.IsNull())
                    throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

                if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

                if (!Entity.HasModified())
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Modified");

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

                            if (!Reader.HasRows)
                                if (!Reader.Read()) // force to read exception ...
                                    concurrency_conflicts.Add(Entity);

                            if (concurrency_conflicts.Count > 0)
                            {
                                Reader.NextResult();
                                continue;
                            }

                            Reader.Read();

                            Entity.State |= EntityState.Busy;

                            SetCallBackData(Reader, Entity);

                            Entity.EntitySet.NotifyAllRelated(Entity);

                            Entity.EntitySet.EntityChangeTracker.Remove(Entity);
                            Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                            Entity.State = EntityState.Unchanged;

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
            return $"{typeof(TEntityContext).Name} Context, {typeof(TEntity).Name} Entity Database Writer Update";
        }
    }








    public sealed class EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack> : EntityDatabaseWriter where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseWriterUpdate()
        {
            CommandTextBuilders = new Dictionary<int, (string, Action<StringBuilder, TEntity>)>();
            BatchCommandTextBuilders = new Dictionary<int, (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>)>();

            EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();

            Type typeEntity = typeof(TEntity);
            Type typeEntityCallBack = typeof(TEntityCallBack);

            PropertyInfo[] entityCallBackProperties = typeEntityCallBack.GetEntityAcceptableDataProperties().ToArray();

            #region Generate CallBackData Setter

            #region Expression Tree
#if ET
            int counter = 0;
            int index = 0;
            int resize = 0;

            Type typeData = EntityProxy<TEntity>.DataType;
            Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

            Type typeEntitySet = typeof(EntitySet<>).MakeGenericType(typeEntity);

            MethodInfo propEntitySetGetMthd = typeEntity.GetProperty("EntitySet", typeEntitySet).GetGetMethod(true);
            MethodInfo mthdOnPropChng = typeEntity.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
            MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            FieldInfo fldItem = typeProxyData.GetField("item");

            ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");
            ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

            Expression[] expressions = new Expression[entityCallBackProperties.Length * 2];

            foreach (PropertyInfo propCallBack in entityCallBackProperties)
            {
                PropertyInfo propEntity = typeEntity.GetProperty(propCallBack.Name);
                EntityRelation entityRelation = configuration.EntityRelations.GetEntityRelation(propEntity);

                if (entityRelation == null)
                {
                    expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(propCallBack.Name)), Expression.Call(propCallBack.GetDataReaderGetValueMethod(), dr, Expression.Constant(index++)));
                    expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(propCallBack.Name));
                }
                else
                {
                    Type typeEditedEntity = typeof(EditedEntity<,>).MakeGenericType(typeEntity, propCallBack.PropertyType);
                    Type typeEditedProperty = typeof(EditedProperty<>).MakeGenericType(propCallBack.PropertyType);

                    ConstructorInfo ctrEdtdProp = typeEditedProperty.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(PropertyInfo), propCallBack.PropertyType, propCallBack.PropertyType }, null);
                    ConstructorInfo ctrEdtdEnty = typeEditedEntity.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeEditedProperty, typeof(TrakMode) }, null);

                    Expression expNewEdtdProp = Expression.New(ctrEdtdProp, new Expression[] { Expression.Constant(propEntity), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(propCallBack.Name)), Expression.Call(propCallBack.GetDataReaderGetValueMethod(), dr, Expression.Constant(index++)) });
                    Expression expNewEdtdEnty = Expression.New(ctrEdtdEnty, new Expression[] { expNewEdtdProp, Expression.Constant(TrakMode.None) });

                    MethodInfo mthdCallBack = typeof(EntitySet<TEntity>).GetMethod("CallBack", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(propCallBack.PropertyType);

                    expressions[counter++] = Expression.Call(Expression.Call(e, propEntitySetGetMthd), mthdCallBack, new[] { e, expNewEdtdEnty });

                    resize++;
                }
            }

            if (resize > 0)
                Array.Resize(ref expressions, expressions.Length - resize);

            SetCallBackData = Expression.Lambda<Action<SqlDataReader, TEntity>>(Expression.Block(expressions), dr, e).Compile();
#endif
            #endregion Expression Tree

            #region IL
#if IL
#endif
            #endregion IL

            #endregion Generate CallBackData Setter

        }
        internal EntityDatabaseWriterUpdate(EntityDatabaseConnection Connection) : base(Connection) { }


        private static Action<SqlDataReader, TEntity> SetCallBackData;
        private static Dictionary<int, (string, Action<StringBuilder, TEntity>)> CommandTextBuilders;
        private static Dictionary<int, (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>)> BatchCommandTextBuilders;


        private static string GetCommandText(TEntity Entity)
        {
            StringBuilder builder = new StringBuilder();

            (string CommandTextFixPart, Action<StringBuilder, TEntity> GetCommandTextVariablePart) commandTextBuilder = GetCommandTextBuilder(Entity);

            builder.Append(commandTextBuilder.CommandTextFixPart);

            commandTextBuilder.GetCommandTextVariablePart(builder, Entity);

            builder.Append(";");

            return builder.ToString();
        }
        private static (string, Action<StringBuilder, TEntity>) GetCommandTextBuilder(TEntity Entity)
        {
            int patternID = Entity.EntitySet.EntityChangeTracker.GetPatternID(Entity);

            if (!CommandTextBuilders.TryGetValue(patternID, out (string, Action<StringBuilder, TEntity>) commandTextBuilder))
            {
                Type typeEntity = typeof(TEntity);

                ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
                ParameterDefinition[] update_set_params = CommandParameter<TEntityContext, TEntity>.Modified(Entity).ModifiedProperties.Definitions;
                ParameterDefinition[] calbck_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityCallBack>().DerivativeProperties.Definitions;

                #region Generate Command Fix Part

                // Generate Command Fix Part

                string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

                StringBuilder builder = new StringBuilder();

                builder.Append($"execute sp_executesql");

                builder.AppendLine()
                       .Append($"       N'");

                builder.Append($"set nocount on;");

                builder.AppendLine()
                       .Append($"         update {tableName}");

                builder.AppendLine()
                       .Append("           set ");

                int index = update_set_params.Length;

                foreach (ParameterDefinition param in update_set_params)
                {
                    builder.Append($"{param.Name} = @{param.Name}");

                    if (--index > 0)
                        builder.Append(", ");
                }

                builder.AppendLine()
                       .Append($"             output ");

                foreach (ParameterDefinition param in calbck_params)
                    builder.Append("inserted.").Append($"{param.Name}").Append(", ");

                builder.Remove(builder.Length - 2, 2);

                builder.AppendLine();
                builder.Append($"         where {key_param.Name} = @{key_param.Name}");

                builder.Append(";");

                builder.Append("'");

                builder.AppendLine()
                       .AppendLine()
                       .Append("     , N'");

                builder.Append($"@{key_param.Name} {key_param.ParameterType}");

                foreach (ParameterDefinition param in update_set_params)
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

                Expression[] expressions = new Expression[(update_set_params.Length + 1) * 2];

                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{key_param.Name} = "));
                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(key_param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key_param.Property.Name))));

                foreach (ParameterDefinition param in update_set_params)
                {
                    expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{param.Name} = "));
                    expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(param.Property.Name))));
                }

                Action<StringBuilder, TEntity> getCommandTextVariablePart = Expression.Lambda<Action<StringBuilder, TEntity>>(Expression.Block(expressions), sb, e).Compile();
#endif
                #endregion Expression Tree

                #region IL
#if IL
            {
            }
#endif
                #endregion IL

                #endregion Generate Command Variable Part

                CommandTextBuilders.Add(patternID, commandTextBuilder = (commandTextFixPart, getCommandTextVariablePart));
            }

            return commandTextBuilder;
        }
        private static (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>) GetBatchCommandTextBuilder(TEntity Entity)
        {
            int patternID = Entity.EntitySet.EntityChangeTracker.GetPatternID(Entity);

            if (!BatchCommandTextBuilders.TryGetValue(patternID, out (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>) commandTextBuilder))
            {
                Type typeEntity = typeof(TEntity);

                ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
                ParameterDefinition[] update_set_params = CommandParameter<TEntityContext, TEntity>.Modified(Entity).ModifiedProperties.Definitions;
                ParameterDefinition[] calbck_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityCallBack>().DerivativeProperties.Definitions;

                #region Generate Command Fix Part

                // Generate Command Fix Part

                string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

                StringBuilder builder = new StringBuilder();

                builder.Append($"execute sp_executesql");

                builder.AppendLine()
                       .Append($"       N'");

                builder.Append($"set nocount on;");

                builder.AppendLine()
                       .Append($"         update {tableName}");

                builder.AppendLine()
                       .Append("           set ");

                int index = update_set_params.Length;

                foreach (ParameterDefinition param in update_set_params)
                {
                    builder.Append($"{param.Name} = @{param.Name}");

                    if (--index > 0)
                        builder.Append(", ");
                }

                builder.AppendLine()
                       .Append($"             output ");

                foreach (ParameterDefinition param in calbck_params)
                    builder.Append("inserted.").Append($"{param.Name}").Append(", ");

                builder.Remove(builder.Length - 2, 2);

                builder.AppendLine();
                builder.Append($"         where {key_param.Name} = @{key_param.Name}");

                builder.Append(";");

                builder.Append("'");

                builder.AppendLine()
                       .AppendLine()
                       .Append("     , N'");

                builder.Append($"@{key_param.Name} {key_param.ParameterType}");

                foreach (ParameterDefinition param in update_set_params)
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

                Expression[] expressions = new Expression[(update_set_params.Length + 1) * 2];

                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{key_param.Name} = "));
                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(key_param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key_param.Property.Name))));

                foreach (ParameterDefinition param in update_set_params)
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
            {
            }
#endif
                #endregion IL

                #endregion Generate Command Variable Part

                BatchCommandTextBuilders.Add(patternID, commandTextBuilder = (commandTextFixPart, getCommandTextVariablePart));
            }

            return commandTextBuilder;
        }

        internal override void Append(Entity Entity, SaveBatchCommand Batch)
        {
            TEntity entity = (TEntity)Entity;

            if (entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!entity.HasModified())
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} State Must Be Modified");

            Dictionary<string, ParameterIdentity> param_identities = new Dictionary<string, ParameterIdentity>();

            ParameterDefinition[] update_set_params = CommandParameter<TEntityContext, TEntity>.Modified(entity).ModifiedProperties.Definitions;

            foreach (ParameterDefinition param in update_set_params)
                param_identities.Add(param.Property.Name, Batch.GetParameter(entity, param.Property));


            StringBuilder builder = new StringBuilder();

            (string CommandTextFixPart, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>> GetCommandTextVariablePart) commandTextBuilder = GetBatchCommandTextBuilder(entity);

            builder.Append(commandTextBuilder.CommandTextFixPart);

            commandTextBuilder.GetCommandTextVariablePart(builder, entity, param_identities);

            builder.Append(";");

            string text = builder.ToString();

            EntityCommandUpdate command = new EntityCommandUpdate(this, entity, text);

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

            if (!Reader.HasRows)
            {
                try
                {
                    if (!Reader.Read())
                        Batch.Exceptions.Add(entity);
                }
                catch (Exception e)
                {
                    if (Batch.HasException)
                        throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, Some Entities Had Already Changed Or Removed Before That", e, Batch.Exceptions.ToArray());
                    else
                        throw e;
                }
            }

            if (Batch.HasException)
                return;

            Reader.Read();

            entity.State |= EntityState.Busy;

            SetCallBackData(Reader, entity);

            entity.EntitySet.NotifyAllRelated(entity);

            entity.EntitySet.EntityChangeTracker.Remove(entity);
            entity.EntitySet.Context.ChangeTracker.Remove(entity); // this line of code must be executed first, before state change

            entity.State = EntityState.Unchanged;
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack> Write(TEntity Entity)
        {
            if (Entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!Entity.HasModified())
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Modified");

            using (SqlCommand Command = Connection.GetCommand(GetCommandText(Entity)))
            {
                DatabaseTrace.Append(Command.CommandText, Constant.Query);

                if (Connection.HasTransaction)
                    Command.Transaction = Connection.Transaction;

                using (SqlDataReader Reader = Command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!Reader.HasRows)
                    {
                        try
                        {
                            if (!Reader.Read()) // if reader haven't exception, it can read but return false (Reader.Read() returns true if there are more rows; otherwise false)
                                throw new EntityDatabaseConcurrencyException($"Concurrency Conflicts, {Entity} Has Already Changed Or Removed Before That", Entity);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }

                    Reader.Read();

                    Entity.State |= EntityState.Busy;

                    SetCallBackData(Reader, Entity);

                    Entity.EntitySet.NotifyAllRelated(Entity);

                    Entity.EntitySet.EntityChangeTracker.Remove(Entity);
                    Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                    Entity.State = EntityState.Unchanged;

                    Reader.Close();
                }
            }

            return this;
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack> Write(params TEntity[] Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack> Write(IEnumerable<TEntity> Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity, TEntityCallBack> Write<TSource>(IEnumerable<TSource> Items, Func<TSource, TEntity> Predicate)
        {
            StringBuilder builder = new StringBuilder();

            foreach (TSource Item in Items)
            {
                TEntity Entity = Predicate(Item);

                if (Entity.IsNull())
                    throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

                if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

                if (!Entity.HasModified())
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Modified");

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

                            if (!Reader.HasRows)
                                if (!Reader.Read()) // force to read exception ...
                                    concurrency_conflicts.Add(Entity);

                            if (concurrency_conflicts.Count > 0)
                            {
                                Reader.NextResult();
                                continue;
                            }

                            Reader.Read();

                            Entity.State |= EntityState.Busy;

                            SetCallBackData(Reader, Entity);

                            Entity.EntitySet.NotifyAllRelated(Entity);

                            Entity.EntitySet.EntityChangeTracker.Remove(Entity);
                            Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                            Entity.State = EntityState.Unchanged;

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
            return $"{typeof(TEntityContext).Name} Context, {typeof(TEntity).Name} Entity Database Writer Update";
        }
    }








    public sealed class EntityDatabaseWriterUpdate<TEntityContext, TEntity> : EntityDatabaseWriter where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseWriterUpdate()
        {
            CommandTextBuilders = new Dictionary<int, (string, Action<StringBuilder, TEntity>)>();
            BatchCommandTextBuilders = new Dictionary<int, (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>)>();
        }
        internal EntityDatabaseWriterUpdate(EntityDatabaseConnection Connection) : base(Connection) { }


        private static Dictionary<int, (string, Action<StringBuilder, TEntity>)> CommandTextBuilders;
        private static Dictionary<int, (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>)> BatchCommandTextBuilders;


        private static string GetCommandText(TEntity Entity)
        {
            StringBuilder builder = new StringBuilder();

            (string CommandTextFixPart, Action<StringBuilder, TEntity> GetCommandTextVariablePart) commandTextBuilder = GetCommandTextBuilder(Entity);

            builder.Append(commandTextBuilder.CommandTextFixPart);

            commandTextBuilder.GetCommandTextVariablePart(builder, Entity);

            builder.Append(";");

            return builder.ToString();
        }
        private static (string, Action<StringBuilder, TEntity>) GetCommandTextBuilder(TEntity Entity)
        {
            int patternID = Entity.EntitySet.EntityChangeTracker.GetPatternID(Entity);

            if (!CommandTextBuilders.TryGetValue(patternID, out (string, Action<StringBuilder, TEntity>) commandTextBuilder))
            {
                Type typeEntity = typeof(TEntity);

                ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
                ParameterDefinition[] update_set_params = CommandParameter<TEntityContext, TEntity>.Modified(Entity).ModifiedProperties.Definitions;

                #region Generate Command Fix Part

                // Generate Command Fix Part

                string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

                StringBuilder builder = new StringBuilder();

                builder.Append($"execute sp_executesql");

                builder.AppendLine()
                       .Append($"       N'");

                builder.Append($"set nocount on;");

                builder.AppendLine()
                       .Append($"         update {tableName}");

                builder.AppendLine()
                       .Append("           set ");

                int index = update_set_params.Length;

                foreach (ParameterDefinition param in update_set_params)
                {
                    builder.Append($"{param.Name} = @{param.Name}");

                    if (--index > 0)
                        builder.Append(", ");
                }

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

                foreach (ParameterDefinition param in update_set_params)
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

                Expression[] expressions = new Expression[(update_set_params.Length + 1) * 2];

                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{key_param.Name} = "));
                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(key_param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key_param.Property.Name))));

                foreach (ParameterDefinition param in update_set_params)
                {
                    expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{param.Name} = "));
                    expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(param.Property.Name))));
                }

                Action<StringBuilder, TEntity> getCommandTextVariablePart = Expression.Lambda<Action<StringBuilder, TEntity>>(Expression.Block(expressions), sb, e).Compile();
#endif
                #endregion Expression Tree

                #region IL
#if IL
            {
            }
#endif
                #endregion IL

                #endregion Generate Command Variable Part

                CommandTextBuilders.Add(patternID, commandTextBuilder = (commandTextFixPart, getCommandTextVariablePart));
            }

            return commandTextBuilder;
        }
        private static (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>) GetBatchCommandTextBuilder(TEntity Entity)
        {
            int patternID = Entity.EntitySet.EntityChangeTracker.GetPatternID(Entity);

            if (!BatchCommandTextBuilders.TryGetValue(patternID, out (string, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>>) commandTextBuilder))
            {
                Type typeEntity = typeof(TEntity);

                ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
                ParameterDefinition[] update_set_params = CommandParameter<TEntityContext, TEntity>.Modified(Entity).ModifiedProperties.Definitions;

                #region Generate Command Fix Part

                // Generate Command Fix Part

                string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

                StringBuilder builder = new StringBuilder();

                builder.Append($"execute sp_executesql");

                builder.AppendLine()
                       .Append($"       N'");

                builder.Append($"set nocount on;");

                builder.AppendLine()
                       .Append($"         update {tableName}");

                builder.AppendLine()
                       .Append("           set ");

                int index = update_set_params.Length;

                foreach (ParameterDefinition param in update_set_params)
                {
                    builder.Append($"{param.Name} = @{param.Name}");

                    if (--index > 0)
                        builder.Append(", ");
                }

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

                foreach (ParameterDefinition param in update_set_params)
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

                Expression[] expressions = new Expression[(update_set_params.Length + 1) * 2];

                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant($", @{key_param.Name} = "));
                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(key_param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key_param.Property.Name))));

                foreach (ParameterDefinition param in update_set_params)
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
            {
            }
#endif
                #endregion IL

                #endregion Generate Command Variable Part

                BatchCommandTextBuilders.Add(patternID, commandTextBuilder = (commandTextFixPart, getCommandTextVariablePart));
            }

            return commandTextBuilder;
        }

        internal override void Append(Entity Entity, SaveBatchCommand Batch)
        {
            TEntity entity = (TEntity)Entity;

            if (entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!entity.HasModified())
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} State Must Be Modified");

            Dictionary<string, ParameterIdentity> param_identities = new Dictionary<string, ParameterIdentity>();

            ParameterDefinition[] update_set_params = CommandParameter<TEntityContext, TEntity>.Modified(entity).ModifiedProperties.Definitions;

            foreach (ParameterDefinition param in update_set_params)
                param_identities.Add(param.Property.Name, Batch.GetParameter(entity, param.Property));


            StringBuilder builder = new StringBuilder();

            (string CommandTextFixPart, Action<StringBuilder, TEntity, Dictionary<string, ParameterIdentity>> GetCommandTextVariablePart) commandTextBuilder = GetBatchCommandTextBuilder(entity);

            builder.Append(commandTextBuilder.CommandTextFixPart);

            commandTextBuilder.GetCommandTextVariablePart(builder, entity, param_identities);

            builder.Append(";");

            string text = builder.ToString();

            EntityCommandUpdate command = new EntityCommandUpdate(this, entity, text);

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

            entity.EntitySet.NotifyAllRelated(entity);

            entity.EntitySet.EntityChangeTracker.Remove(entity);
            entity.EntitySet.Context.ChangeTracker.Remove(entity); // this line of code must be executed first, before state change

            entity.State = EntityState.Unchanged;
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity> Write(TEntity Entity)
        {
            if (Entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!Entity.HasModified())
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Modified");

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

                Entity.EntitySet.NotifyAllRelated(Entity);

                Entity.EntitySet.EntityChangeTracker.Remove(Entity);
                Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                Entity.State = EntityState.Unchanged;
            }

            return this;
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity> Write(params TEntity[] Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity> Write(IEnumerable<TEntity> Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterUpdate<TEntityContext, TEntity> Write<TSource>(IEnumerable<TSource> Items, Func<TSource, TEntity> Predicate)
        {
            StringBuilder builder = new StringBuilder();

            foreach (TSource Item in Items)
            {
                TEntity Entity = Predicate(Item);

                if (Entity.IsNull())
                    throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

                if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

                if (!Entity.HasModified())
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Modified");

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

                            Entity.EntitySet.NotifyAllRelated(Entity);

                            Entity.EntitySet.EntityChangeTracker.Remove(Entity);
                            Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                            Entity.State = EntityState.Unchanged;

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
            return $"{typeof(TEntityContext).Name} Context, {typeof(TEntity).Name} Entity Database Writer Update";
        }
    }
}
