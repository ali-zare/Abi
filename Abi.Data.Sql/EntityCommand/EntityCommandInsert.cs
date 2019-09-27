using System;
using System.Text;

namespace Abi.Data.Sql
{
    internal class EntityCommandInsert : EntityCommand
    {
        internal EntityCommandInsert(EntityDatabaseWriter Writer, Entity Entity, string FirstQuery, string ParameterDefinition, string ParameterValue, string SecondQuery = null) : base(Writer, Entity)
        {
            first_query = FirstQuery ?? throw new ArgumentNullException($"Create EntityCommand For {Entity} Failed, FirstQuery Is Null");
            parameter_definition = ParameterDefinition ?? throw new ArgumentNullException($"Create EntityCommand For {Entity} Failed, ParameterDefinition Is Null");
            parameter_value = ParameterValue ?? throw new ArgumentNullException($"Create EntityCommand For {Entity} Failed, ParameterValue Is Null");

            second_query = SecondQuery;
        }


        private string declare_key;
        private string first_query;
        private string second_query;
        private string parameter_definition;
        private string parameter_value;


        internal void Change(ParameterIdentity Parameter) => writer.Append(entity, this, Parameter);
        internal void Change(string DeclareKey, string SecondQuery, string ParameterDefinition, string ParameterValue)
        {
            declare_key = DeclareKey ?? throw new ArgumentNullException($"Change {entity} EntityCommand Failed, DeclareKey Is Null");
            second_query = SecondQuery ?? throw new ArgumentNullException($"Change {entity} EntityCommand Failed, SecondQuery Is Null");

            if (parameter_definition != null)
                parameter_definition += ", ";

            parameter_definition += ParameterDefinition ?? throw new ArgumentNullException($"Change {entity} EntityCommand Failed, ParameterDefinition Is Null");

            if (parameter_value != null)
                parameter_value += ", ";

            parameter_value += ParameterValue ?? throw new ArgumentNullException($"Change {entity} EntityCommand Failed, ParameterValue Is Null");
        }


        internal override void Append(StringBuilder Builder)
        {
            if (declare_key != null)
            {
                Builder.Append(declare_key);

                Builder.AppendLine()
                       .AppendLine();
            }

            Builder.Append($"execute sp_executesql");

            Builder.AppendLine()
                   .Append($"       N'");

            Builder.Append($"set nocount on;");

            if (first_query != null)
            {
                Builder.AppendLine()
                       .AppendLine();

                Builder.Append(first_query);

                if (second_query != null)
                {
                    Builder.AppendLine()
                           .AppendLine();

                    Builder.Append(second_query);
                }
            }

            Builder.Append("'");

            Builder.AppendLine()
                   .AppendLine()
                   .Append("     , N'");

            if (parameter_definition != null)
                Builder.Append(parameter_definition);

            Builder.Append("'")
                   .AppendLine()
                   .AppendLine();

            if (parameter_value != null)
                Builder.Append("     , ").Append(parameter_value).Append(";");
        }
    }
}
