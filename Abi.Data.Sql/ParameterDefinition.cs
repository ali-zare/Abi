using System.Reflection;

namespace Abi.Data.Sql
{
    internal struct ParameterDefinition
    {
        internal ParameterDefinition(PropertyInfo property, string name, string parameterType)
        {
            Name = name;
            Property = property;
            ParameterType = parameterType;
        }


        internal string Name { get; }
        internal string ParameterType { get; }
        internal PropertyInfo Property { get; }


        public override string ToString()
        {
            return $"Property: {Property.Name} Type: {Property.PropertyType} Name: {Name} ParameterType: {ParameterType}";
        }


        #region Operator

        public static bool operator ==(ParameterDefinition pd1, ParameterDefinition pd2)
        {
            return pd1.Property == pd2.Property && pd1.Name == pd2.Name && pd1.ParameterType == pd2.ParameterType;
        }
        public static bool operator !=(ParameterDefinition pd1, ParameterDefinition pd2)
        {
            return !(pd1 == pd2);
        }

        #endregion Operator

        #region Equals & GetHashCode

        public override bool Equals(object obj)
        {
            if (obj is ParameterDefinition pd)
                return pd.Property == Property;

            return false;
        }

        public override int GetHashCode()
        {
            return Property.GetHashCode();
        }

        #endregion Equals & GetHashCode
    }
}
