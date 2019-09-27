using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    internal struct CommandToken<TEntityContext, TEntity, TEntityCallBack> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static CommandToken()
        {
            typeEntity = typeof(TEntity);
            typeEntityCallBack = typeof(TEntityCallBack);

            entityCallBackProperties = typeEntityCallBack.GetProperties();
        }
        internal CommandToken(DataTable table_schema)
        {
            map = new int[entityCallBackProperties.Length];

            int index = 0;

            // important : definition in AllDerivativeProperties.Definitions has same order as property in entityCallBackProperties
            foreach (ParameterDefinition definition in CommandParameter<TEntityContext, TEntity>.Derive<TEntityCallBack>().AllDerivativeProperties.Definitions)
            {
                bool found = false;

                foreach (DataRow column_schema in table_schema.Rows)
                    if (definition.Name == (string)column_schema["ColumnName"])
                    {
                        map[index++] = (int)column_schema["ColumnOrdinal"];
                        found = true;
                        break;
                    }

                if (!found)
                    throw new EntityDatabaseException($"{typeEntity.Name} {definition.Property.Name} Not Found In SqlDataReader Table Schema");
            }

            int hash = 17;

            unchecked
            {
                foreach (int i in map)
                    hash = hash * 23 + i.GetHashCode();
            }

            hashcode = hash;
        }


        private static Type typeEntity;
        private static Type typeEntityCallBack;
        private static PropertyInfo[] entityCallBackProperties;

        private int[] map;
        private int hashcode;


        internal Action<SqlDataReader, EntitySet<TEntity>> Generate_Fill_Setter()
        {
            #region Expression Tree
#if ET
            {
                int counter = 0;
                int index = 0;

                Type typeEntitySet = typeof(EntitySet<TEntity>);
                Type typeData = EntityProxy<TEntity>.DataType;
                Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

                MethodInfo mthdAdd = typeEntitySet.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeEntity }, null);
                ConstructorInfo ctrEntity = typeEntity.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
                MethodInfo mthdOnPropChng = typeEntity.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                FieldInfo fldItem = typeProxyData.GetField("item");

                ParameterExpression es = Expression.Parameter(typeEntitySet, "es");
                ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");
                ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

                Expression[] expressions = new Expression[entityCallBackProperties.Length * 2 + 2];

                expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                foreach (PropertyInfo prop in entityCallBackProperties)
                {
                    expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index++])));
                    expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                }

                expressions[counter++] = Expression.Call(es, mthdAdd, e);

                return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(Expression.Block(new[] { e }, expressions), dr, es).Compile();
            }
#endif
            #endregion Expression Tree

            #region IL
#if IL
            {
            }
