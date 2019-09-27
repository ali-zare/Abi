using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseContextPreparedSelect<TEntityContext, TParameter> where TEntityContext : EntityContext<TEntityContext>
    {
        static EntityDatabaseContextPreparedSelect()
        {
            CommandTextBuilder = GetCommandTextBuilder();
        }
        internal EntityDatabaseContextPreparedSelect(EntityDatabaseConnection Connection, IsolationLevel Isolation, string CommandText, TParameter Parameter)
        {
            parameter = Parameter;
            isolation = Isolation;
            connection = Connection;
            commandText = CommandText;
        }


        private string commandText;
        private TParameter parameter;
        private IsolationLevel isolation;
        private EntityDatabaseConnection connection;
        private static (string CommandTextUpperFixPart, Action<StringBuilder, TParameter> GetCommandTextUpperVariablePart, Action<StringBuilder, TParameter> GetCommandTextBottomLeftVariablePart, Action<StringBuilder, TParameter> GetCommandTextBottomRightVariablePart) CommandTextBuilder;


        private static (string CommandTextUpperFixPart, Action<StringBuilder, TParameter> GetCommandTextUpperVariablePart, Action<StringBuilder, TParameter> GetCommandTextBottomLeftVariablePart, Action<StringBuilder, TParameter> GetCommandTextBottomRightVariablePart) GetCommandTextBuilder()
        {
            ParameterDefinition[] param_definitions = CommandParameter<TParameter>.VariableProperties.Definitions;

            int index = 0;
            StringBuilder builder = null;

            MethodInfo mthdAppend = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });
            MethodInfo mthdAppendLine = typeof(StringBuilder).GetMethod("AppendLine", new Type[] { });

            ParameterExpression sb = Expression.Parameter(typeof(StringBuilder), "sb");
            ParameterExpression p = Expression.Parameter(typeof(TParameter), "p");

            List<Expression> expressions = null;

            #region Generate Command Upper Fix Part

            // Generate Command Upper Fix Part

            builder = new StringBuilder();

            builder.Append($"execute sp_executesql");

            builder.AppendLine()
                   .Append($"       N'");

            string commandTextUpperFixPart = builder.ToString();

            #endregion Generate Command Upper Fix Part

            #region Generate Command Upper Variable Part

            // Generate Command Upper Variable Part

            #region Expression Tree
#if ET
            expressions = new List<Expression>();

            foreach (ParameterDefinition param_definition in param_definitions)
                if (param_definition.ParameterType == "UserDefinedTableType")
                {
                    MethodInfo mthdGetCommandText = param_definition.Property.PropertyType.GetMethod("GetCommandText", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);

                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Call(Expression.Call(p, param_definition.Property.GetGetMethod()), mthdGetCommandText, Expression.Constant(param_definition.Name))));
                    expressions.Add(Expression.Call(sb, mthdAppendLine));
                }

            Action<StringBuilder, TParameter> getCommandTextUpperVariablePart = (expressions.Count == 0) ? null : Expression.Lambda<Action<StringBuilder, TParameter>>(Expression.Block(expressions), sb, p).Compile();
#endif
            #endregion Expression Tree

            #region IL
#if IL
#endif
            #endregion IL

            #endregion Generate Command Upper Variable Part

            #region Generate Command Bottom Left Variable Part

            // Generate Command Bottom Left Variable Part

            expressions = new List<Expression>();

            builder = new StringBuilder();

            expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant("'")));
            expressions.Add(Expression.Call(sb, mthdAppendLine));
            expressions.Add(Expression.Call(sb, mthdAppendLine));
            expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant("     , N'")));

            index = param_definitions.Length;

            foreach (ParameterDefinition param_definition in param_definitions)
            {
                expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($"@{param_definition.Name} ")));

                if (param_definition.ParameterType == "UserDefinedTableType")
                {
                    MethodInfo propTableTypeNameGetMthd = param_definition.Property.PropertyType.GetProperty("TableTypeName").GetGetMethod();

                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Call(Expression.Call(p, param_definition.Property.GetGetMethod()), propTableTypeNameGetMthd)));
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($" readonly")));
                }
                else
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($"{param_definition.ParameterType}")));

                if (--index > 0)
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant(", ")));
            }

            expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant("'")));
            expressions.Add(Expression.Call(sb, mthdAppendLine));
            expressions.Add(Expression.Call(sb, mthdAppendLine));
            expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant("     ")));

            Action<StringBuilder, TParameter> getCommandTextBottomLeftVariablePart = Expression.Lambda<Action<StringBuilder, TParameter>>(Expression.Block(expressions), sb, p).Compile();

            #endregion Generate Command Bottom Left Variable Part

            #region Generate Command Bottom Right Variable Part

            // Generate Command Bottom Right Variable Part

            #region Expression Tree
