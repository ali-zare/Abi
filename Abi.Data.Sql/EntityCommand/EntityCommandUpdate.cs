using System;
using System.Text;

namespace Abi.Data.Sql
{
    internal sealed class EntityCommandUpdate : EntityCommand
    {
        internal EntityCommandUpdate(EntityDatabaseWriter Writer, Entity Entity, string Text) : base(Writer, Entity)
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new ArgumentNullException($"Create EntityCommand For {Entity} Failed, Text Is Null Or Empty");

            text = Text;
        }


        private string text;


        internal override void Append(StringBuilder Builder) => Builder.Append(text);
    }
}