#endif
            #endregion IL
        }
        internal Action<SqlDataReader, EntitySet<TEntity>> Generate_Merge_Setter()
        {
            // logic : if query returned data haven't key column, therefore all query result be added as new entity,
            //         else if key manager find the key of returned entity with select command, then execute true_expressions, else execute false_expressions

            #region Expression Tree
#if ET
            {
                EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();
                PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);
                Type keyType = key.PropertyType;
                int key_index = 0;

                bool callback_contains_key_property = false;

                foreach (PropertyInfo prop in entityCallBackProperties)
                {
                    if (typeEntity.GetProperty(prop.Name) == key)
                    {
                        callback_contains_key_property = true;
                        break;
                    }
                    key_index++;
                }

                Type typeEntitySet = typeof(EntitySet<TEntity>);
                Type typeData = EntityProxy<TEntity>.DataType;
                Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

                MethodInfo mthdAdd = typeEntitySet.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeEntity }, null);
                ConstructorInfo ctrEntity = typeEntity.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
                MethodInfo mthdOnPropChng = typeEntity.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                FieldInfo fldItem = typeProxyData.GetField("item");

                ParameterExpression es = Expression.Parameter(typeEntitySet, "es");
                ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");
                ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

                if (callback_contains_key_property)
                {
                    Type typeKeyManager = typeof(EntityKeyManager<,>).MakeGenericType(new[] { typeEntity, keyType });

                    MethodInfo propKeyMngrGetMthd = typeEntitySet.GetProperty("EntityKeyManager", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                    MethodInfo mthdFind = typeKeyManager.GetMethod("Find", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { keyType }, null);

                    Expression[] true_expressions = new Expression[entityCallBackProperties.Length]; // found key in key manager
                    Expression[] false_expressions = new Expression[entityCallBackProperties.Length * 2 + 2]; // not found key in key manager

                    #region if key manager found callbacked key ....
                    {
                        int index = 0, counter = 0;

                        Type typeExtension = typeof(Extension);

                        MethodInfo mthdGetEntity = typeKeyManager.GetMethod("GetEntity", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { keyType }, null);
                        MethodInfo propStateGetMthd = typeEntity.GetProperty("State", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
                        MethodInfo propStateSetMthd = typeEntity.GetProperty("State", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
                        MethodInfo mthdIsMdfid = typeEntity.GetMethod("IsModified", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(PropertyInfo) }, null);
                        MethodInfo mthdBeginEdit = typeExtension.GetMethod("BeginEdit", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(EntityState) }, null);
                        MethodInfo mthdEndEdit = typeExtension.GetMethod("EndEdit", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(EntityState) }, null);

                        true_expressions[counter++] = Expression.Assign(e, Expression.Call(Expression.TypeAs(Expression.Call(es, propKeyMngrGetMthd), typeKeyManager), mthdGetEntity, Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[key_index]))));

                        foreach (PropertyInfo prop in entityCallBackProperties)
                        {
                            PropertyInfo propEntity = typeEntity.GetProperty(prop.Name);
                            EntityRelation entityRelation = configuration.EntityRelations.GetEntityRelation(propEntity);

                            if (propEntity == key)
                            {
                                index++;
                                continue;
                            }

                            Expression[] true_sub_expressions = new Expression[entityRelation == null ? 4 : 3];

                            true_sub_expressions[0] = Expression.Call(e, propStateSetMthd, Expression.Call(mthdBeginEdit, Expression.Call(e, propStateGetMthd)));

                            if (entityRelation == null)
                            {
                                true_sub_expressions[1] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index])));
                                true_sub_expressions[2] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                            }
                            else
                            {
                                Type typeEditedEntity = typeof(EditedEntity<,>).MakeGenericType(typeEntity, prop.PropertyType);
                                Type typeEditedProperty = typeof(EditedProperty<>).MakeGenericType(prop.PropertyType);

                                ConstructorInfo ctrEdtdProp = typeEditedProperty.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(PropertyInfo), prop.PropertyType, prop.PropertyType }, null);
                                ConstructorInfo ctrEdtdEnty = typeEditedEntity.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeEditedProperty, typeof(TrakMode) }, null);

                                Expression expNewEdtdProp = Expression.New(ctrEdtdProp, new Expression[] { Expression.Constant(propEntity), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index])) });
                                Expression expNewEdtdEnty = Expression.New(ctrEdtdEnty, new Expression[] { expNewEdtdProp, Expression.Constant(TrakMode.None) });

                                MethodInfo mthdCallBack = typeof(EntitySet<TEntity>).GetMethod("CallBack", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(prop.PropertyType);

                                true_sub_expressions[1] = Expression.Call(es, mthdCallBack, new[] { e, expNewEdtdEnty });
                            }

                            true_sub_expressions[true_sub_expressions.Length - 1] = Expression.Call(e, propStateSetMthd, Expression.Call(mthdEndEdit, Expression.Call(e, propStateGetMthd)));

                            Expression expPropNewAndCurntValueIsEqual = Expression.Equal(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index++])));
                            Expression expPropIsMdfid = Expression.Call(e, mthdIsMdfid, Expression.Constant(propEntity));

                            true_expressions[counter++] = Expression.IfThen(Expression.IsFalse(expPropNewAndCurntValueIsEqual), Expression.IfThen(Expression.IsFalse(expPropIsMdfid), Expression.Block(true_sub_expressions)));
                        }
                    }
                    #endregion if key manager found callbacked key ....

                    #region if key manager not found callbacked key ....
                    {
                        int index = 0, counter = 0;

                        false_expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                        foreach (PropertyInfo prop in entityCallBackProperties)
                        {
                            false_expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index++])));
                            false_expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                        }

                        false_expressions[counter++] = Expression.Call(es, mthdAdd, e);
                    }
                    #endregion if key manager not found callbacked key ....

                    Expression expIF = Expression.IfThenElse(Expression.Call(Expression.TypeAs(Expression.Call(es, propKeyMngrGetMthd), typeKeyManager), mthdFind, Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[key_index]))), Expression.Block(new[] { e }, true_expressions), Expression.Block(new[] { e }, false_expressions));

                    return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(expIF, dr, es).Compile();
                }
                else
                {
                    int index = 0, counter = 0;

                    Expression[] expressions = new Expression[entityCallBackProperties.Length * 2 + 2];

                    expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                    foreach (PropertyInfo prop in entityCallBackProperties)
                    {
                        expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index++])));
                        expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                    }

                    expressions[counter++] = Expression.Call(es, mthdAdd, e);

                    return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(Expression.Block(new[] { e }, expressions), dr, es).Compile();
                }
            }
#endif
            #endregion Expression Tree

            #region IL
#if IL
            {
            }
