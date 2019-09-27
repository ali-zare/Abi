namespace Abi.Data
{
    internal sealed class EntityUnique<TForeignEntity, TRelatedEntity> : EntityUnique<TRelatedEntity> where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
        internal EntityUnique()
        {
        }

        private TForeignEntity owner;
        private IOneToOneEntityRelationManager<TForeignEntity, TRelatedEntity> entityRelationManager;




        internal void InternalSet(TRelatedEntity RelatedEntity)
        {
            valueSet(RelatedEntity);
        }

        internal void Reset()
        {
            Entity = default;

            entityRelationManager = null;
        }
        internal void Initialize(TForeignEntity Owner, IOneToOneEntityRelationManager<TForeignEntity, TRelatedEntity> EntityRelationManager)
        {
            this.owner = Owner;

            entityRelationManager = EntityRelationManager;
        }

        private void valueSet(TRelatedEntity RelatedEntity)
        {
            Entity = RelatedEntity;

            owner.OnPropertyChanged(entityRelationManager.EntityRelation.PropRelated.Name);
        }

        public override void Set(TRelatedEntity RelatedEntity)
        {
            if (!IsInitialized)
                throw new EntityUniqueSetException($"[{this}] Set({RelatedEntity}) Failed, {this} Is Not Initialized");

            if (Entity == RelatedEntity)
                throw new EntityUniqueSetException($"[{this}] Set({RelatedEntity}) Failed, {RelatedEntity} Aleardy Exist");

            if (!Entity.IsNull())
                entityRelationManager.Remove(Entity); // after call remove, value must be set as null (by call valueSet(null)), but to setting value just for one time, do that after check related entity is null

            if (RelatedEntity.IsNull())
                valueSet(null); // value must be set as null, immediately after call relation manager remove, but to prevent from repeatedly call setValue(null), call setValue(null) here

            else if (entityRelationManager.Add(RelatedEntity, owner))
                valueSet(RelatedEntity);

        }




        private bool IsInitialized => entityRelationManager != null;

        public override string ToString()
        {
            return $"{owner} {(entityRelationManager == null ? $"{typeof(TRelatedEntity).Name} Unique" : entityRelationManager.EntityRelation.PropRelated.Name)}";
        }
    }








    public abstract class EntityUnique<TRelatedEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
        private protected EntityUnique()
        {
        }


        public TRelatedEntity Entity { get; private protected set; }


        public abstract void Set(TRelatedEntity RelatedEntity);
    }
}
