namespace Abi.Data.Sql
{
    internal struct ParameterIdentity
    {
        internal ParameterIdentity(string name = null, bool isDeclarative = false)
        {
            Name = name;
            IsDeclarative = isDeclarative;
        }

        internal string Name { get; }
        internal bool IsDeclarative { get; }
    }
}