#endif
            #endregion IL
        }
        internal Action<SqlDataReader, EntitySet<TEntity>> Generate_Refresh_Setter()
        {
            // logic : if query returned data haven't key column, therefore all query result be added as new entity,
            //         else if key manager find the key of returned entity with select command, then execute true_expressions, else execute false_expressions

            #region Expression Tree
#if ET
            {
                EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();
                PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);
                Type keyType = key.PropertyType;
                int key_index = 0;

                bool callback_contains_key_property = false;

                foreach (PropertyInfo prop in entityCallBackProperties)
                {
                    if (typeEntity.GetProperty(prop.Name) == key)
                    {
                        callback_contains_key_property = true;
                        break;
                    }
                    key_index++;
                }

                Type typeEntitySet = typeof(EntitySet<TEntity>);
                Type typeData = EntityProxy<TEntity>.DataType;
                Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

                MethodInfo mthdAdd = typeEntitySet.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeEntity }, null);
                ConstructorInfo ctrEntity = typeEntity.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
                MethodInfo mthdOnPropChng = typeEntity.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                FieldInfo fldItem = typeProxyData.GetField("item");

                ParameterExpression es = Expression.Parameter(typeEntitySet, "es");
                ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");
                ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

                if (callback_contains_key_property)
                {
                    Type typeKeyManager = typeof(EntityKeyManager<,>).MakeGenericType(new[] { typeEntity, keyType });

                    MethodInfo propKeyMngrGetMthd = typeEntitySet.GetProperty("EntityKeyManager", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                    MethodInfo mthdFind = typeKeyManager.GetMethod("Find", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { keyType }, null);

                    Expression[] true_expressions = new Expression[entityCallBackProperties.Length]; // found key in key manager
                    Expression[] false_expressions = new Expression[entityCallBackProperties.Length * 2 + 2]; // not found key in key manager

                    #region if key manager found callbacked key ....
                    {
                        int index = 0, counter = 0;

                        Type typeExtension = typeof(Extension);
                        Type typeChangeTracker = typeof(EntityChangeTracker<>).MakeGenericType(new[] { typeEntity });

                        MethodInfo propChngTrkrGetMthd = typeEntitySet.GetProperty("EntityChangeTracker", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                        MethodInfo mthdUnTrack = typeChangeTracker.GetMethod("UnTrack", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeEntity, typeof(PropertyInfo) }, null);
                        MethodInfo mthdGetEntity = typeKeyManager.GetMethod("GetEntity", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { keyType }, null);
                        MethodInfo propStateGetMthd = typeEntity.GetProperty("State", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
                        MethodInfo propStateSetMthd = typeEntity.GetProperty("State", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
                        MethodInfo mthdBeginEdit = typeExtension.GetMethod("BeginEdit", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(EntityState) }, null);
                        MethodInfo mthdEndEdit = typeExtension.GetMethod("EndEdit", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(EntityState) }, null);

                        true_expressions[counter++] = Expression.Assign(e, Expression.Call(Expression.TypeAs(Expression.Call(es, propKeyMngrGetMthd), typeKeyManager), mthdGetEntity, Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[key_index]))));

                        foreach (PropertyInfo prop in entityCallBackProperties)
                        {
                            PropertyInfo propEntity = typeEntity.GetProperty(prop.Name);
                            EntityRelation entityRelation = configuration.EntityRelations.GetEntityRelation(propEntity);

                            if (propEntity == key)
                            {
                                index++;
                                continue;
                            }

                            Expression[] true_sub_expressions = new Expression[entityRelation == null ? 5 : 3];

                            true_sub_expressions[0] = Expression.Call(e, propStateSetMthd, Expression.Call(mthdBeginEdit, Expression.Call(e, propStateGetMthd)));

                            if (entityRelation == null)
                            {
                                true_sub_expressions[1] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index])));
                                true_sub_expressions[2] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                                true_sub_expressions[3] = Expression.Call(Expression.Call(es, propChngTrkrGetMthd), mthdUnTrack, e, Expression.Constant(propEntity));
                            }
                            else
                            {
                                Type typeEditedEntity = typeof(EditedEntity<,>).MakeGenericType(typeEntity, prop.PropertyType);
                                Type typeEditedProperty = typeof(EditedProperty<>).MakeGenericType(prop.PropertyType);

                                ConstructorInfo ctrEdtdProp = typeEditedProperty.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(PropertyInfo), prop.PropertyType, prop.PropertyType }, null);
                                ConstructorInfo ctrEdtdEnty = typeEditedEntity.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeEditedProperty, typeof(TrakMode) }, null);

                                Expression expNewEdtdProp = Expression.New(ctrEdtdProp, new Expression[] { Expression.Constant(propEntity), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index])) });
                                Expression expNewEdtdEnty = Expression.New(ctrEdtdEnty, new Expression[] { expNewEdtdProp, Expression.Constant(TrakMode.UnTrack) });

                                MethodInfo mthdCallBack = typeof(EntitySet<TEntity>).GetMethod("CallBack", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(prop.PropertyType);

                                true_sub_expressions[1] = Expression.Call(es, mthdCallBack, new[] { e, expNewEdtdEnty });
                            }

                            true_sub_expressions[true_sub_expressions.Length - 1] = Expression.Call(e, propStateSetMthd, Expression.Call(mthdEndEdit, Expression.Call(e, propStateGetMthd)));

                            Expression expPropNewAndCurntValueIsEqual = Expression.Equal(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index++])));
                            Expression expUntrack = Expression.Call(Expression.Call(es, propChngTrkrGetMthd), mthdUnTrack, e, Expression.Constant(propEntity));

                            true_expressions[counter++] = Expression.IfThenElse(Expression.IsFalse(expPropNewAndCurntValueIsEqual), Expression.Block(true_sub_expressions), expUntrack);
                        }
                    }
                    #endregion if key manager found callbacked key ....

                    #region if key manager not found callbacked key ....
                    {
                        int index = 0, counter = 0;

                        false_expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                        foreach (PropertyInfo prop in entityCallBackProperties)
                        {
                            false_expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index++])));
                            false_expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                        }

                        false_expressions[counter++] = Expression.Call(es, mthdAdd, e);
                    }
                    #endregion if key manager not found callbacked key ....

                    Expression expIF = Expression.IfThenElse(Expression.Call(Expression.TypeAs(Expression.Call(es, propKeyMngrGetMthd), typeKeyManager), mthdFind, Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[key_index]))), Expression.Block(new[] { e }, true_expressions), Expression.Block(new[] { e }, false_expressions));

                    return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(expIF, dr, es).Compile();
                }
                else
                {
                    int index = 0, counter = 0;

                    Expression[] expressions = new Expression[entityCallBackProperties.Length * 2 + 2];

                    expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                    foreach (PropertyInfo prop in entityCallBackProperties)
                    {
                        expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(map[index++])));
                        expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                    }

                    expressions[counter++] = Expression.Call(es, mthdAdd, e);

                    return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(Expression.Block(new[] { e }, expressions), dr, es).Compile();
                }
            }
