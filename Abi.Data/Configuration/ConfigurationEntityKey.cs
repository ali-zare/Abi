using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    public sealed class ConfigurationEntityKey<TEntityContext> : Configuration<TEntityContext>, IConfigurationEntityKey<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal ConfigurationEntityKey(EntityContextConfiguration<TEntityContext> EntityContextConfiguration) : base(EntityContextConfiguration)
        {
            internalManager = new Dictionary<Type, PropertyInfo>();
        }

        private Dictionary<Type, PropertyInfo> internalManager;

        public void Add<T>(Expression<Func<T, object>> expression)
        {
            Add(typeof(T), expression.GetPropInfo());
        }
        private void Add(Type EntityType, PropertyInfo PropEntityKey)
        {
            if (EntityContextConfiguration.CheckConfigured) throw new EntityContextConfigurationException($"Configuration Entity Key Add Failed, {EntityContextConfiguration.EntityContextType.Name} Configuration Already Is Configured, Can Not Add Any Entity Key After Configuration");

            if (!EntityContextConfiguration.Entities.Contains(EntityType)) throw new EntityContextConfigurationException($"Configuration Entity Key Add Failed, [{EntityType.Name}] Is Not Defined As Acceptable Type In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (EntityContextConfiguration.EntityTrackings.IsExcepted(PropEntityKey)) throw new EntityContextConfigurationException($"Configuration Entity Key Add Failed, [{EntityType.Name}.{PropEntityKey.Name}] Is Excepted From Tracking In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (!PropEntityKey.HasAcceptableKeyType()) throw new EntityContextConfigurationException($"Configuration Entity Key Add Failed, [{EntityType.Name}.{PropEntityKey.Name}] Type Is Not Acceptable");

            if (!internalManager.ContainsKey(EntityType))
                internalManager.Add(EntityType, PropEntityKey);
        }

        public PropertyInfo GetPropEntityKey(Type EntityType)
        {
            return internalManager[EntityType];
        }
        public bool Contains(Type EntityType)
        {
            return internalManager.ContainsKey(EntityType);
        }
        public bool Contains(PropertyInfo Property)
        {
            if (internalManager.TryGetValue(Property.DeclaringType, out PropertyInfo PropEntityKey))
                return PropEntityKey == Property;
            else
                return false;
        }


        #region Interface Implementation

        PropertyInfo IConfigurationEntityKey.GetPropEntityKey(Type Type)
        {
            return GetPropEntityKey(Type);
        }

        #endregion Interface Implementation
    }

    #region interface

    internal interface IConfigurationEntityKey<TEntityContext> : IConfigurationEntityKey where TEntityContext : EntityContext<TEntityContext>
    {
    }

    internal interface IConfigurationEntityKey
    {
        PropertyInfo GetPropEntityKey(Type EntityType);
    }

    #endregion interface
}
