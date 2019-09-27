using System;

namespace Abi.Data
{
    internal struct TableDefinition
    {
        internal TableDefinition(Type entityType, string schema, string name)
        {
            EntityType = entityType;

            Name = name;
            Schema = schema;
        }

        internal Type EntityType { get; }

        internal string Name { get; }
        internal string Schema { get; }
    }
}