#endif
            #endregion Expression Tree

            #region IL
#if IL
            {
            }
#endif
            #endregion IL
        }


        public override bool Equals(object obj)
        {
            if (!(obj is CommandToken<TEntityContext, TEntity, TEntityCallBack> token))
                return false;

            if (map.Length != token.map.Length)
                return false;

            int i = 0;
            while (i < map.Length)
            {
                if (map[i] != token.map[i])
                    return false;

                i++;
            }

            return true;
        }
        public override int GetHashCode()
        {
            return hashcode;
        }
    }








    internal struct CommandToken<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static CommandToken()
        {
            typeEntity = typeof(TEntity);
        }
        internal CommandToken(DataTable table_schema)
        {
            map = new List<(PropertyInfo, int)>();

            foreach (DataRow column_schema in table_schema.Rows)
                foreach (ParameterDefinition definition in CommandParameter<TEntityContext, TEntity>.AcceptableProperties.Definitions)
                    if (definition.Name == (string)column_schema["ColumnName"])
                    {
                        map.Add((definition.Property, (int)column_schema["ColumnOrdinal"]));
                        break;
                    }

            int hash = 17;

            unchecked
            {
                foreach ((PropertyInfo p, int i) in map)
                {
                    hash = hash * 23 + p.GetHashCode();
                    hash = hash * 23 + i.GetHashCode();
                }
            }

            hashcode = hash;
        }


        private static Type typeEntity;

        private List<(PropertyInfo, int)> map;
        private int hashcode;


        internal Action<SqlDataReader, EntitySet<TEntity>> Generate_Fill_Setter()
        {
            #region Expression Tree
#if ET
            {
                int counter = 0;

                Type typeEntitySet = typeof(EntitySet<TEntity>);
                Type typeData = EntityProxy<TEntity>.DataType;
                Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

                MethodInfo mthdAdd = typeEntitySet.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeEntity }, null);
                ConstructorInfo ctrEntity = typeEntity.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
                MethodInfo mthdOnPropChng = typeEntity.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                FieldInfo fldItem = typeProxyData.GetField("item");

                ParameterExpression es = Expression.Parameter(typeEntitySet, "es");
                ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");
                ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

                Expression[] expressions = new Expression[map.Count * 2 + 2];

                expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                foreach ((PropertyInfo prop, int index) in map)
                {
                    expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)));
                    expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                }

                expressions[counter++] = Expression.Call(es, mthdAdd, e);

                return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(Expression.Block(new[] { e }, expressions), dr, es).Compile();
            }
