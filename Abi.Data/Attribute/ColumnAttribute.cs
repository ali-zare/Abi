using System;

namespace Abi.Data
{

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public int? Size { get; set; }
        public string Name { get; set; }
        public SqlType? SqlType { get; set; }
    }
}
