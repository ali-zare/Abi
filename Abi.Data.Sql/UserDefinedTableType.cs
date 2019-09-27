using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Abi.Data.Sql
{
    public sealed class UserDefinedTableType<TItem> : IEnumerable<TItem>
    {
        static UserDefinedTableType()
        {
            StaticCommandTextBuilder = GetStaticCommandTextBuilder();
        }
        public UserDefinedTableType(string tableTypeName)
        {
            TableTypeName = tableTypeName ?? typeof(TItem).Name;

            items = new List<TItem>();
        }


        private static (string CommandTextFixPart, Action<StringBuilder, TItem> GetCommandTextVariablePart) StaticCommandTextBuilder;
        private List<TItem> items;


        public string TableTypeName { get; }


        private static (string, Action<StringBuilder, TItem>) GetStaticCommandTextBuilder()
        {
            Type typeEntity = typeof(TItem);

            #region Generate Command Fix Part

            // Generate Command Fix Part

            StringBuilder builder = new StringBuilder();

            builder.Append("(");

            PropertyInfo[] properties = typeEntity.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
                builder.Append($"{property.Name}, ");

            builder.Remove(builder.Length - 2, 2)
                   .Append(")");

            string commandTextFixPart = builder.ToString();

            #endregion Generate Command Fix Part

            #region Generate Command Variable Part

            // Generate Command Variable Part

            #region Expression Tree
#if ET
            int counter = 0;

            MethodInfo mthdAppend = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });

            ParameterExpression sb = Expression.Parameter(typeof(StringBuilder), "sb");
            ParameterExpression i = Expression.Parameter(typeof(TItem), "i");

            Expression[] expressions = new Expression[(properties.Length * 2) - 1 + 2];

            expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant("("));

            foreach (PropertyInfo property in properties)
            {
                expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Call(property.GetSetParameterValueMethod(), Expression.Call(i, property.GetGetMethod())));

                if (counter < (properties.Length * 2) - 1)
                    expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant(", "));
            }

            expressions[counter++] = Expression.Call(sb, mthdAppend, Expression.Constant(")"));

            Action<StringBuilder, TItem> getCommandTextVariablePart = Expression.Lambda<Action<StringBuilder, TItem>>(Expression.Block(expressions), sb, i).Compile();
#endif
            #endregion Expression Tree

            #region IL
#if IL
#endif
            #endregion IL

            #endregion Generate Command Variable Part

            return (commandTextFixPart, getCommandTextVariablePart);
        }
        internal string GetCommandText(string parameterName)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append($"declare @{parameterName} {TableTypeName};")
                   .AppendLine()
                   .AppendLine();

            int chunk_count = items.Count / 1000; // microsoft sql server limit number of values in single insert command to 1000 items.
            int last_chunk_items = items.Count % 1000;

            for (int chunk_no = 0; chunk_no < chunk_count; chunk_no++)
            {
                int chunk_star = chunk_no * 1000;

                GetInsertCommandText(parameterName, builder, chunk_star, chunk_star + 1000);
            }

            if (last_chunk_items > 0)
            {
                int chunk_star = chunk_count * 1000;

                GetInsertCommandText(parameterName, builder, chunk_star, chunk_star + last_chunk_items);
            }

            return builder.ToString();
        }
        private void GetInsertCommandText(string parameterName, StringBuilder builder, int index, int count)
        {
            builder.Append($"insert into @{parameterName} ")
                   .Append(StaticCommandTextBuilder.CommandTextFixPart)
                   .AppendLine()
                   .Append(new string(' ', 13 + parameterName.Length - 6))
                   .Append("values ");

            bool first = true;

            for (int i = index; i < count; i++)
            {
                if (!first)
                    builder.AppendLine()
                           .Append(new string(' ', 13 + parameterName.Length - 1))
                           .Append(", ");

                StaticCommandTextBuilder.GetCommandTextVariablePart(builder, items[i]);

                first = false;
            }

            builder.Append(";");

            builder.AppendLine()
                   .AppendLine();
        }
        public UserDefinedTableType<TItem> Add(TItem item)
        {
            items.Add(item);

            return this;
        }


        #region IEnumerable

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (TItem item in items)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable
    }
}
