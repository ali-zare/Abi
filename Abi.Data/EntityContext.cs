using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace Abi.Data
{
    public abstract class EntityContext<TEntityContext> : EntityContext where TEntityContext : EntityContext<TEntityContext>
    {
        protected EntityContext()
        {
            base.Configuration = Configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();

            Initialize();
        }



        private void Initialize()
        {
            if (ConstructEntitySets == null)
                ConstructEntitySets = ConstructEntitySetsGenerator();

            EntitySet[] EntitySets = ConstructEntitySets((TEntityContext)this);

            foreach (EntitySet entitySet in EntitySets)
                entitySets.Add(entitySet.EntityType, entitySet);

            foreach (EntityRelation entityRelation in Configuration.EntityRelations.Relations())
                entityRelationManagers.Add(entityRelation, entityRelation.CreateEntityRelationManager(entitySets[entityRelation.ForeignType], entitySets[entityRelation.RelatedType]));

            foreach (EntitySet entitySet in EntitySets)
                entitySet.Initialize();
        }
        private static Func<TEntityContext, EntitySet[]> ConstructEntitySetsGenerator()
        {
            EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();

            int index = 0;

            PropertyInfo[] entitySetProperties = typeof(TEntityContext).GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(EntitySet))).ToArray();

            Expression[] expressions = new Expression[entitySetProperties.Length + 2];

            ParameterExpression argContext = Expression.Parameter(typeof(TEntityContext), "Context");
            ParameterExpression varEntitySets = Expression.Variable(typeof(EntitySet[]), "EntitySets");

            expressions[index++] = Expression.Assign(varEntitySets, Expression.Constant(new EntitySet[entitySetProperties.Length]));

            foreach (PropertyInfo propEntitySet in entitySetProperties)
            {
                Type typeEntity = propEntitySet.PropertyType.GenericTypeArguments[0];

                Type typeKey = configuration.EntityKeys.GetPropEntityKey(typeEntity).PropertyType;

                Type typeEntitySet = null;

                if (typeKey == typeof(short))
                    typeEntitySet = typeof(EntitySet16<>);
                else if (typeKey == typeof(int))
                    typeEntitySet = typeof(EntitySet32<>);
                else if (typeKey == typeof(long))
                    typeEntitySet = typeof(EntitySet64<>);
                else
                    throw new EntityContextConfigurationException("Configuration EntitySet Add Failed, This ORM Just Support Entity With Key Property Of Type short, int or long");

                ConstructorInfo ctrEntitySet = typeEntitySet.MakeGenericType(typeEntity).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(TEntityContext) }, null);

                MethodInfo propEntitySetSetMthd = propEntitySet.GetSetMethod();

                expressions[index] = Expression.Call(argContext, propEntitySetSetMthd, Expression.TypeAs(Expression.Assign(Expression.ArrayAccess(varEntitySets, Expression.Constant(index++ - 1)), Expression.New(ctrEntitySet, argContext)), propEntitySet.PropertyType));
            }

            expressions[index++] = varEntitySets;

            return Expression.Lambda<Func<TEntityContext, EntitySet[]>>(Expression.Block(new[] { varEntitySets }, expressions), argContext).Compile();
        }

        internal override IEnumerable<PropertyInfo> GetTrackExceptedProperties<TEntity>()
        {
            return Configuration.EntityTrackings.GetExceptedProperties(typeof(TEntity));
        }
        internal override bool IsExceptedToTrack(PropertyInfo Property)
        {
            return Configuration.EntityTrackings.IsExcepted(Property);
        }



        private static Func<TEntityContext, EntitySet[]> ConstructEntitySets { get; set; }
        public override Type ContextType => typeof(TEntityContext);
        public new EntityContextConfiguration<TEntityContext> Configuration { get; set; }



        public override string ToString()
        {
            return $"{typeof(TEntityContext).Name} EntityContext, EntitySet Count {entitySets.Count}";
        }
    }










    public abstract class EntityContext
    {
        internal EntityContext()
        {
            Changes = ChangeTracker = new EntityContextChangeTracker();

            entitySets = new Dictionary<Type, EntitySet>();
            entityRelationManagers = new Dictionary<EntityRelation, IEntityRelationManager>();
        }



        private protected Dictionary<Type, EntitySet> entitySets;
        private protected Dictionary<EntityRelation, IEntityRelationManager> entityRelationManagers;



        internal abstract bool IsExceptedToTrack(PropertyInfo Property);
        internal abstract IEnumerable<PropertyInfo> GetTrackExceptedProperties<TEntity>() where TEntity : Entity<TEntity>;

        internal void FailOnLock()
        {
            if (IsLocked)
                throw new EntityContextBusyException($"{ContextType.Name} Context Is Locked");
        }
        internal IEntityRelationManager GetEntityRelationManager(EntityRelation EntityRelation)
        {
            return entityRelationManagers[EntityRelation];
        }



        public abstract Type ContextType { get; }
        public EntityContextChanges Changes { get; }

        internal bool IsLocked { private get; set; }
        internal EntityContextChangeTracker ChangeTracker { get; }
        internal EntityContextConfiguration Configuration { get; private protected set; }

    }
}
