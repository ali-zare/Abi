using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    public sealed class ConfigurationEntityTracking<TEntityContext> : Configuration<TEntityContext>, IConfigurationEntityTracking<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal ConfigurationEntityTracking(EntityContextConfiguration<TEntityContext> EntityContextConfiguration) : base(EntityContextConfiguration)
        {
            exceptedProperties = new HashSet<PropertyInfo>();
            internalManager = new Dictionary<Type, List<PropertyInfo>>();
        }

        private HashSet<PropertyInfo> exceptedProperties;
        private Dictionary<Type, List<PropertyInfo>> internalManager;

        public void Except<T>(Expression<Func<T, object>> expression)
        {
            Except(typeof(T), expression.GetPropInfo());
        }
        private void Except(Type EntityType, PropertyInfo ExceptedProperty)
        {
            if (EntityContextConfiguration.CheckConfigured) throw new EntityContextConfigurationException($"Configuration Entity Tracking Except Property Failed, {EntityContextConfiguration.EntityContextType.Name} Configuration Already Is Configured, Can Not Except Any Property After Configuration");

            if (!EntityContextConfiguration.Entities.Contains(EntityType)) throw new EntityContextConfigurationException($"Configuration Entity Tracking Except Property Failed, [{EntityType.Name}] Is Not Defined As Acceptable Type In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (EntityContextConfiguration.EntityKeys.Contains(ExceptedProperty)) throw new EntityContextConfigurationException($"Configuration Entity Tracking Except Property Failed, [{EntityType.Name}.{ExceptedProperty.Name}] Is Defined As Key In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (!exceptedProperties.Contains(ExceptedProperty))
            {
                exceptedProperties.Add(ExceptedProperty);

                if (!internalManager.TryGetValue(EntityType, out List<PropertyInfo> Properties))
                    internalManager.Add(EntityType, Properties = new List<PropertyInfo>());

                Properties.Add(ExceptedProperty);
            }
        }

        public bool IsExcepted(PropertyInfo Property)
        {
            return exceptedProperties.Contains(Property);
        }
        public IEnumerable<PropertyInfo> GetExceptedProperties(Type EntityType)
        {
            IEnumerable<PropertyInfo> properties = internalManager.TryGetValue(EntityType, out List<PropertyInfo> Properties) ? Properties : Enumerable.Empty<PropertyInfo>();

            foreach (PropertyInfo property in properties)
                yield return property;
        }
    }

    #region interface

    internal interface IConfigurationEntityTracking<TEntityContext> : IConfigurationEntityTracking where TEntityContext : EntityContext<TEntityContext>
    {
    }

    internal interface IConfigurationEntityTracking
    {
    }

    #endregion interface
}
