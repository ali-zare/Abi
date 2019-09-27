using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    public sealed class ConfigurationEntityRelation<TEntityContext> : Configuration<TEntityContext>, IConfigurationEntityRelation<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal ConfigurationEntityRelation(EntityContextConfiguration<TEntityContext> EntityContextConfiguration) : base(EntityContextConfiguration)
        {
            types = new HashSet<Type>();
            entityRelations = new List<EntityRelation>();
            props = new Dictionary<PropertyInfo, EntityRelation>();
            relations_Of_foreignTypes = new Dictionary<Type, HashSet<EntityRelation>>();
            relations_Of_relatedTypes = new Dictionary<Type, HashSet<EntityRelation>>();


            OneToOne = new ConfigurationEntityOneToOneRelation<TEntityContext>(this);
            OneToMany = new ConfigurationEntityOneToManyRelation<TEntityContext>(this);
        }


        private HashSet<Type> types;
        private List<EntityRelation> entityRelations;
        private Dictionary<PropertyInfo, EntityRelation> props;
        private Dictionary<Type, HashSet<EntityRelation>> relations_Of_foreignTypes;
        private Dictionary<Type, HashSet<EntityRelation>> relations_Of_relatedTypes;



        internal void Add<TForeignEntity, TRelatedEntity>(EntityRelationType EntityRelationType, Expression<Func<TRelatedEntity, object>> expPropForeignKey, Expression<Func<TRelatedEntity, object>> expPropForeignReference) where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity>
        {
            if (EntityContextConfiguration.CheckConfigured) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {EntityContextConfiguration.EntityContextType.Name} Configuration Already Is Configured, Can Not Add Any Entity Relation After Configuration");

            Type typeForeign = typeof(TForeignEntity);
            Type typeRelated = typeof(TRelatedEntity);

            PropertyInfo propForeignKey = expPropForeignKey.GetPropInfo();
            PropertyInfo propForeignReference = expPropForeignReference.GetPropInfo();

            if (props.ContainsKey(propForeignKey)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}.{propForeignKey.Name}] Foreign Key Property Already Exist");
            if (props.ContainsKey(propForeignReference)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}.{propForeignReference.Name}] Foreign Reference Property Already Exist");

            if (!EntityContextConfiguration.Entities.Contains(typeForeign)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeForeign.Name}] Is Not Defined As Acceptable Type In {EntityContextConfiguration.EntityContextType.Name} Configuration");
            if (!EntityContextConfiguration.Entities.Contains(typeRelated)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}] Is Not Defined As Acceptable Type In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (!EntityContextConfiguration.EntityKeys.Contains(typeForeign)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeForeign.Name}] Key Is Not Defined In {EntityContextConfiguration.EntityContextType.Name} Configuration");
            if (!EntityContextConfiguration.EntityKeys.Contains(typeRelated)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}] Key Is Not Defined In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (EntityContextConfiguration.EntityTrackings.IsExcepted(propForeignKey)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}.{propForeignKey.Name}] Is Excepted From Tracking In {EntityContextConfiguration.EntityContextType.Name} Configuration");
            if (EntityContextConfiguration.EntityTrackings.IsExcepted(propForeignReference)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}.{propForeignReference.Name}] Is Excepted From Tracking In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (typeForeign != propForeignReference.PropertyType) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {propForeignReference.PropertyType.Name} Is Not Expected Type For [{propForeignReference.Name}] Foreign Property [{typeForeign.Name} Is Expected Type]");
            if (typeRelated != propForeignReference.DeclaringType) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {propForeignReference.DeclaringType.Name} Type Is Not Expected Owner For [{propForeignReference.Name}] Foreign Property [{typeRelated.Name} Type Is Expected Owner]");

            string getForeignKeyType() => (propForeignKey.PropertyType.IsGenericType
                                        && propForeignKey.PropertyType.IsGenericTypeDefinition == false
                                        && propForeignKey.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? $"Nullable<{propForeignKey.PropertyType.GenericTypeArguments[0].Name}>" : $"{propForeignKey.PropertyType.Name}";

            PropertyInfo propForeignEntityKey = EntityContextConfiguration.EntityKeys.GetPropEntityKey(typeForeign);

            if (!propForeignKey.HasSameTypeAs(propForeignEntityKey)
             && !propForeignKey.HasSameTypeAsNullable(propForeignEntityKey)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [({getForeignKeyType()}){typeRelated.Name}.{propForeignKey.Name}] Type Is Not Equal To [({propForeignEntityKey.PropertyType.Name}){typeForeign.Name}.{propForeignEntityKey.Name}] Or Nullable Of That Type");

            EntityRelation relation = new EntityRelation<TForeignEntity, TRelatedEntity>(EntityRelationType, propForeignKey, propForeignReference, null);

            entityRelations.Add(relation);

            if (!types.Contains(typeForeign))
                types.Add(typeForeign);

            if (!types.Contains(typeRelated))
                types.Add(typeRelated);

            props.Add(propForeignKey, relation);
            props.Add(propForeignReference, relation);

            if (!relations_Of_foreignTypes.TryGetValue(typeRelated, out HashSet<EntityRelation> foreignType_relations))
                relations_Of_foreignTypes.Add(typeRelated, foreignType_relations = new HashSet<EntityRelation>());
            foreignType_relations.Add(relation);

            if (!relations_Of_relatedTypes.TryGetValue(typeForeign, out HashSet<EntityRelation> relatedType_relations))
                relations_Of_relatedTypes.Add(typeForeign, relatedType_relations = new HashSet<EntityRelation>());
            relatedType_relations.Add(relation);
        }
        internal void Add<TForeignEntity, TRelatedEntity>(EntityRelationType EntityRelationType, Expression<Func<TRelatedEntity, object>> expPropForeignKey, Expression<Func<TRelatedEntity, object>> expPropForeignReference, Expression<Func<TForeignEntity, object>> expPropRelated) where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity>
        {
            if (EntityContextConfiguration.CheckConfigured) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {EntityContextConfiguration.EntityContextType.Name} Configuration Already Is Configured, Can Not Add Any Entity Relation After Configuration");

            Type typeForeign = typeof(TForeignEntity);
            Type typeRelated = typeof(TRelatedEntity);

            PropertyInfo propForeignKey = expPropForeignKey.GetPropInfo();
            PropertyInfo propForeignReference = expPropForeignReference.GetPropInfo();
            PropertyInfo propRelated = expPropRelated.GetPropInfo();

            if (props.ContainsKey(propForeignKey)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}.{propForeignKey.Name}] Foreign Key Property Already Exist");
            if (props.ContainsKey(propForeignReference)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}.{propForeignReference.Name}] Foreign Reference Property Already Exist");
            if (props.ContainsKey(propRelated)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {typeForeign.Name}.{propRelated.Name} Related Property Already Exist");

            if (!EntityContextConfiguration.Entities.Contains(typeForeign)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeForeign.Name}] Is Not Defined As Acceptable Type In {EntityContextConfiguration.EntityContextType.Name} Configuration");
            if (!EntityContextConfiguration.Entities.Contains(typeRelated)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}] Is Not Defined As Acceptable Type In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (!EntityContextConfiguration.EntityKeys.Contains(typeForeign)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeForeign.Name}] Key Is Not Defined In {EntityContextConfiguration.EntityContextType.Name} Configuration");
            if (!EntityContextConfiguration.EntityKeys.Contains(typeRelated)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}] Key Is Not Defined In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (EntityContextConfiguration.EntityTrackings.IsExcepted(propForeignKey)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}.{propForeignKey.Name}] Is Excepted From Tracking In {EntityContextConfiguration.EntityContextType.Name} Configuration");
            if (EntityContextConfiguration.EntityTrackings.IsExcepted(propForeignReference)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [{typeRelated.Name}.{propForeignReference.Name}] Is Excepted From Tracking In {EntityContextConfiguration.EntityContextType.Name} Configuration");

            if (typeForeign != propForeignReference.PropertyType) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {propForeignReference.PropertyType.Name} Is Not Expected Type For [{propForeignReference.Name}] Foreign Property [{typeForeign.Name} Is Expected Type]");
            if (typeRelated != propForeignReference.DeclaringType) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {propForeignReference.DeclaringType.Name} Type Is Not Expected Owner For [{propForeignReference.Name}] Foreign Property [{typeRelated.Name} Type Is Expected Owner]");

            string getForeignKeyType() => (propForeignKey.PropertyType.IsGenericType
                                        && propForeignKey.PropertyType.IsGenericTypeDefinition == false
                                        && propForeignKey.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? $"Nullable<{propForeignKey.PropertyType.GenericTypeArguments[0].Name}>" : $"{propForeignKey.PropertyType.Name}";

            PropertyInfo propForeignEntityKey = EntityContextConfiguration.EntityKeys.GetPropEntityKey(typeForeign);

            if (!propForeignKey.HasSameTypeAs(propForeignEntityKey)
             && !propForeignKey.HasSameTypeAsNullable(propForeignEntityKey)) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, [({getForeignKeyType()}){typeRelated.Name}.{propForeignKey.Name}] Type Is Not Equal To [({propForeignEntityKey.PropertyType.Name}){typeForeign.Name}.{propForeignEntityKey.Name}] Or Nullable Of That Type");

            if (typeForeign != propRelated.DeclaringType) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {propRelated.DeclaringType.Name} Type Is Not Expected Owner For [{propRelated.Name}] Related Property [{typeForeign.Name} Type Is Expected Owner]");


            if (EntityRelationType.IsOneToMany())
                if (propRelated.PropertyType.IsGenericType
                 && propRelated.PropertyType.IsGenericTypeDefinition == false
                 && propRelated.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>))
                {
                    string getCollectionName() => $"{typeForeign.Name}.{propRelated.Name}" + $"<{propRelated.PropertyType.GenericTypeArguments[0].Name}, " + $"{propRelated.PropertyType.GenericTypeArguments[1].Name}> Related Collection";

                    if (typeRelated != propRelated.PropertyType.GenericTypeArguments[0]) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, Related Type [{typeRelated.Name}] Is Not Equal To Second Argument Of {getCollectionName()}");
                }
                else
                    throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {typeForeign.Name}.{propRelated.Name} Must Be EntityCollection<{typeRelated}>");

            if (EntityRelationType.IsOneToOne())
                if (propRelated.PropertyType.IsGenericType
                    && propRelated.PropertyType.IsGenericTypeDefinition == false
                    && propRelated.PropertyType.GetGenericTypeDefinition() == typeof(EntityUnique<>))
                {
                    string getUniqueName() => $"{typeForeign.Name}.{propRelated.Name}" + $"<{propRelated.PropertyType.GenericTypeArguments[0].Name}, " + $"{propRelated.PropertyType.GenericTypeArguments[1].Name}> Related Collection";

                    if (typeRelated != propRelated.PropertyType.GenericTypeArguments[0]) throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, Related Type [{typeRelated.Name}] Is Not Equal To Second Argument Of {getUniqueName()}");
                }
                else
                    throw new EntityContextConfigurationException($"Configuration EntityRelation Add Failed, {typeForeign.Name}.{propRelated.Name} Must Be EntityUnique<{typeRelated}>");


            EntityRelation relation = new EntityRelation<TForeignEntity, TRelatedEntity>(EntityRelationType, propForeignKey, propForeignReference, propRelated);

            entityRelations.Add(relation);

            if (!types.Contains(typeForeign))
                types.Add(typeForeign);

            if (!types.Contains(typeRelated))
                types.Add(typeRelated);

            props.Add(propForeignKey, relation);
            props.Add(propForeignReference, relation);
            props.Add(propRelated, relation);

            if (!relations_Of_foreignTypes.TryGetValue(typeRelated, out HashSet<EntityRelation> foreignType_relations))
                relations_Of_foreignTypes.Add(typeRelated, foreignType_relations = new HashSet<EntityRelation>());
            foreignType_relations.Add(relation);

            if (!relations_Of_relatedTypes.TryGetValue(typeForeign, out HashSet<EntityRelation> relatedType_relations))
                relations_Of_relatedTypes.Add(typeForeign, relatedType_relations = new HashSet<EntityRelation>());
            relatedType_relations.Add(relation);
        }


        internal IEnumerable<EntityRelation> GetAllForeign(Type Type)
        {
            if (relations_Of_foreignTypes.TryGetValue(Type, out HashSet<EntityRelation> foreignType_relations))
                foreach (EntityRelation entityRelation in foreignType_relations)
                    yield return entityRelation;
        }
        internal IEnumerable<EntityRelation> GetAllRelated(Type Type)
        {
            if (relations_Of_relatedTypes.TryGetValue(Type, out HashSet<EntityRelation> relatedType_relations))
                foreach (EntityRelation entityRelation in relatedType_relations)
                    yield return entityRelation;
        }

        internal EntityRelation GetEntityRelation(PropertyInfo Property)
        {
            if (props.TryGetValue(Property, out EntityRelation relation))
                return relation;

            return null;
        }

        internal IEnumerable<Type> Types()
        {
            return types;
        }
        internal IEnumerable<EntityRelation> Relations()
        {
            foreach (EntityRelation entityRelation in entityRelations)
                yield return entityRelation;
        }

        public ConfigurationEntityOneToOneRelation<TEntityContext> OneToOne { get; }
        public ConfigurationEntityOneToManyRelation<TEntityContext> OneToMany { get; }


        #region Interface Implementation

        IEnumerable<EntityRelation> IConfigurationEntityRelation.GetAllForeign(Type Type)
        {
            return GetAllForeign(Type);
        }
        IEnumerable<EntityRelation> IConfigurationEntityRelation.GetAllRelated(Type Type)
        {
            return GetAllRelated(Type);
        }

        EntityRelation IConfigurationEntityRelation.GetEntityRelation(PropertyInfo Property)
        {
            return GetEntityRelation(Property);
        }

        #endregion Interface Implementation
    }

    public sealed class ConfigurationEntityOneToOneRelation<TContext> where TContext : EntityContext<TContext>
    {
        private ConfigurationEntityOneToOneRelation()
        {
        }
        internal ConfigurationEntityOneToOneRelation(ConfigurationEntityRelation<TContext> ConfigurationEntityRelation)
        {
            configurationEntityRelation = ConfigurationEntityRelation;
        }

        private ConfigurationEntityRelation<TContext> configurationEntityRelation;

        public void Add<TForeignEntity, TRelatedEntity>(Expression<Func<TRelatedEntity, object>> expPropForeignKey, Expression<Func<TRelatedEntity, object>> expPropForeignReference) where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity> => configurationEntityRelation.Add<TForeignEntity, TRelatedEntity>(EntityRelationType.OneToOne, expPropForeignKey, expPropForeignReference);
        public void Add<TForeignEntity, TRelatedEntity>(Expression<Func<TRelatedEntity, object>> expPropForeignKey, Expression<Func<TRelatedEntity, object>> expPropForeignReference, Expression<Func<TForeignEntity, object>> expPropRelated) where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity> => configurationEntityRelation.Add(EntityRelationType.OneToOne, expPropForeignKey, expPropForeignReference, expPropRelated);
    }

    public sealed class ConfigurationEntityOneToManyRelation<TContext> where TContext : EntityContext<TContext>
    {
        private ConfigurationEntityOneToManyRelation()
        {
        }
        internal ConfigurationEntityOneToManyRelation(ConfigurationEntityRelation<TContext> ConfigurationEntityRelation)
        {
            configurationEntityRelation = ConfigurationEntityRelation;
        }

        private ConfigurationEntityRelation<TContext> configurationEntityRelation;

        public void Add<TForeignEntity, TRelatedEntity>(Expression<Func<TRelatedEntity, object>> expPropForeignKey, Expression<Func<TRelatedEntity, object>> expPropForeignReference) where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity> => configurationEntityRelation.Add<TForeignEntity, TRelatedEntity>(EntityRelationType.OneToMany, expPropForeignKey, expPropForeignReference);
        public void Add<TForeignEntity, TRelatedEntity>(Expression<Func<TRelatedEntity, object>> expPropForeignKey, Expression<Func<TRelatedEntity, object>> expPropForeignReference, Expression<Func<TForeignEntity, object>> expPropRelated) where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity> => configurationEntityRelation.Add(EntityRelationType.OneToMany, expPropForeignKey, expPropForeignReference, expPropRelated);
    }

    #region interface

    internal interface IConfigurationEntityRelation<TEntityContext> : IConfigurationEntityRelation where TEntityContext : EntityContext<TEntityContext>
    {

    }

    internal interface IConfigurationEntityRelation
    {
        IEnumerable<EntityRelation> GetAllForeign(Type Type);
        IEnumerable<EntityRelation> GetAllRelated(Type Type);

        EntityRelation GetEntityRelation(PropertyInfo Property);
    }

    #endregion interface
}