#if ET
            expressions = new List<Expression>();

            foreach (ParameterDefinition param_definition in param_definitions)
                if (param_definition.ParameterType == "UserDefinedTableType")
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($", @{param_definition.Name} = @{param_definition.Name}")));
                else
                {
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($", @{param_definition.Name} = ")));
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Call(param_definition.Property.GetSetParameterValueMethod(), Expression.Call(p, param_definition.Property.GetGetMethod()))));
                }

            Action<StringBuilder, TParameter> getCommandTextBottomRightVariablePart = (expressions.Count == 0) ? null : Expression.Lambda<Action<StringBuilder, TParameter>>(Expression.Block(expressions), sb, p).Compile();
#endif
            #endregion Expression Tree

            #region IL
#if IL
#endif
            #endregion IL

            #endregion Generate Command Bottom Right Variable Part

            return (commandTextUpperFixPart, getCommandTextUpperVariablePart, getCommandTextBottomLeftVariablePart, getCommandTextBottomRightVariablePart);
        }


        public EntityDatabaseContextSelect<TEntityContext> To(TEntityContext context)
        {
            return new EntityDatabaseContextSelect<TEntityContext>(connection, isolation, GetCommandText(commandText, parameter), context);
        }


        private static string GetCommandText(string commandText, TParameter parameter)
        {
            StringBuilder builder = new StringBuilder();

            CommandTextBuilder.GetCommandTextUpperVariablePart?.Invoke(builder, parameter);

            builder.Append(CommandTextBuilder.CommandTextUpperFixPart);

            builder.Append(commandText.Replace("'", "''").CommandTextDecorator(9));

            CommandTextBuilder.GetCommandTextBottomLeftVariablePart(builder, parameter);

            CommandTextBuilder.GetCommandTextBottomRightVariablePart?.Invoke(builder, parameter);

            return builder.ToString();
        }
    }








    public sealed class EntityDatabasePreparedSelect<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal EntityDatabasePreparedSelect(EntityDatabaseConnection Connection, IsolationLevel Isolation, string CommandText)
        {
            isolation = Isolation;
            connection = Connection;
            commandText = CommandText;
        }


        private string commandText;
        private IsolationLevel isolation;
        private EntityDatabaseConnection connection;


        public EntityDatabaseContextPreparedSelect<TEntityContext, TParameter> WithParameters<TParameter>(TParameter parameter)
        {
            return new EntityDatabaseContextPreparedSelect<TEntityContext, TParameter>(connection, isolation, commandText, parameter);
        }
        public EntityDatabaseContextSelect<TEntityContext> To(TEntityContext context)
        {
            return new EntityDatabaseContextSelect<TEntityContext>(connection, isolation, GetCommandText(commandText), context);
        }


        private static string GetCommandText(string commandText)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"execute sp_executesql");

            builder.AppendLine()
                   .Append($"       N'");

            builder.Append(commandText.Replace("'", "''").CommandTextDecorator(9));

            builder.Append("'");

            return builder.ToString();
        }
    }








    public sealed class EntityDatabasePrepared<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal EntityDatabasePrepared(EntityDatabaseConnection Connection, IsolationLevel Isolation)
        {
            isolation = Isolation;
            connection = Connection;
        }


        private IsolationLevel isolation;
        private EntityDatabaseConnection connection;


        public EntityDatabasePreparedSelect<TEntityContext> Select(string commandText)
        {
            return new EntityDatabasePreparedSelect<TEntityContext>(connection, isolation, commandText);
        }
    }
}
