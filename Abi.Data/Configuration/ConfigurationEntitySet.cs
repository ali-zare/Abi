using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    public sealed class ConfigurationEntitySet<TEntityContext> : Configuration<TEntityContext>, IConfigurationEntitySet<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal ConfigurationEntitySet(EntityContextConfiguration<TEntityContext> EntityContextConfiguration) : base(EntityContextConfiguration)
        {
            internalManager = new Dictionary<Type, PropertyInfo>();
        }

        private Dictionary<Type, PropertyInfo> internalManager;

        public void Add<T>(Expression<Func<TEntityContext, EntitySet<T>>> expression) where T : Entity<T>
        {
            Add(typeof(T), expression.GetPropInfo());
        }
        internal void Add(Type EntityType, PropertyInfo PropEntitySet)
        {
            if (EntityContextConfiguration.CheckConfigured) throw new EntityContextConfigurationException($"Configuration EntitySet Add Failed, {EntityContextConfiguration.EntityContextType.Name} Configuration Already Is Configured, Can Not Add Any Entity Set After Configuration");

            if (!EntityContextConfiguration.Entities.Contains(EntityType)) throw new EntityContextConfigurationException($"Configuration EntitySet Add Failed, [{EntityType.Name}] Is Not Defined As Acceptable Type In {EntityContextConfiguration.EntityContextType.Name} Configuration");
            if (!EntityContextConfiguration.EntityKeys.Contains(EntityType)) throw new EntityContextConfigurationException($"Configuration EntitySet Add Failed, [{EntityType.Name}] Key Is Not Defined In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (!internalManager.ContainsKey(EntityType))
                internalManager.Add(EntityType, PropEntitySet);
        }


        public bool Contains(Type EntityType)
        {
            return internalManager.ContainsKey(EntityType);
        }
    }

    #region interface

    internal interface IConfigurationEntitySet<TEntityContext> : IConfigurationEntitySet where TEntityContext : EntityContext<TEntityContext>
    {
    }

    internal interface IConfigurationEntitySet
    {
    }

    #endregion interface
}