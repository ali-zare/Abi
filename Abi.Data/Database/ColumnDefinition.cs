using System;
using System.Reflection;

namespace Abi.Data
{
    internal struct ColumnDefinition
    {
        internal ColumnDefinition(PropertyInfo property, string name, SqlType? sqlType, int? size)
        {
            Property = property;

            Size = size;
            Name = name;
            SqlType = sqlType;
        }

        internal PropertyInfo Property { get; }

        internal int? Size { get; }
        internal string Name { get; }
        internal SqlType? SqlType { get; }


        public override string ToString()
        {
            return $"Property: {Property.Name} Type: {Property.PropertyType} Name: {Name} SqlType: {SqlType}";
        }
    }
}
