using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    public sealed class ConfigurationEntityProperty<TEntityContext> : Configuration<TEntityContext>, IConfigurationEntityProperty<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal ConfigurationEntityProperty(EntityContextConfiguration<TEntityContext> EntityContextConfiguration) : base(EntityContextConfiguration)
        {
            internalManager = new Dictionary<Type, PropertyInfo>();
            definitions = new Dictionary<PropertyInfo, ColumnDefinition>();
        }

        private Dictionary<Type, PropertyInfo> internalManager;
        private Dictionary<PropertyInfo, ColumnDefinition> definitions;

        public void Add<T>(Expression<Func<T, object>> expression) => Add(typeof(T), expression.GetPropInfo());
        public void Add<T>(Expression<Func<T, object>> expression, SqlType? sqlType, int? size = null) => Add(typeof(T), expression.GetPropInfo(), null, sqlType, size);
        public void Add<T>(Expression<Func<T, object>> expression, string columnName, SqlType? sqlType = null, int? size = null) => Add(typeof(T), expression.GetPropInfo(), columnName, sqlType, size);

        private void Add(Type EntityType, PropertyInfo Property, string ColumnName = null, SqlType? SqlType = null, int? Size = null)
        {
            if (EntityContextConfiguration.CheckConfigured) throw new EntityContextConfigurationException($"Configuration Entity Property Add Failed, {EntityContextConfiguration.EntityContextType.Name} Configuration Already Is Configured, Can Not Add Any Entity Property After Configuration");

            if (!EntityContextConfiguration.Entities.Contains(EntityType)) throw new EntityContextConfigurationException($"Configuration Entity Property Add Failed, [{EntityType.Name}] Is Not Defined As Acceptable Type In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (EntityContextConfiguration.EntityTrackings.IsExcepted(Property)) throw new EntityContextConfigurationException($"Configuration Entity Property Add Failed, [{EntityType.Name}.{Property.Name}] Is Excepted From Tracking In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (!internalManager.ContainsKey(EntityType))
                internalManager.Add(EntityType, Property);

            if (ColumnName != null || SqlType != null || Size != null)
                definitions.Add(Property, new ColumnDefinition(Property, ColumnName, SqlType, Size));
        }

        internal ColumnDefinition? GetDefinition(PropertyInfo Property)
        {
            if (definitions.TryGetValue(Property, out ColumnDefinition definition))
                return definition;

            return null;
        }
    }

    #region interface

    internal interface IConfigurationEntityProperty<TEntityContext> : IConfigurationEntityProperty where TEntityContext : EntityContext<TEntityContext>
    {
    }

    internal interface IConfigurationEntityProperty
    {
    }

    #endregion interface
}