#endif
            #endregion Expression Tree

            #region IL
#if IL
            {
            }
#endif
            #endregion IL
        }
        internal Action<SqlDataReader, EntitySet<TEntity>> Generate_Merge_Setter()
        {
            // logic : if query returned data haven't key column, therefore all query result be added as new entity,
            //         else if key manager find the key of returned entity with select command, then execute true_expressions, else execute false_expressions

            #region Expression Tree
#if ET
            {
                EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();
                PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);
                Type keyType = key.PropertyType;
                int key_index = 0;

                bool callback_contains_key_property = false;

                foreach ((PropertyInfo prop, int index) in map)
                {
                    if (prop == key)
                    {
                        callback_contains_key_property = true;
                        key_index = index;
                        break;
                    }
                }

                Type typeEntitySet = typeof(EntitySet<TEntity>);
                Type typeData = EntityProxy<TEntity>.DataType;
                Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

                MethodInfo mthdAdd = typeEntitySet.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeEntity }, null);
                ConstructorInfo ctrEntity = typeEntity.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
                MethodInfo mthdOnPropChng = typeEntity.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                FieldInfo fldItem = typeProxyData.GetField("item");

                ParameterExpression es = Expression.Parameter(typeEntitySet, "es");
                ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");
                ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

                if (callback_contains_key_property)
                {
                    Type typeKeyManager = typeof(EntityKeyManager<,>).MakeGenericType(new[] { typeEntity, keyType });

                    MethodInfo propKeyMngrGetMthd = typeEntitySet.GetProperty("EntityKeyManager", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                    MethodInfo mthdFind = typeKeyManager.GetMethod("Find", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { keyType }, null);

                    Expression[] true_expressions = new Expression[map.Count]; // found key in key manager
                    Expression[] false_expressions = new Expression[map.Count * 2 + 2]; // not found key in key manager

                    #region if key manager found callbacked key ....
                    {
                        int counter = 0;

                        Type typeExtension = typeof(Extension);

                        MethodInfo mthdGetEntity = typeKeyManager.GetMethod("GetEntity", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { keyType }, null);
                        MethodInfo propStateGetMthd = typeEntity.GetProperty("State", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
                        MethodInfo propStateSetMthd = typeEntity.GetProperty("State", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
                        MethodInfo mthdIsMdfid = typeEntity.GetMethod("IsModified", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(PropertyInfo) }, null);
                        MethodInfo mthdBeginEdit = typeExtension.GetMethod("BeginEdit", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(EntityState) }, null);
                        MethodInfo mthdEndEdit = typeExtension.GetMethod("EndEdit", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(EntityState) }, null);

                        true_expressions[counter++] = Expression.Assign(e, Expression.Call(Expression.TypeAs(Expression.Call(es, propKeyMngrGetMthd), typeKeyManager), mthdGetEntity, Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(key_index))));

                        foreach ((PropertyInfo prop, int index) in map)
                        {
                            PropertyInfo propEntity = typeEntity.GetProperty(prop.Name);
                            EntityRelation entityRelation = configuration.EntityRelations.GetEntityRelation(propEntity);

                            if (propEntity == key)
                                continue;

                            Expression[] true_sub_expressions = new Expression[entityRelation == null ? 4 : 3];

                            true_sub_expressions[0] = Expression.Call(e, propStateSetMthd, Expression.Call(mthdBeginEdit, Expression.Call(e, propStateGetMthd)));

                            if (entityRelation == null)
                            {
                                true_sub_expressions[1] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)));
                                true_sub_expressions[2] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                            }
                            else
                            {
                                Type typeEditedEntity = typeof(EditedEntity<,>).MakeGenericType(typeEntity, prop.PropertyType);
                                Type typeEditedProperty = typeof(EditedProperty<>).MakeGenericType(prop.PropertyType);

                                ConstructorInfo ctrEdtdProp = typeEditedProperty.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(PropertyInfo), prop.PropertyType, prop.PropertyType }, null);
                                ConstructorInfo ctrEdtdEnty = typeEditedEntity.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeEditedProperty, typeof(TrakMode) }, null);

                                Expression expNewEdtdProp = Expression.New(ctrEdtdProp, new Expression[] { Expression.Constant(propEntity), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)) });
                                Expression expNewEdtdEnty = Expression.New(ctrEdtdEnty, new Expression[] { expNewEdtdProp, Expression.Constant(TrakMode.None) });

                                MethodInfo mthdCallBack = typeof(EntitySet<TEntity>).GetMethod("CallBack", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(prop.PropertyType);

                                true_sub_expressions[1] = Expression.Call(es, mthdCallBack, new[] { e, expNewEdtdEnty });
                            }

                            true_sub_expressions[true_sub_expressions.Length - 1] = Expression.Call(e, propStateSetMthd, Expression.Call(mthdEndEdit, Expression.Call(e, propStateGetMthd)));

                            Expression expPropNewAndCurntValueIsEqual = Expression.Equal(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)));
                            Expression expPropIsMdfid = Expression.Call(e, mthdIsMdfid, Expression.Constant(propEntity));

                            true_expressions[counter++] = Expression.IfThen(Expression.IsFalse(expPropNewAndCurntValueIsEqual), Expression.IfThen(Expression.IsFalse(expPropIsMdfid), Expression.Block(true_sub_expressions)));
                        }
                    }
                    #endregion if key manager found callbacked key ....

                    #region if key manager not found callbacked key ....
                    {
                        int counter = 0;

                        false_expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                        foreach ((PropertyInfo prop, int index) in map)
                        {
                            false_expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)));
                            false_expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                        }

                        false_expressions[counter++] = Expression.Call(es, mthdAdd, e);
                    }
                    #endregion if key manager not found callbacked key ....

                    Expression expIF = Expression.IfThenElse(Expression.Call(Expression.TypeAs(Expression.Call(es, propKeyMngrGetMthd), typeKeyManager), mthdFind, Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(key_index))), Expression.Block(new[] { e }, true_expressions), Expression.Block(new[] { e }, false_expressions));

                    return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(expIF, dr, es).Compile();
                }
                else
                {
                    int counter = 0;

                    Expression[] expressions = new Expression[map.Count * 2 + 2];

                    expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                    foreach ((PropertyInfo prop, int index) in map)
                    {
                        expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)));
                        expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                    }

                    expressions[counter++] = Expression.Call(es, mthdAdd, e);

                    return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(Expression.Block(new[] { e }, expressions), dr, es).Compile();
                }
            }
