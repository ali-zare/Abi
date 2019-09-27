using System.Reflection;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    internal class DatabaseParameterDefinition<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        static DatabaseParameterDefinition()
        {
            Default = new DatabaseParameterDefinition<TEntityContext>();
        }
        internal DatabaseParameterDefinition()
        {
            definitions = new Dictionary<PropertyInfo, ParameterDefinition>();
        }


        private Dictionary<PropertyInfo, ParameterDefinition> definitions;


        internal static DatabaseParameterDefinition<TEntityContext> Default { get; }
        internal static EntityContextConfiguration<TEntityContext> Configuration => EntityContextConfiguration.GetConfiguration<TEntityContext>();


        internal ParameterDefinition GetParameterDefinition(PropertyInfo Property)
        {
            int? size = null;
            string colName = null;
            SqlType? sqlType = null;

            if (!definitions.TryGetValue(Property, out ParameterDefinition definition))
            {
                ColumnDefinition? column = Configuration.EntityProperties.GetDefinition(Property);

                if (column != null)
                {
                    size = column.Value.Size;
                    colName = column.Value.Name;
                    sqlType = column.Value.SqlType;
                }
                else
                {
                    ColumnAttribute attribute = Property.GetCustomAttribute<ColumnAttribute>();

                    if (attribute != null)
                    {
                        size = attribute.Size;
                        colName = attribute.Name;
                        sqlType = attribute.SqlType;
                    }
                }

                if (sqlType == null)
                    sqlType = Property.GetSqlType();

                if (colName == null)
                    colName = Property.Name;

                string Name = colName;
                SqlType ParameterSqlType = sqlType.Value;
                string ParameterType = ParameterSqlType.GetParameterType(size);

                definitions.Add(Property, definition = new ParameterDefinition(Property, Name, ParameterType));
            }

            return definition;
        }
    }








    internal class DatabaseParameterDefinition
    {
        static DatabaseParameterDefinition()
        {
            Default = new DatabaseParameterDefinition();
        }
        internal DatabaseParameterDefinition()
        {
            definitions = new Dictionary<PropertyInfo, ParameterDefinition>();
        }


        private Dictionary<PropertyInfo, ParameterDefinition> definitions;


        internal static DatabaseParameterDefinition Default { get; }


        internal ParameterDefinition GetParameterDefinition(PropertyInfo Property)
        {
            int? size = null;
            string colName = null;
            SqlType? sqlType = null;

            if (!definitions.TryGetValue(Property, out ParameterDefinition definition))
            {
                ColumnAttribute attribute = Property.GetCustomAttribute<ColumnAttribute>();

                if (attribute != null)
                {
                    size = attribute.Size;
                    colName = attribute.Name;
                    sqlType = attribute.SqlType;
                }

                if (sqlType == null)
                    sqlType = (Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(UserDefinedTableType<>)) ? SqlType.UserDefinedTableType : Property.GetSqlType();

                if (colName == null)
                    colName = Property.Name;

                string Name = colName;
                SqlType ParameterSqlType = sqlType.Value;
                string ParameterType = (ParameterSqlType == SqlType.UserDefinedTableType) ? "UserDefinedTableType" : ParameterSqlType.GetParameterType(size);

                definitions.Add(Property, definition = new ParameterDefinition(Property, Name, ParameterType));
            }

            return definition;
        }
    }
}
