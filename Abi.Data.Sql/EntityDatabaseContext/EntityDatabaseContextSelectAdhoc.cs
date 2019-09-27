using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    public sealed class EntityDatabaseContextAdhocSelect<TEntityContext, TParameter> where TEntityContext : EntityContext<TEntityContext>
    {
        static EntityDatabaseContextAdhocSelect()
        {
            CommandTextBuilder = GetCommandTextBuilder();
        }
        internal EntityDatabaseContextAdhocSelect(EntityDatabaseConnection Connection, IsolationLevel Isolation, string CommandText, TParameter Parameter)
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
        private static Action<StringBuilder, TParameter> CommandTextBuilder;


        private static Action<StringBuilder, TParameter> GetCommandTextBuilder()
        {
            ParameterDefinition[] param_definitions = CommandParameter<TParameter>.VariableProperties.Definitions;

            MethodInfo mthdAppend = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });
            MethodInfo mthdAppendLine = typeof(StringBuilder).GetMethod("AppendLine", new Type[] { });

            ParameterExpression sb = Expression.Parameter(typeof(StringBuilder), "sb");
            ParameterExpression p = Expression.Parameter(typeof(TParameter), "p");

            List<Expression> expressions = null;

            #region Generate Command Variable Part

            // Generate Command Variable Part

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
                else
                {
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($"declare @{param_definition.Name} ")));
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($"{param_definition.ParameterType}")));
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($";")));
                    expressions.Add(Expression.Call(sb, mthdAppendLine));
                    expressions.Add(Expression.Call(sb, mthdAppendLine));
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($"set @{param_definition.Name} = ")));
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Call(param_definition.Property.GetSetParameterValueMethod(), Expression.Call(p, param_definition.Property.GetGetMethod()))));
                    expressions.Add(Expression.Call(sb, mthdAppend, Expression.Constant($";")));
                    expressions.Add(Expression.Call(sb, mthdAppendLine));
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

            return getCommandTextUpperVariablePart;
        }


        public EntityDatabaseContextSelect<TEntityContext> To(TEntityContext context)
        {
            return new EntityDatabaseContextSelect<TEntityContext>(connection, isolation, GetCommandText(commandText, parameter), context);
        }


        private static string GetCommandText(string commandText, TParameter parameter)
        {
            StringBuilder builder = new StringBuilder();

            CommandTextBuilder?.Invoke(builder, parameter);

            builder.Append(commandText.CommandTextDecorator(0));

            return builder.ToString();
        }
    }








    public sealed class EntityDatabaseAdhocSelect<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal EntityDatabaseAdhocSelect(EntityDatabaseConnection Connection, IsolationLevel Isolation, string CommandText)
        {
            isolation = Isolation;
            connection = Connection;
            commandText = CommandText;
        }


        private string commandText;
        private IsolationLevel isolation;
        private EntityDatabaseConnection connection;


        public EntityDatabaseContextAdhocSelect<TEntityContext, TParameter> WithParameters<TParameter>(TParameter parameter)
        {
            return new EntityDatabaseContextAdhocSelect<TEntityContext, TParameter>(connection, isolation, commandText, parameter);
        }
        public EntityDatabaseContextSelect<TEntityContext> To(TEntityContext context)
        {
            return new EntityDatabaseContextSelect<TEntityContext>(connection, isolation, GetCommandText(commandText), context);
        }


        private static string GetCommandText(string commandText)
        {
            return commandText.CommandTextDecorator(0);
        }
    }








    public sealed class EntityDatabaseAdhoc<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal EntityDatabaseAdhoc(EntityDatabaseConnection Connection, IsolationLevel Isolation)
        {
            isolation = Isolation;
            connection = Connection;
        }


        private IsolationLevel isolation;
        private EntityDatabaseConnection connection;


        public EntityDatabaseAdhocSelect<TEntityContext> Select(string commandText)
        {
            return new EntityDatabaseAdhocSelect<TEntityContext>(connection, isolation, commandText);
        }
    }
}
