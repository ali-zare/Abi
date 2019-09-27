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
    public sealed class EntityDatabaseWriterInsert<TEntityContext, TEntity, TEntityCallBack> : EntityDatabaseWriter where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseWriterInsert()
        {
            EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();

            Type typeEntity = typeof(TEntity);
            Type typeEntityCallBack = typeof(TEntityCallBack);

            PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);
            PropertyInfo[] entityCallBackProperties = typeEntityCallBack.GetEntityAcceptableDataProperties().ToArray();

            #region Generate CallBackData Setter

            #region Expression Tree
#if ET
            {
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

                Expression[] expressions = new Expression[(entityCallBackProperties.Length + 1) * 2];

                //key expressions

                expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key.Name)), Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(index++)));
                expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(key.Name));

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
            }
#endif
            #endregion Expression Tree

            #region IL
#if IL
            {
            }
#endif
            #endregion IL

            #endregion Generate CallBackData Setter
        }
        internal EntityDatabaseWriterInsert(EntityDatabaseConnection Connection) : base(Connection) { }


        private static Action<SqlDataReader, TEntity> SetCallBackData;
        private static (string CommandTextFixPart, Action<StringBuilder, TEntity> GetCommandTextVariablePart) CommandTextBuilder;
        private static (string CommandTextFixPart, Func<TEntity, Dictionary<string, ParameterIdentity>, List<string>> GetCommandTextVariablePart) BatchCommandTextBuilder;


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
            ParameterDefinition[] track_params = CommandParameter<TEntityContext, TEntity>.TrackableProperties.Definitions;
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
                   .Append($"         insert into {tableName} (");

            foreach (ParameterDefinition param in track_params)
                builder.Append($"{param.Name}, ");

            builder.Remove(builder.Length - 2, 2)
                   .Append(")");

            builder.AppendLine()
                   .Append(new string(' ', 21 + tableName.Length - 3))
                   .Append($"output ")
                   .Append("inserted.")
                   .Append(key_param.Name);

            foreach (ParameterDefinition param in calbck_params)
                builder.Append(", inserted.").Append($"{param.Name}");

            builder.AppendLine()
                   .Append(new string(' ', 21 + tableName.Length - 6))
                   .Append("values ")
                   .Append("(");

            foreach (ParameterDefinition param in track_params)
                builder.Append($"@{param.Name}, ");

            builder.Remove(builder.Length - 2, 2)
                   .Append(")");

            builder.Append(";");

            builder.Append("'");

            builder.AppendLine()
                   .AppendLine()
                   .Append("     , N'");

            int index = track_params.Length;

            foreach (ParameterDefinition param in track_params)
            {
                builder.Append($"@{param.Name} {param.ParameterType}");

                if (--index > 0)
                    builder.Append(", ");
            }

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

            Expression[] expressions = new Expression[track_params.Length * 2];

            foreach (ParameterDefinition param in track_params)
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
        private static (string, Func<TEntity, Dictionary<string, ParameterIdentity>, List<string>>) GetBatchCommandTextBuilder()
        {
            Type typeEntity = typeof(TEntity);

            ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
            ParameterDefinition[] track_params = CommandParameter<TEntityContext, TEntity>.TrackableProperties.Definitions;
            ParameterDefinition[] calbck_params = CommandParameter<TEntityContext, TEntity>.Derive<TEntityCallBack>().DerivativeProperties.Definitions;

            #region Generate Command Fix Part

            // Generate Command Fix Part

            string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

            StringBuilder builder = new StringBuilder();
            builder.Append($"         insert into {tableName} (");

            foreach (ParameterDefinition param in track_params)
                builder.Append($"{param.Name}, ");

            builder.Remove(builder.Length - 2, 2)
                   .Append(")");

            builder.AppendLine()
                   .Append(new string(' ', 21 + tableName.Length - 3))
                   .Append($"output ")
                   .Append("inserted.")
                   .Append(key_param.Name);

            foreach (ParameterDefinition param in calbck_params)
                builder.Append(", inserted.").Append($"{param.Name}");

            builder.AppendLine()
                   .Append(new string(' ', 21 + tableName.Length - 6))
                   .Append("values ")
                   .Append("(");

            foreach (ParameterDefinition param in track_params)
                builder.Append($"@{param.Name}, ");

            builder.Remove(builder.Length - 2, 2)
                   .Append(");");

            string commandTextFixPart = builder.ToString();

            #endregion Generate Command Fix Part

            #region Generate Command Variable Part

            // Generate Command Variable Part

            #region Expression Tree
#if ET
            int counter = 0;

            Type typeData = EntityProxy<TEntity>.DataType;
            Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

            ConstructorInfo ctrLst = typeof(List<string>).GetConstructor(new Type[] { });
            MethodInfo mthdAdd = typeof(List<string>).GetMethod("Add");
            MethodInfo mthdConcat = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
            MethodInfo propItemGetMthd = typeof(Dictionary<string, ParameterIdentity>).GetProperty("Item").GetGetMethod();
            MethodInfo propNameGetMthd = typeof(ParameterIdentity).GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            MethodInfo propIsDclGetMthd = typeof(ParameterIdentity).GetProperty("IsDeclarative", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            FieldInfo fldItem = typeProxyData.GetField("item");

            ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");
            ParameterExpression pi = Expression.Parameter(typeof(Dictionary<string, ParameterIdentity>), "pi");
            ParameterExpression lst = Expression.Parameter(typeof(List<string>), "lst");

            Expression[] expressions = new Expression[track_params.Length + 3];

            expressions[counter++] = Expression.Assign(lst, Expression.New(ctrLst));

            foreach (ParameterDefinition param in track_params)
            {
                Expression declarative_param_value = Expression.Call(mthdConcat, Expression.Constant($"@{param.Name} = ", typeof(string)), Expression.Call(Expression.Call(pi, propItemGetMthd, Expression.Constant(param.Property.Name)), propNameGetMthd));
                Expression non_declarative_param_value = Expression.Call(mthdConcat, Expression.Constant($"@{param.Name} = ", typeof(string)), Expression.Call(param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(param.Property.Name))));

                Expression list_add_declarative_param_value = Expression.Call(lst, mthdAdd, declarative_param_value);
                Expression list_add_non_declarative_param_value = Expression.Call(lst, mthdAdd, non_declarative_param_value);

                Expression is_declarative = Expression.Call(Expression.Call(pi, propItemGetMthd, Expression.Constant(param.Property.Name)), propIsDclGetMthd);

                expressions[counter++] = Expression.IfThenElse(is_declarative, list_add_declarative_param_value, list_add_non_declarative_param_value);
            }


            LabelTarget returnTarget = Expression.Label(typeof(List<string>));

            GotoExpression returnExpression = Expression.Return(returnTarget, lst, typeof(List<string>));

            LabelExpression returnLabel = Expression.Label(returnTarget, lst);

            expressions[counter++] = returnExpression;
            expressions[counter++] = returnLabel;

            Func<TEntity, Dictionary<string, ParameterIdentity>, List<string>> getCommandTextVariablePart = Expression.Lambda<Func<TEntity, Dictionary<string, ParameterIdentity>, List<string>>>(Expression.Block(new[] { lst }, expressions), e, pi).Compile();
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
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!entity.IsAdded())
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} State Must Be Added");

            Dictionary<string, ParameterIdentity> param_identities = new Dictionary<string, ParameterIdentity>();

            ParameterDefinition[] track_params = CommandParameter<TEntityContext, TEntity>.TrackableProperties.Definitions;

            foreach (ParameterDefinition param in track_params)
                param_identities.Add(param.Property.Name, Batch.GetParameter(entity, param.Property));



            if (BatchCommandTextBuilder == default)
                BatchCommandTextBuilder = GetBatchCommandTextBuilder();

            string first_query = BatchCommandTextBuilder.CommandTextFixPart;

            StringBuilder builder = new StringBuilder();

            int index = 0;

            foreach (ParameterDefinition param in track_params)
            {
                if (index++ > 0)
                    builder.Append(", ");

                builder.Append($"@{param.Name} {param.ParameterType}");
            }

            string parameter_definition = builder.ToString();

            string parameter_value = string.Join(", ", BatchCommandTextBuilder.GetCommandTextVariablePart(entity, param_identities));

            EntityCommandInsert command = new EntityCommandInsert(this, entity, first_query, parameter_definition, parameter_value);

            Batch.Add(command);
        }
        internal override void Append(Entity Entity, EntityCommand Command, ParameterIdentity Parameter)
        {
            ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;

            EntityCommandInsert command = (EntityCommandInsert)Command;

            string declare_key = $"declare {Parameter.Name} {key_param.ParameterType};";
            string second_query = $"         set {Parameter.Name} = scope_identity();";
            string parameter_definition = $"{Parameter.Name} {key_param.ParameterType} output";
            string parameter_value = $"{Parameter.Name} = {Parameter.Name} output";

            command.Change(declare_key, second_query, parameter_definition, parameter_value);
        }

        internal override void Write(Entity Entity)
        {
            Write((TEntity)Entity);
        }
        internal override void Write(Entity Entity, SaveBatchCommand Batch, SqlDataReader Reader)
        {
            TEntity entity = (TEntity)Entity;

            Reader.Read();

            entity.State |= EntityState.Busy;

            entity.EntitySet.EntityKeyManager.Remove(entity); // this line of code must be executed first, before key be setted with callback

            SetCallBackData(Reader, entity);

            entity.EntitySet.EntityKeyManager.Add(entity);

            entity.EntitySet.UpdatedAllRelated(entity);

            entity.EntitySet.Context.ChangeTracker.Remove(entity); // this line of code must be executed first, before state change

            entity.State = EntityState.Unchanged;
        }
        public EntityDatabaseWriterInsert<TEntityContext, TEntity, TEntityCallBack> Write(TEntity Entity)
        {
            if (Entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!Entity.IsAdded())
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Added");

            using (SqlCommand Command = Connection.GetCommand(GetCommandText(Entity)))
            {
                DatabaseTrace.Append(Command.CommandText, Constant.Query);

                if (Connection.HasTransaction)
                    Command.Transaction = Connection.Transaction;

                using (SqlDataReader Reader = Command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    Reader.Read();

                    Entity.State |= EntityState.Busy;

                    Entity.EntitySet.EntityKeyManager.Remove(Entity); // this line of code must be executed first, before key be setted with callback

                    SetCallBackData(Reader, Entity);

                    Entity.EntitySet.EntityKeyManager.Add(Entity);

                    Entity.EntitySet.UpdatedAllRelated(Entity);

                    Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                    Entity.State = EntityState.Unchanged;

                    Reader.Close();
                }
            }

            return this;
        }
        public EntityDatabaseWriterInsert<TEntityContext, TEntity, TEntityCallBack> Write(params TEntity[] Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterInsert<TEntityContext, TEntity, TEntityCallBack> Write(IEnumerable<TEntity> Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterInsert<TEntityContext, TEntity, TEntityCallBack> Write<TSource>(IEnumerable<TSource> Items, Func<TSource, TEntity> Predicate)
        {
            StringBuilder builder = new StringBuilder();

            foreach (TSource Item in Items)
            {
                TEntity Entity = Predicate(Item);

                if (Entity.IsNull())
                    throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

                if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

                if (!Entity.IsAdded())
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Added");

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
                    foreach (TSource Item in Items)
                    {
                        TEntity Entity = Predicate(Item);

                        Reader.Read();

                        Entity.State |= EntityState.Busy;

                        Entity.EntitySet.EntityKeyManager.Remove(Entity); // this line of code must be executed first, before key be setted with callback

                        SetCallBackData(Reader, Entity);

                        Entity.EntitySet.EntityKeyManager.Add(Entity);

                        Entity.EntitySet.UpdatedAllRelated(Entity);

                        Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                        Entity.State = EntityState.Unchanged;

                        Reader.NextResult();
                    }

                    Reader.Close();
                }
            }

            return this;
        }


        public override string ToString()
        {
            return $"{typeof(TEntityContext).Name} Context, {typeof(TEntity).Name} Entity Database Writer Insert";
        }
    }








    public sealed class EntityDatabaseWriterInsert<TEntityContext, TEntity> : EntityDatabaseWriter where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static EntityDatabaseWriterInsert()
        {
            EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();

            Type typeEntity = typeof(TEntity);

            PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);

            #region Generate CallBackData Setter

            #region Expression Tree