#endif
            #endregion Expression Tree

            #region IL
#if IL
            {
            }
#endif
            #endregion IL
        }
        internal Action<SqlDataReader, EntitySet<TEntity>> Generate_Refresh_Setter()
        {
            // logic : if query returned data haven't key column, therefore all query result be added as new entity,
            //         else if key manager find the key of returned entity with select command, then execute true_expressions, else execute false_expressions

            #region Expression Tree
#if ET
            {
                EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();
                PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);
                Type keyType = key.PropertyType;
                int key_index = 0;

                bool callback_contains_key_property = false;

                foreach ((PropertyInfo prop, int index) in map)
                {
                    if (typeEntity.GetProperty(prop.Name) == key)
                    {
                        callback_contains_key_property = true;
                        key_index = index;
                        break;
                    }
                }

                Type typeEntitySet = typeof(EntitySet<TEntity>);
                Type typeData = EntityProxy<TEntity>.DataType;
                Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

                MethodInfo mthdAdd = typeEntitySet.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeEntity }, null);
                ConstructorInfo ctrEntity = typeEntity.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
                MethodInfo mthdOnPropChng = typeEntity.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
                MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                FieldInfo fldItem = typeProxyData.GetField("item");

                ParameterExpression es = Expression.Parameter(typeEntitySet, "es");
                ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");
                ParameterExpression e = Expression.Parameter(typeof(TEntity), "e");

                if (callback_contains_key_property)
                {
                    Type typeKeyManager = typeof(EntityKeyManager<,>).MakeGenericType(new[] { typeEntity, keyType });

                    MethodInfo propKeyMngrGetMthd = typeEntitySet.GetProperty("EntityKeyManager", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                    MethodInfo mthdFind = typeKeyManager.GetMethod("Find", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { keyType }, null);

                    Expression[] true_expressions = new Expression[map.Count]; // found key in key manager
                    Expression[] false_expressions = new Expression[map.Count * 2 + 2]; // not found key in key manager

                    #region if key manager found callbacked key ....
                    {
                        int counter = 0;

                        Type typeExtension = typeof(Extension);
                        Type typeChangeTracker = typeof(EntityChangeTracker<>).MakeGenericType(new[] { typeEntity });

                        MethodInfo propChngTrkrGetMthd = typeEntitySet.GetProperty("EntityChangeTracker", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                        MethodInfo mthdUnTrack = typeChangeTracker.GetMethod("UnTrack", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeEntity, typeof(PropertyInfo) }, null);
                        MethodInfo mthdGetEntity = typeKeyManager.GetMethod("GetEntity", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { keyType }, null);
                        MethodInfo propStateGetMthd = typeEntity.GetProperty("State", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
                        MethodInfo propStateSetMthd = typeEntity.GetProperty("State", BindingFlags.Public | BindingFlags.Instance).GetSetMethod(true);
                        MethodInfo mthdBeginEdit = typeExtension.GetMethod("BeginEdit", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(EntityState) }, null);
                        MethodInfo mthdEndEdit = typeExtension.GetMethod("EndEdit", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(EntityState) }, null);

                        true_expressions[counter++] = Expression.Assign(e, Expression.Call(Expression.TypeAs(Expression.Call(es, propKeyMngrGetMthd), typeKeyManager), mthdGetEntity, Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(key_index))));

                        foreach ((PropertyInfo prop, int index) in map)
                        {
                            PropertyInfo propEntity = typeEntity.GetProperty(prop.Name);
                            EntityRelation entityRelation = configuration.EntityRelations.GetEntityRelation(propEntity);

                            if (propEntity == key)
                                continue;

                            Expression[] true_sub_expressions = new Expression[entityRelation == null ? 5 : 3];

                            true_sub_expressions[0] = Expression.Call(e, propStateSetMthd, Expression.Call(mthdBeginEdit, Expression.Call(e, propStateGetMthd)));

                            if (entityRelation == null)
                            {
                                true_sub_expressions[1] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)));
                                true_sub_expressions[2] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                                true_sub_expressions[3] = Expression.Call(Expression.Call(es, propChngTrkrGetMthd), mthdUnTrack, e, Expression.Constant(propEntity));
                            }
                            else
                            {
                                Type typeEditedEntity = typeof(EditedEntity<,>).MakeGenericType(typeEntity, prop.PropertyType);
                                Type typeEditedProperty = typeof(EditedProperty<>).MakeGenericType(prop.PropertyType);

                                ConstructorInfo ctrEdtdProp = typeEditedProperty.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(PropertyInfo), prop.PropertyType, prop.PropertyType }, null);
                                ConstructorInfo ctrEdtdEnty = typeEditedEntity.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeEditedProperty, typeof(TrakMode) }, null);

                                Expression expNewEdtdProp = Expression.New(ctrEdtdProp, new Expression[] { Expression.Constant(propEntity), Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)) });
                                Expression expNewEdtdEnty = Expression.New(ctrEdtdEnty, new Expression[] { expNewEdtdProp, Expression.Constant(TrakMode.UnTrack) });

                                MethodInfo mthdCallBack = typeof(EntitySet<TEntity>).GetMethod("CallBack", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(prop.PropertyType);

                                true_sub_expressions[1] = Expression.Call(es, mthdCallBack, new[] { e, expNewEdtdEnty });
                            }

                            true_sub_expressions[true_sub_expressions.Length - 1] = Expression.Call(e, propStateSetMthd, Expression.Call(mthdEndEdit, Expression.Call(e, propStateGetMthd)));

                            Expression expPropNewAndCurntValueIsEqual = Expression.Equal(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)));
                            Expression expUntrack = Expression.Call(Expression.Call(es, propChngTrkrGetMthd), mthdUnTrack, e, Expression.Constant(propEntity));

                            true_expressions[counter++] = Expression.IfThenElse(Expression.IsFalse(expPropNewAndCurntValueIsEqual), Expression.Block(true_sub_expressions), expUntrack);
                        }
                    }
                    #endregion if key manager found callbacked key ....

                    #region if key manager not found callbacked key ....
                    {
                        int counter = 0;

                        false_expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                        foreach ((PropertyInfo prop, int index) in map)
                        {
                            false_expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)));
                            false_expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                        }

                        false_expressions[counter++] = Expression.Call(es, mthdAdd, e);
                    }
                    #endregion if key manager not found callbacked key ....

                    Expression expIF = Expression.IfThenElse(Expression.Call(Expression.TypeAs(Expression.Call(es, propKeyMngrGetMthd), typeKeyManager), mthdFind, Expression.Call(key.GetDataReaderGetValueMethod(), dr, Expression.Constant(key_index))), Expression.Block(new[] { e }, true_expressions), Expression.Block(new[] { e }, false_expressions));

                    return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(expIF, dr, es).Compile();
                }
                else
                {
                    int counter = 0;

                    Expression[] expressions = new Expression[map.Count * 2 + 2];

                    expressions[counter++] = Expression.Assign(e, Expression.New(ctrEntity));

                    foreach ((PropertyInfo prop, int index) in map)
                    {
                        expressions[counter++] = Expression.Assign(Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(e, propDataGetMthd), typeProxyData), fldItem), typeData.GetField(prop.Name)), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index)));
                        expressions[counter++] = Expression.Call(e, mthdOnPropChng, Expression.Constant(prop.Name));
                    }

                    expressions[counter++] = Expression.Call(es, mthdAdd, e);

                    return Expression.Lambda<Action<SqlDataReader, EntitySet<TEntity>>>(Expression.Block(new[] { e }, expressions), dr, es).Compile();
                }
            }
