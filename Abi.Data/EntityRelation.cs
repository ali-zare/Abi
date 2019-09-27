using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Abi.Data
{
    internal sealed class EntityRelation<TForeignEntity, TRelatedEntity> : EntityRelation where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
        private EntityRelation()
        {
        }

        internal EntityRelation(EntityRelationType EntityRelationType, PropertyInfo PropForeignKey, PropertyInfo PropForeignReference, PropertyInfo PropRelated) : base(EntityRelationType, PropForeignKey, PropForeignReference, PropRelated)
        {
        }

        internal override Type ForeignType => typeof(TForeignEntity);
        internal override Type RelatedType => typeof(TRelatedEntity);

        private Func<EntityRelation<TForeignEntity, TRelatedEntity>, EntitySet<TForeignEntity>, EntitySet<TRelatedEntity>, IEntityRelationManager> entityRelationManagerCreator;

        internal override IEntityRelationManager CreateEntityRelationManager(EntitySet ForeignEntitySet, EntitySet RelatedEntitySet)
        {
            if (entityRelationManagerCreator != null)
                return entityRelationManagerCreator(this, (EntitySet<TForeignEntity>)ForeignEntitySet, (EntitySet<TRelatedEntity>)RelatedEntitySet);

            if (ForeignEntitySet.EntityType != ForeignType)
                throw new CriticalException($"EntityRelation<{ForeignType.Name}, {RelatedType.Name}>.CreateEntityRelationManager Foreign EntitySet {ForeignEntitySet.EntityType.Name} Type Is Not Equal To EntityRelation Foreign {ForeignType.Name} Type");

            if (RelatedEntitySet.EntityType != RelatedType)
                throw new CriticalException($"EntityRelation<{ForeignType.Name}, {RelatedType.Name}>.CreateEntityRelationManager Related EntitySet {RelatedEntitySet.EntityType.Name} Type Is Not Equal To EntityRelation Related {RelatedType.Name} Type");

            Type typeEntityRelationManager = null;

            if (EntityRelationType.IsOneToMany())
                typeEntityRelationManager = typeof(OneToManyEntityRelationManager<,,>).MakeGenericType(ForeignType, ForeignEntitySet.KeyType, RelatedType);

            else if (EntityRelationType.IsOneToOne())
                typeEntityRelationManager = typeof(OneToOneEntityRelationManager<,,>).MakeGenericType(ForeignType, ForeignEntitySet.KeyType, RelatedType);

            else
                throw new CriticalException($"EntityRelation<{ForeignType.Name}, {RelatedType.Name}>.CreateEntityRelationManager {EntityRelationType} EntityRelationType Is Not Supported");


            ConstructorInfo ctr = typeEntityRelationManager.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(EntityRelation<TForeignEntity, TRelatedEntity>), typeof(EntitySet<TForeignEntity>), typeof(EntitySet<TRelatedEntity>) }, null);

            ParameterExpression entityRelation = Expression.Parameter(typeof(EntityRelation<TForeignEntity, TRelatedEntity>), "EntityRelation");
            ParameterExpression foreignEntitySet = Expression.Parameter(typeof(EntitySet<TForeignEntity>), "ForeignEntitySet");
            ParameterExpression relatedEntitySet = Expression.Parameter(typeof(EntitySet<TRelatedEntity>), "RelatedEntitySet");

            entityRelationManagerCreator = Expression.Lambda<Func<EntityRelation<TForeignEntity, TRelatedEntity>, EntitySet<TForeignEntity>, EntitySet<TRelatedEntity>, IEntityRelationManager>>(Expression.New(ctr, entityRelation, foreignEntitySet, relatedEntitySet), entityRelation, foreignEntitySet, relatedEntitySet).Compile();

            return entityRelationManagerCreator(this, (EntitySet<TForeignEntity>)ForeignEntitySet, (EntitySet<TRelatedEntity>)RelatedEntitySet);
        }

    }

    internal abstract class EntityRelation
    {
        protected EntityRelation()
        {
        }
        protected EntityRelation(EntityRelationType EntityRelationType, PropertyInfo PropForeignKey, PropertyInfo PropForeignReference, PropertyInfo PropRelated)
        {
            this.EntityRelationType = EntityRelationType;
            this.PropForeignKey = PropForeignKey;
            this.PropForeignReference = PropForeignReference;
            this.PropRelated = PropRelated;
        }

        internal abstract Type ForeignType { get; }
        internal abstract Type RelatedType { get; }

        internal abstract IEntityRelationManager CreateEntityRelationManager(EntitySet ForeignEntitySet, EntitySet RelatedEntitySet);

        internal EntityRelationType EntityRelationType { get; private set; }
        internal PropertyInfo PropForeignKey { get; private set; }
        internal PropertyInfo PropForeignReference { get; private set; }
        internal PropertyInfo PropRelated { get; private set; }

        public override string ToString()
        {
            return $"{ForeignType.Name},{RelatedType.Name} Relation";
        }
    }
}