#if ET
            {
                Type typeData = EntityProxy<TEntity>.DataType;
                Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

                Type typeEntitySet = typeof(EntitySet<>).MakeGenericType(typeEntity);

                MethodInfo propEntitySetGetMthd = typeEntity.GetProperty("EntitySet", typeEntitySet).GetGetMethod(true);
                MethodInfo mthdOnPropChng = typeEntity.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                FieldInfo fldItem = typeProxyData.GetField("item");

                ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");
                ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

                Expression[] expressions = new Expression[2];

                //key expressions

                expressions[0] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(key.Name)), Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(0)));
                expressions[1] = Expression.Call(e, mthdOnPropChng, Expression.Constant(key.Name));

                SetCallBackData = Expression.Lambda<Action<SqlDataReader, TEntity>>(Expression.Block(expressions), dr, e).Compile();
            }
#endif
            #endregion Expression Tree

            #region IL
#if IL
            {
            }
#endif
            #endregion IL

            #endregion Generate CallBackData Setter
        }
        internal EntityDatabaseWriterInsert(EntityDatabaseConnection Connection) : base(Connection) { }


        private static Action<SqlDataReader, TEntity> SetCallBackData;
        private static (string CommandTextFixPart, Action<StringBuilder, TEntity> GetCommandTextVariablePart) CommandTextBuilder;
        private static (string CommandTextQueryFixPart, string CommandTextDeclareFixPart, Func<TEntity, Dictionary<string, ParameterIdentity>, List<string>> GetCommandTextVariablePart) BatchCommandTextBuilder;


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
            ParameterDefinition[] track_params = CommandParameter<TEntityContext, TEntity>.TrackableProperties.Definitions;

            #region Generate Command Fix Part

            // Generate Command Fix Part

            string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

            StringBuilder builder = new StringBuilder();

            builder.Append($"execute sp_executesql");

            builder.AppendLine()
                   .Append($"       N'");

            builder.Append($"set nocount on;");

            builder.AppendLine()
                   .Append($"         insert into {tableName} (");

            foreach (ParameterDefinition param in track_params)
                builder.Append($"{param.Name}, ");

            builder.Remove(builder.Length - 2, 2)
                   .Append(")");

            builder.AppendLine()
                   .Append(new string(' ', 21 + tableName.Length - 6))
                   .Append("values ")
                   .Append("(");

            foreach (ParameterDefinition param in track_params)
                builder.Append($"@{param.Name}, ");

            builder.Remove(builder.Length - 2, 2)
                   .Append(")");

            builder.Append(";");

            builder.AppendLine()
                   .AppendLine()
                   .Append($"         select cast(scope_identity() as {key_param.ParameterType});");

            builder.Append("'");

            builder.AppendLine()
                   .AppendLine()
                   .Append("     , N'");

            int index = track_params.Length;

            foreach (ParameterDefinition param in track_params)
            {
                builder.Append($"@{param.Name} {param.ParameterType}");

                if (--index > 0)
                    builder.Append(", ");
            }

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

            Expression[] expressions = new Expression[track_params.Length * 2];

            foreach (ParameterDefinition param in track_params)
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
        private static (string, string, Func<TEntity, Dictionary<string, ParameterIdentity>, List<string>>) GetBatchCommandTextBuilder()
        {
            Type typeEntity = typeof(TEntity);

            ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;
            ParameterDefinition[] track_params = CommandParameter<TEntityContext, TEntity>.TrackableProperties.Definitions;

            #region Generate Command Fix Part

            // Generate Command Fix Part

            string tableName = DatabaseTableDefinition<TEntityContext>.Default.GetTableName<TEntity>();

            StringBuilder builder = new StringBuilder();
            builder.Append($"         insert into {tableName} (");

            foreach (ParameterDefinition param in track_params)
                builder.Append($"{param.Name}, ");

            builder.Remove(builder.Length - 2, 2)
                   .Append(")");

            builder.AppendLine()
                   .Append(new string(' ', 21 + tableName.Length - 6))
                   .Append("values ")
                   .Append("(");

            foreach (ParameterDefinition param in track_params)
                builder.Append($"@{param.Name}, ");

            builder.Remove(builder.Length - 2, 2)
                   .Append(");");

            string commandTextFixPart = builder.ToString();

            string commandTextDeclareFixPart = $"         select cast(scope_identity() as {key_param.ParameterType});";

            #endregion Generate Command Fix Part

            #region Generate Command Variable Part

            // Generate Command Variable Part

            #region Expression Tree
#if ET
            int counter = 0;

            Type typeData = EntityProxy<TEntity>.DataType;
            Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

            ConstructorInfo ctrLst = typeof(List<string>).GetConstructor(new Type[] { });
            MethodInfo mthdAdd = typeof(List<string>).GetMethod("Add");
            MethodInfo mthdConcat = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
            MethodInfo propItemGetMthd = typeof(Dictionary<string, ParameterIdentity>).GetProperty("Item").GetGetMethod();
            MethodInfo propNameGetMthd = typeof(ParameterIdentity).GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            MethodInfo propIsDclGetMthd = typeof(ParameterIdentity).GetProperty("IsDeclarative", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            FieldInfo fldItem = typeProxyData.GetField("item");

            ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");
            ParameterExpression pi = Expression.Parameter(typeof(Dictionary<string, ParameterIdentity>), "pi");
            ParameterExpression lst = Expression.Parameter(typeof(List<string>), "lst");

            Expression[] expressions = new Expression[track_params.Length + 3];

            expressions[counter++] = Expression.Assign(lst, Expression.New(ctrLst));

            foreach (ParameterDefinition param in track_params)
            {
                Expression declarative_param_value = Expression.Call(mthdConcat, Expression.Constant($"@{param.Name} = ", typeof(string)), Expression.Call(Expression.Call(pi, propItemGetMthd, Expression.Constant(param.Property.Name)), propNameGetMthd));
                Expression non_declarative_param_value = Expression.Call(mthdConcat, Expression.Constant($"@{param.Name} = ", typeof(string)), Expression.Call(param.Property.GetSetParameterValueMethod(), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(param.Property.Name))));

                Expression list_add_declarative_param_value = Expression.Call(lst, mthdAdd, declarative_param_value);
                Expression list_add_non_declarative_param_value = Expression.Call(lst, mthdAdd, non_declarative_param_value);

                Expression is_declarative = Expression.Call(Expression.Call(pi, propItemGetMthd, Expression.Constant(param.Property.Name)), propIsDclGetMthd);

                expressions[counter++] = Expression.IfThenElse(is_declarative, list_add_declarative_param_value, list_add_non_declarative_param_value);
            }


            LabelTarget returnTarget = Expression.Label(typeof(List<string>));

            GotoExpression returnExpression = Expression.Return(returnTarget, lst, typeof(List<string>));

            LabelExpression returnLabel = Expression.Label(returnTarget, lst);

            expressions[counter++] = returnExpression;
            expressions[counter++] = returnLabel;

            Func<TEntity, Dictionary<string, ParameterIdentity>, List<string>> getCommandTextVariablePart = Expression.Lambda<Func<TEntity, Dictionary<string, ParameterIdentity>, List<string>>>(Expression.Block(new[] { lst }, expressions), e, pi).Compile();
#endif
            #endregion Expression Tree

            #region IL
#if IL
#endif
            #endregion IL

            #endregion Generate Command Variable Part

            return (commandTextFixPart, commandTextDeclareFixPart, getCommandTextVariablePart);
        }

        internal override void Append(Entity Entity, SaveBatchCommand Batch)
        {
            TEntity entity = (TEntity)Entity;

            if (entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!entity.IsAdded())
                throw new EntityDatabaseException($"{this}, Write Failed, {entity} State Must Be Added");

            Dictionary<string, ParameterIdentity> param_identities = new Dictionary<string, ParameterIdentity>();

            ParameterDefinition[] track_params = CommandParameter<TEntityContext, TEntity>.TrackableProperties.Definitions;

            foreach (ParameterDefinition param in track_params)
                param_identities.Add(param.Property.Name, Batch.GetParameter(entity, param.Property));



            if (BatchCommandTextBuilder == default)
                BatchCommandTextBuilder = GetBatchCommandTextBuilder();

            string first_query = BatchCommandTextBuilder.CommandTextQueryFixPart;
            string second_query = BatchCommandTextBuilder.CommandTextDeclareFixPart;

            StringBuilder builder = new StringBuilder();

            int index = 0;

            foreach (ParameterDefinition param in track_params)
            {
                if (index++ > 0)
                    builder.Append(", ");

                builder.Append($"@{param.Name} {param.ParameterType}");
            }

            string parameter_definition = builder.ToString();

            string parameter_value = string.Join(", ", BatchCommandTextBuilder.GetCommandTextVariablePart(entity, param_identities));

            EntityCommandInsert command = new EntityCommandInsert(this, entity, first_query, parameter_definition, parameter_value, second_query);

            Batch.Add(command);
        }
        internal override void Append(Entity Entity, EntityCommand Command, ParameterIdentity Parameter)
        {
            ParameterDefinition key_param = CommandParameter<TEntityContext, TEntity>.KeyProperty.Definition;

            EntityCommandInsert command = (EntityCommandInsert)Command;

            string declare_key = $"declare {Parameter.Name} {key_param.ParameterType};";
            string second_query = $"         set {Parameter.Name} = scope_identity();\n         select {Parameter.Name};";
            string parameter_definition = $"{Parameter.Name} {key_param.ParameterType} output";
            string parameter_value = $"{Parameter.Name} = {Parameter.Name} output";

            command.Change(declare_key, second_query, parameter_definition, parameter_value);
        }

        internal override void Write(Entity Entity)
        {
            Write((TEntity)Entity);
        }
        internal override void Write(Entity Entity, SaveBatchCommand Batch, SqlDataReader Reader)
        {
            TEntity entity = (TEntity)Entity;

            Reader.Read();

            entity.State |= EntityState.Busy;

            entity.EntitySet.EntityKeyManager.Remove(entity); // this line of code must be executed first, before key be setted with callback

            SetCallBackData(Reader, entity);

            entity.EntitySet.EntityKeyManager.Add(entity);

            entity.EntitySet.UpdatedAllRelated(entity);

            entity.EntitySet.Context.ChangeTracker.Remove(entity); // this line of code must be executed first, before state change

            entity.State = EntityState.Unchanged;
        }
        public EntityDatabaseWriterInsert<TEntityContext, TEntity> Write(TEntity Entity)
        {
            if (Entity.IsNull())
                throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

            if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

            if (!Entity.IsAdded())
                throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Added");

            using (SqlCommand Command = Connection.GetCommand(GetCommandText(Entity)))
            {
                DatabaseTrace.Append(Command.CommandText, Constant.Query);

                if (Connection.HasTransaction)
                    Command.Transaction = Connection.Transaction;

                using (SqlDataReader Reader = Command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    Reader.Read();

                    Entity.State |= EntityState.Busy;

                    Entity.EntitySet.EntityKeyManager.Remove(Entity); // this line of code must be executed first, before key be setted with callback

                    SetCallBackData(Reader, Entity);

                    Entity.EntitySet.EntityKeyManager.Add(Entity);

                    Entity.EntitySet.UpdatedAllRelated(Entity);

                    Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                    Entity.State = EntityState.Unchanged;

                    Reader.Close();
                }
            }

            return this;
        }
        public EntityDatabaseWriterInsert<TEntityContext, TEntity> Write(params TEntity[] Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterInsert<TEntityContext, TEntity> Write(IEnumerable<TEntity> Entities)
        {
            return Write(Entities, e => e);
        }
        public EntityDatabaseWriterInsert<TEntityContext, TEntity> Write<TSource>(IEnumerable<TSource> Items, Func<TSource, TEntity> Predicate)
        {
            StringBuilder builder = new StringBuilder();

            foreach (TSource Item in Items)
            {
                TEntity Entity = Predicate(Item);

                if (Entity.IsNull())
                    throw new EntityDatabaseException($"{this}, Write Failed, Entity Argument Is Null");

                if (Entity.EntitySet.Context.ContextType != typeof(TEntityContext))
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} Context Must Be Type Of {typeof(TEntityContext)}");

                if (!Entity.IsAdded())
                    throw new EntityDatabaseException($"{this}, Write Failed, {Entity} State Must Be Added");

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
                    foreach (TSource Item in Items)
                    {
                        TEntity Entity = Predicate(Item);

                        Reader.Read();

                        Entity.State |= EntityState.Busy;

                        Entity.EntitySet.EntityKeyManager.Remove(Entity); // this line of code must be executed first, before key be setted with callback

                        SetCallBackData(Reader, Entity);

                        Entity.EntitySet.EntityKeyManager.Add(Entity);

                        Entity.EntitySet.UpdatedAllRelated(Entity);

                        Entity.EntitySet.Context.ChangeTracker.Remove(Entity); // this line of code must be executed first, before state change

                        Entity.State = EntityState.Unchanged;

                        Reader.NextResult();
                    }

                    Reader.Close();
                }
            }

            return this;
        }


        public override string ToString()
        {
            return $"{typeof(TEntityContext).Name} Context, {typeof(TEntity).Name} Entity Database Writer Insert";
        }
    }
}
