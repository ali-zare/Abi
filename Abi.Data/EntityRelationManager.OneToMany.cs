using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    internal sealed class OneToManyEntityRelationManager<TForeignEntity, TForeignKey, TRelatedEntity> : EntityRelationManager<TForeignEntity, TForeignKey, TRelatedEntity>, IOneToManyEntityRelationManager<TForeignEntity, TRelatedEntity> where TForeignEntity : Entity<TForeignEntity> where TForeignKey : struct where TRelatedEntity : Entity<TRelatedEntity>
    {
        #region Constructors and Variables

        static OneToManyEntityRelationManager()
        {
            accessors = new Dictionary<EntityRelation, Func<TForeignEntity, EntityCollection<TForeignEntity, TRelatedEntity>>>();
        }
        internal OneToManyEntityRelationManager(EntityRelation<TForeignEntity, TRelatedEntity> EntityRelation, EntitySet<TForeignEntity> ForeignEntitySet, EntitySet<TRelatedEntity> RelatedEntitySet) : base(EntityRelation, ForeignEntitySet, RelatedEntitySet)
        {
            lateAdds = new Dictionary<TRelatedEntity, TForeignEntity>();

            #region Generate Property Setter And Getter

            if (propRelated != null)
            {
                if (!accessors.TryGetValue(EntityRelation, out Func<TForeignEntity, EntityCollection<TForeignEntity, TRelatedEntity>> accessor))
                {
                    Type typeForeignEntity = typeof(TForeignEntity);

                    MethodInfo propRelatedGetMthd = propRelated.GetGetMethod(true);

                    ParameterExpression argForeignEntity = Expression.Parameter(typeForeignEntity, "ForeignEntity");

                    accessors.Add(EntityRelation, accessor = Expression.Lambda<Func<TForeignEntity, EntityCollection<TForeignEntity, TRelatedEntity>>>(Expression.TypeAs(Expression.Call(argForeignEntity, propRelatedGetMthd), typeof(EntityCollection<TForeignEntity, TRelatedEntity>)), argForeignEntity).Compile());
                }

                getRColc = accessor;
            }

            #endregion Generate Property Settery And Getter

        }


        private static Dictionary<EntityRelation, Func<TForeignEntity, EntityCollection<TForeignEntity, TRelatedEntity>>> accessors;

        private Func<TForeignEntity, EntityCollection<TForeignEntity, TRelatedEntity>> getRColc;
        private Dictionary<TRelatedEntity, TForeignEntity> lateAdds;

        #endregion Constructors and Variables



        #region Foreign Manager

        private protected override void RelatedAdd(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityCollection<TForeignEntity, TRelatedEntity> relatedCollection = getRColc(ForeignEntity);
#if Test || Test1
                if (relatedCollection == null)
                    throw new TestException($"[{this}] RelatedAdd Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");
#endif
#if Test || Test3
                if (relatedCollection.Contains(RelatedEntity))
                    throw new TestException($"[{this}] RelatedAdd Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Contains {RelatedEntity}");
#endif
                relatedCollection.InternalAdd(RelatedEntity);
            }
        }
        private protected override void RelatedRemove(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityCollection<TForeignEntity, TRelatedEntity> relatedCollection = getRColc(ForeignEntity);
#if Test || Test1
                if (relatedCollection == null)
                    throw new TestException($"[{this}] RelatedRemove Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");
#endif
                relatedCollection.InternalRemove(RelatedEntity);
            }
        }


        #endregion Foreign Manager



        #region Related Manager   

        public override bool IsRelated(TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityCollection<TForeignEntity, TRelatedEntity> relatedCollection = getRColc(ForeignEntity);
#if Test || Test1
                if (relatedCollection == null)
                    throw new TestException($"[{this}] IsRelated Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");
#endif
                return relatedCollection.Count != 0;
            }
            else
            {
                foreach (TRelatedEntity RelatedEntity in relatedKeyManager)
                {
                    if (RelatedEntity.HasBusy())
                        throw new EntityRelationManagerException($"[{this}] IsRelated Failed, {ForeignEntity} Cannot Be Verified {RelatedEntity} Is Related, Because It's State Is {RelatedEntity.State}");

                    if (RelatedEntity.IsDeleted())
                        continue;

                    TForeignEntity foreignEntity = getFRef(RelatedEntity);

                    if (foreignEntity == ForeignEntity)
                        return true;
                }

                return false;
            }

        }

        public override void NotifyAllRelated(TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityCollection<TForeignEntity, TRelatedEntity> relatedCollection = getRColc(ForeignEntity);

                foreach (TRelatedEntity RelatedEntity in relatedCollection)
                {
                    if (!RelatedEntity.HasEditable() || RelatedEntity.HasBusy())
                        throw new EntityRelationManagerException($"[{this}] NotifyAllRelated Failed, {RelatedEntity} With {RelatedEntity.State} State Is Not Supported");

                    RelatedEntity.State |= EntityState.Busy;

                    RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                    RelatedEntity.State ^= EntityState.Busy;
                }
            }
            else
            {
                foreach (TRelatedEntity RelatedEntity in relatedEntitySet)
                {
                    TForeignEntity foreignEntity = getFRef(RelatedEntity);

                    if (foreignEntity == ForeignEntity)
                    {
                        if (!RelatedEntity.HasEditable() || RelatedEntity.HasBusy())
                            throw new EntityRelationManagerException($"[{this}] NotifyAllRelated Failed, {RelatedEntity} With {RelatedEntity.State} State Is Not Supported");

                        RelatedEntity.State |= EntityState.Busy;

                        RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                        RelatedEntity.State ^= EntityState.Busy;
                    }
                }
            }
        }
        public override void RestoreAllRelated(TForeignEntity ForeignEntity)
        {
            TForeignKey? foreignKey = foreignKeyManager.GetEntityKey(ForeignEntity);

            if (propRelated != null)
            {
                EntityCollection<TForeignEntity, TRelatedEntity> relatedCollection = getRColc(ForeignEntity);

                foreach (TRelatedEntity RelatedEntity in relatedCollection)
                {
                    setFKey(RelatedEntity, foreignKey);

                    RelatedEntity.State |= EntityState.Busy;

                    RelatedEntity.OnPropertyChanged(propForeignKey.Name);
                    RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                    RelatedEntity.State ^= EntityState.Busy;
                }
            }
            else
                foreach (TRelatedEntity RelatedEntity in relatedEntitySet)
                {
                    TForeignEntity foreignEntity = getFRef(RelatedEntity);

                    if (foreignEntity == ForeignEntity)
                    {
                        setFKey(RelatedEntity, foreignKey);

                        RelatedEntity.State |= EntityState.Busy;

                        RelatedEntity.OnPropertyChanged(propForeignKey.Name);
                        RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                        RelatedEntity.State ^= EntityState.Busy;
                    }
                }
        }

        public override void UpdatedAllOrphan(TForeignEntity ForeignEntity)
        {
            TForeignKey foreignKey = foreignKeyManager.GetEntityKey(ForeignEntity);

            if (propRelated != null)
            {
                EntityCollection<TForeignEntity, TRelatedEntity> relatedCollection = getRColc(ForeignEntity);

                foreach (TRelatedEntity RelatedEntity in orphanage.GetAllRelated(foreignKeyManager.GetEntityKey(ForeignEntity)).ToArray())
                {
                    if (!RelatedEntity.HasEditable() || RelatedEntity.HasBusy())
                        throw new EntityRelationManagerException($"[{this}] UpdatedAllOrphan Failed, {RelatedEntity} With {RelatedEntity.State} State Is Not Supported");

                    // UpdatedOrphan ....
                    setFRef(RelatedEntity, ForeignEntity);
                    OrphanageRemove(RelatedEntity, foreignKey);

                    relatedCollection.InternalAdd(RelatedEntity);

                    RelatedEntity.State |= EntityState.Busy;

                    RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                    RelatedEntity.State ^= EntityState.Busy;
                }
            }
            else
                foreach (TRelatedEntity RelatedEntity in orphanage.GetAllRelated(foreignKeyManager.GetEntityKey(ForeignEntity)).ToArray())
                {
                    if (!RelatedEntity.HasEditable() || RelatedEntity.HasBusy())
                        throw new EntityRelationManagerException($"[{this}] UpdatedAllOrphan Failed, {RelatedEntity} With {RelatedEntity.State} State Is Not Supported");

                    // UpdatedOrphan ....
                    setFRef(RelatedEntity, ForeignEntity);
                    OrphanageRemove(RelatedEntity, foreignKey);

                    RelatedEntity.State |= EntityState.Busy;

                    RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                    RelatedEntity.State ^= EntityState.Busy;
                }
        }
        public override void UpdatedAllRelated(TForeignEntity ForeignEntity)
        {
            TForeignKey? foreignKey = foreignKeyManager.GetEntityKey(ForeignEntity);

            if (propRelated != null)
            {
                EntityCollection<TForeignEntity, TRelatedEntity> relatedCollection = getRColc(ForeignEntity);

                foreach (TRelatedEntity RelatedEntity in relatedCollection)
                {
                    // when an unchanged entity reference to added entity it's state change from unchanged to modified,
                    // therefore when insert added entity to database, any related entity can be added or modified.
                    if (RelatedEntity.HasBusy() || !RelatedEntity.HasChanged())
                        throw new EntityRelationManagerException($"[{this}] UpdatedAllRelated Failed, {RelatedEntity} With {RelatedEntity.State} State Is Not Supported");

                    setFKey(RelatedEntity, foreignKey);

                    RelatedEntity.State |= EntityState.Busy;

                    RelatedEntity.OnPropertyChanged(propForeignKey.Name);
                    RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                    RelatedEntity.State ^= EntityState.Busy;
                }
            }
            else
                foreach (TRelatedEntity RelatedEntity in relatedEntitySet)
                {
                    TForeignEntity foreignEntity = getFRef(RelatedEntity);

                    if (foreignEntity == ForeignEntity)
                    {
                        // when an unchanged entity reference to added entity it's state change from unchanged to modified,
                        // therefore when insert added entity to database, any related entity can be added or modified.
                        if (RelatedEntity.HasBusy() || !RelatedEntity.HasChanged())
                            throw new EntityRelationManagerException($"[{this}] UpdatedAllRelated Failed, {RelatedEntity} With {RelatedEntity.State} State Is Not Supported");

                        setFKey(RelatedEntity, foreignKey);

                        RelatedEntity.State |= EntityState.Busy;

                        RelatedEntity.OnPropertyChanged(propForeignKey.Name);
                        RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                        RelatedEntity.State ^= EntityState.Busy;
                    }
                }
        }

        public override void ResetRelated(TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityCollection<TForeignEntity, TRelatedEntity> relatedCollection = getRColc(ForeignEntity);
#if Test || Test1
                if (relatedCollection == null)
                    throw new TestException($"[{this}] ResetRelated Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");
#endif
                relatedCollection.Reset();
            }
        }
        public override void InitializeRelated(TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityCollection<TForeignEntity, TRelatedEntity> relatedCollection = getRColc(ForeignEntity);

                if (relatedCollection == null)
                    throw new EntityRelationManagerException($"[{this}] InitializeRelated Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");

                relatedCollection.Initialize(ForeignEntity, this);
            }
        }

        #endregion Related Manager



        #region Unique Constraint

        private protected override bool HasUniqueConstraint => false;
        private protected override bool IsUnique(TForeignKey ForeignKey)
        {
            throw new NotSupportedException($"{this} Not Support Unique Constraint");
        }
        private protected override bool IsUnique(TForeignEntity ForeignEntity)
        {
            throw new NotSupportedException($"{this} Not Support Unique Constraint");
        }

        private protected override void AddUniqueConstraint(TRelatedEntity RelatedEntity)
        {
            throw new NotSupportedException($"{this} Not Support Unique Constraint");
        }
        private protected override void RemoveUniqueConstraint(TForeignKey? ForeignKey)
        {
            throw new NotSupportedException($"{this} Not Support Unique Constraint");
        }

        #endregion Unique Constraint



        #region Late Add

        private protected override bool HasLateAdd => lateAdds.Count > 0;

        private protected override void LateAddUpdateRelated(TRelatedEntity RelatedEntity)
        {
            if (lateAdds.TryGetValue(RelatedEntity, out TForeignEntity previousForeignEntity))
            {
                // related must be removed, whether foreign changed or not changed, 
                // if foreign not changed, when adding related entity to entity set, by calling UpdateRelated method related entity will be added to this foreign entity related collection once again.
                RelatedRemove(RelatedEntity, previousForeignEntity);

                lateAdds.Remove(RelatedEntity);
            }
        }

        public void LateAdd(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            TForeignEntity foreignEntity = ForeignEntity;
            TForeignKey? foreignKey = foreignKeyManager.GetEntityKey(ForeignEntity);

            setFRef(RelatedEntity, foreignEntity);
            setFKey(RelatedEntity, foreignKey);

            RelatedEntity.State = EntityState.LateAdd;
            RelatedEntity.EntitySet = relatedEntitySet;

            lateAdds.Add(RelatedEntity, ForeignEntity);

            relatedEntitySet.LateAddInsert(RelatedEntity);

            RelatedEntity.OnPropertyChanged(propForeignKey.Name);
            RelatedEntity.OnPropertyChanged(propForeignReference.Name);
        }
        public void LateAddCommit(TRelatedEntity RelatedEntity)
        {
            if (lateAdds.Count > 0 && lateAdds.TryGetValue(RelatedEntity, out TForeignEntity previousForeignEntity))
            {
                TForeignKey? previousForeignKey = foreignKeyManager.GetEntityKey(previousForeignEntity);

                TForeignEntity foreignEntity = getFRef(RelatedEntity);
                TForeignKey? foreignKey = getFKey(RelatedEntity);

                byte changes = 0;

                if (!Equals(previousForeignKey, foreignKey)) changes |= 0x2;

                if (previousForeignEntity != foreignEntity) changes |= 0x4;

                if (changes == 2)
                    setFRef(RelatedEntity, default);

                if (changes == 4)
                    setFKey(RelatedEntity, default);
            }
        }
        public void LateAddCancel(TRelatedEntity RelatedEntity)
        {
            lateAdds.Remove(RelatedEntity);

            relatedEntitySet.LateAddCancel(RelatedEntity);
        }

        #endregion Late Add



        #region Other

        public override string ToString()
        {
            return $"{typeof(TForeignEntity).Name},{typeof(TRelatedEntity).Name} OneToManyRelationManager";
        }

        #endregion Other
    }

    #region interface

    internal interface IOneToManyEntityRelationManager<TForeignEntity, TRelatedEntity> : IEntityRelationManager<TForeignEntity, TRelatedEntity>, IOneToManyEntityRelationForeignManager<TRelatedEntity>, IOneToManyEntityRelationRelatedManager<TForeignEntity> where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
        void LateAdd(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity);
    }

    internal interface IOneToManyEntityRelationForeignManager<TRelatedEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
        void LateAddCommit(TRelatedEntity RelatedEntity);
        void LateAddCancel(TRelatedEntity RelatedEntity);
    }

    internal interface IOneToManyEntityRelationRelatedManager<TForeignEntity> where TForeignEntity : Entity<TForeignEntity>
    {
    }

    #endregion interface

}
