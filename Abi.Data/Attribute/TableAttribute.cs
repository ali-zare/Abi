using System;

namespace Abi.Data
{

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        private TableAttribute()
        {
        }
        public TableAttribute(string name)
        {
            Name = name;
        }
        public TableAttribute(string schema, string name = null)
        {
            Name = name;
            Schema = schema;
        }

        public string Name { get; }
        public string Schema { get; }
    }
}
