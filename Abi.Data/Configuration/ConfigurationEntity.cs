using System;
using System.Collections.Generic;

namespace Abi.Data
{
    public sealed class ConfigurationEntity<TEntityContext> : Configuration<TEntityContext>, IConfigurationEntity<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal ConfigurationEntity(EntityContextConfiguration<TEntityContext> EntityContextConfiguration) : base(EntityContextConfiguration)
        {
            internalManager = new HashSet<Type>();
            definitions = new Dictionary<Type, TableDefinition>();
        }

        private HashSet<Type> internalManager;
        private Dictionary<Type, TableDefinition> definitions;

        public void Add<T>() => Add<T>(null, null);
        public void Add<T>(string TableName) => Add<T>(null, TableName);
        public void Add<T>(string SchemaName, string TableName = null)
        {
            if (EntityContextConfiguration.CheckConfigured) throw new EntityContextConfigurationException($"Configuration Entity Add Failed, {EntityContextConfiguration.EntityContextType.Name} Configuration Already Is Configured, Can Not Add Any Entity After Configuration");

            Type EntityType = typeof(T);

            if (internalManager.Contains(EntityType)) throw new EntityContextConfigurationException($"Configuration Entity Add Failed, [{EntityType.Name}] Already Is Defined As Acceptable Type In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            internalManager.Add(EntityType);

            if (SchemaName != null || TableName != null)
                definitions.Add(EntityType, new TableDefinition(EntityType, SchemaName, TableName));
        }

        public bool Contains(Type EntityType)
        {
            return internalManager.Contains(EntityType);
        }

        internal TableDefinition? GetDefinition(Type EntityType)
        {
            if (definitions.TryGetValue(EntityType, out TableDefinition definition))
                return definition;

            return null;
        }
    }

    #region interface

    internal interface IConfigurationEntity<TEntityContext> : IConfigurationEntity where TEntityContext : EntityContext<TEntityContext>
    {
    }

    internal interface IConfigurationEntity
    {
    }

    #endregion interface
}
