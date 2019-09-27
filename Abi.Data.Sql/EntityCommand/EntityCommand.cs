using System;
using System.Text;

namespace Abi.Data.Sql
{
    internal abstract class EntityCommand
    {
        private protected EntityCommand(EntityDatabaseWriter Writer, Entity Entity)
        {
            entity = Entity ?? throw new ArgumentNullException($"Create EntityCommand Failed, Entity Is Null");
            writer = Writer ?? throw new ArgumentNullException($"Create EntityCommand For {Entity} Failed, Writer Is Null");
        }


        private protected Entity entity;
        private protected EntityDatabaseWriter writer;


        internal Entity Entity => entity;


        internal abstract void Append(StringBuilder Builder);
    }
}
