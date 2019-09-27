using System;
using System.Reflection;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    internal class DatabaseTableDefinition<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        static DatabaseTableDefinition()
        {
            Default = new DatabaseTableDefinition<TEntityContext>();
        }
        internal DatabaseTableDefinition()
        {
            definitions = new Dictionary<Type, string>();
        }


        private Dictionary<Type, string> definitions;


        internal static DatabaseTableDefinition<TEntityContext> Default { get; }
        internal static EntityContextConfiguration<TEntityContext> Configuration => EntityContextConfiguration.GetConfiguration<TEntityContext>();


        internal string GetTableName<TEntity>() where TEntity : Entity<TEntity>
        {
            Type typeEntity = typeof(TEntity);

            string schemaName = null;
            string tableName = null;

            if (!definitions.TryGetValue(typeEntity, out string name))
            {
                TableDefinition? table = Configuration.Entities.GetDefinition(typeEntity);

                if (table != null)
                {
                    tableName = table.Value.Name;
                    schemaName = table.Value.Schema;
                }
                else
                {
                    TableAttribute attribute = typeEntity.GetCustomAttribute<TableAttribute>();

                    if (attribute != null)
                    {
                        tableName = attribute.Name;
                        schemaName = attribute.Schema;
                    }
                }

                if (schemaName == null)
                    schemaName = "dbo";

                if (tableName == null)
                    tableName = typeEntity.Name;

                definitions.Add(typeEntity, name = $"{schemaName}.{tableName}");
            }

            return name;
        }
    }








    internal class DatabaseTableDefinition
    {
        static DatabaseTableDefinition()
        {
            Default = new DatabaseTableDefinition();
        }
        internal DatabaseTableDefinition()
        {
            definitions = new Dictionary<Type, string>();
        }


        private Dictionary<Type, string> definitions;


        internal static DatabaseTableDefinition Default { get; }


        internal string GetTableName<T>()
        {
            Type type = typeof(T);

            string schemaName = null;
            string tableName = null;

            if (!definitions.TryGetValue(type, out string name))
            {
                TableAttribute attribute = type.GetCustomAttribute<TableAttribute>();

                if (attribute != null)
                {
                    tableName = attribute.Name;
                    schemaName = attribute.Schema;
                }

                if (schemaName == null)
                    schemaName = "dbo";

                if (tableName == null)
                    tableName = type.Name;

                definitions.Add(type, name = $"{schemaName}.{tableName}");
            }

            return name;
        }
    }
}