#endif
            #endregion Expression Tree

            #region IL
#if IL
            {
            }
#endif
            #endregion IL
        }


        public override bool Equals(object obj)
        {
            if (!(obj is CommandToken<TEntityContext, TEntity> token))
                return false;

            if (map.Count != token.map.Count)
                return false;

            int i = 0;
            while (i < map.Count)
            {
                if (map[i].Item1 != token.map[i].Item1
                 || map[i].Item2 != token.map[i].Item2)
                    return false;

                i++;
            }

            return true;
        }
        public override int GetHashCode()
        {
            return hashcode;
        }
    }








    internal struct CommandToken<T>
    {
        static CommandToken()
        {
            typeItem = typeof(T);

            hasParameterlessConstructor = null != typeItem.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
        }
        internal CommandToken(DataTable table_schema)
        {
            map = new List<(PropertyInfo, int)>();

            foreach (DataRow column_schema in table_schema.Rows)
                foreach (ParameterDefinition definition in CommandParameter<T>.AcceptableProperties.Definitions)
                    if (definition.Name == (string)column_schema["ColumnName"])
                    {
                        map.Add((definition.Property, (int)column_schema["ColumnOrdinal"]));
                        break;
                    }

            int hash = 17;

            unchecked
            {
                foreach ((PropertyInfo p, int i) in map)
                {
                    hash = hash * 23 + p.GetHashCode();
                    hash = hash * 23 + i.GetHashCode();
                }
            }

            hashcode = hash;
        }


        private static Type typeItem;
        private static bool hasParameterlessConstructor;

        private int hashcode;
        private List<(PropertyInfo, int)> map;


        internal Func<SqlDataReader, T> GenerateCallBackDataSetter()
        {
            if (hasParameterlessConstructor) // Non Anonymous Types
            {
                ConstructorInfo ctr = typeItem.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);

                List<Expression> expressions = new List<Expression>();
                ParameterExpression expItem = Expression.Variable(typeof(T), "item");
                ParameterExpression expResult = Expression.Parameter(typeof(T), "result");

                #region Create Item

                #region Expression Tree
#if ET
                {
                    expressions.Add(Expression.Assign(expItem, Expression.New(ctr)));
                }
#endif
                #endregion Expression Tree

                #region IL
#if IL
            {
            }
#endif
                #endregion IL

                #endregion Create Item

                #region Generate CallBackData Setter

                #region Expression Tree
#if ET
                {
                    ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");

                    foreach ((PropertyInfo prop, int index) in map)
                        expressions.Add(Expression.Call(expItem, prop.GetSetMethod(), Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index))));

                    expressions.Add(Expression.Assign(expResult, expItem));

                    return Expression.Lambda<Func<SqlDataReader, T>>(Expression.Block(new[] { expItem, expResult }, expressions), dr).Compile();
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
            else
            {
                ConstructorInfo ctr = typeItem.GetConstructors()[0];
                ParameterInfo[] parameters = ctr.GetParameters();

                Expression[] args = new Expression[parameters.Length];
                ParameterExpression dr = Expression.Parameter(typeof(SqlDataReader), "dr");

                #region Create Item

                #region Expression Tree
#if ET
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        bool found = false;

                        foreach ((PropertyInfo prop, int index) in map)
                            if (prop.Name == parameters[i].Name)
                            {
                                args[i] = Expression.Call(prop.GetDataReaderGetValueMethod(), dr, Expression.Constant(index));
                                found = true;
                                break;
                            }

                        if (!found)
                            args[i] = Expression.Default(parameters[i].ParameterType);

                    }
                }
#endif
                #endregion Expression Tree

                #region IL
#if IL
            {
            }
#endif
                #endregion IL

                #endregion Create Item

                #region Generate CallBackData Setter

                #region Expression Tree
#if ET
                {
                    return Expression.Lambda<Func<SqlDataReader, T>>(Expression.New(ctr, args), dr).Compile();
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
        }


        public override bool Equals(object obj)
        {
            if (!(obj is CommandToken<T> token))
                return false;

            if (map.Count != token.map.Count)
                return false;

            int i = 0;
            while (i < map.Count)
            {
                if (map[i].Item1 != token.map[i].Item1
                 || map[i].Item2 != token.map[i].Item2)
                    return false;

                i++;
            }

            return true;
        }
        public override int GetHashCode()
        {
            return hashcode;
        }
    }
}
