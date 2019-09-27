using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    internal sealed class OneToOneEntityRelationManager<TForeignEntity, TForeignKey, TRelatedEntity> : EntityRelationManager<TForeignEntity, TForeignKey, TRelatedEntity>, IOneToOneEntityRelationManager<TForeignEntity, TRelatedEntity> where TForeignEntity : Entity<TForeignEntity> where TForeignKey : struct where TRelatedEntity : Entity<TRelatedEntity>
    {
        #region Constructors and Variables

        static OneToOneEntityRelationManager()
        {
            accessors = new Dictionary<EntityRelation, Func<TForeignEntity, EntityUnique<TForeignEntity, TRelatedEntity>>>();
        }
        internal OneToOneEntityRelationManager(EntityRelation<TForeignEntity, TRelatedEntity> EntityRelation, EntitySet<TForeignEntity> ForeignEntitySet, EntitySet<TRelatedEntity> RelatedEntitySet) : base(EntityRelation, ForeignEntitySet, RelatedEntitySet)
        {
            hasUniqueConstraint = true;

            uniqueKey = new HashSet<TForeignKey>();

            #region Generate Property Setter And Getter

            if (propRelated != null)
            {
                if (!accessors.TryGetValue(EntityRelation, out Func<TForeignEntity, EntityUnique<TForeignEntity, TRelatedEntity>> accessor))
                {
                    Type typeForeignEntity = typeof(TForeignEntity);

                    MethodInfo propRelatedGetMthd = propRelated.GetGetMethod(true);

                    ParameterExpression argForeignEntity = Expression.Parameter(typeForeignEntity, "ForeignEntity");

                    accessors.Add(EntityRelation, accessor = Expression.Lambda<Func<TForeignEntity, EntityUnique<TForeignEntity, TRelatedEntity>>>(Expression.TypeAs(Expression.Call(argForeignEntity, propRelatedGetMthd), typeof(EntityUnique<TForeignEntity, TRelatedEntity>)), argForeignEntity).Compile());
                }

                getRUniq = accessor;
            }

            #endregion Generate Property Settery And Getter

        }


        private static Dictionary<EntityRelation, Func<TForeignEntity, EntityUnique<TForeignEntity, TRelatedEntity>>> accessors;

        private Func<TForeignEntity, EntityUnique<TForeignEntity, TRelatedEntity>> getRUniq;
        private HashSet<TForeignKey> uniqueKey;
        private bool hasUniqueConstraint;

        #endregion Constructors and Variables



        #region Foreign Manager

        private protected override void RelatedAdd(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityUnique<TForeignEntity, TRelatedEntity> relatedUnique = getRUniq(ForeignEntity);
#if Test || Test2
                if (relatedUnique == null)
                    throw new TestException($"[{this}] RelatedAdd Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");
#endif
                relatedUnique.InternalSet(RelatedEntity);
            }
        }
        private protected override void RelatedRemove(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityUnique<TForeignEntity, TRelatedEntity> relatedUnique = getRUniq(ForeignEntity);
#if Test || Test2
                if (relatedUnique == null)
                    throw new TestException($"[{this}] RelatedRemove Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");
#endif
                relatedUnique.InternalSet(default(TRelatedEntity));
            }
        }

        #endregion Foreign Manager



        #region Related Manager

        public override bool IsRelated(TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityUnique<TForeignEntity, TRelatedEntity> relatedUnique = getRUniq(ForeignEntity);
#if Test || Test2
                if (relatedUnique == null)
                    throw new TestException($"[{this}] IsRelated Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");
#endif
                return !relatedUnique.Entity.IsNull();
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
                EntityUnique<TForeignEntity, TRelatedEntity> relatedUnique = getRUniq(ForeignEntity);

                TRelatedEntity RelatedEntity = relatedUnique.Entity;

                if (!RelatedEntity.IsNull())
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

                        break;
                    }
                }
            }
        }
        public override void RestoreAllRelated(TForeignEntity ForeignEntity)
        {
            TForeignKey? foreignKey = foreignKeyManager.GetEntityKey(ForeignEntity);

            if (propRelated != null)
            {
                EntityUnique<TForeignEntity, TRelatedEntity> relatedUnique = getRUniq(ForeignEntity);

                TRelatedEntity RelatedEntity = relatedUnique.Entity;

                if (!RelatedEntity.IsNull())
                {
                    UpdateUniqueConstraint(foreignKey, getFKey(RelatedEntity));

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
                        UpdateUniqueConstraint(foreignKey, getFKey(RelatedEntity));

                        setFKey(RelatedEntity, foreignKey);

                        RelatedEntity.State |= EntityState.Busy;

                        RelatedEntity.OnPropertyChanged(propForeignKey.Name);
                        RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                        RelatedEntity.State ^= EntityState.Busy;

                        break;
                    }
                }
        }

        public override void UpdatedAllOrphan(TForeignEntity ForeignEntity)
        {
            TForeignKey foreignKey = foreignKeyManager.GetEntityKey(ForeignEntity);

            if (propRelated != null)
            {
                EntityUnique<TForeignEntity, TRelatedEntity> relatedUnique = getRUniq(ForeignEntity);

                TRelatedEntity RelatedEntity = orphanage.Count(foreignKey) == 1 ? orphanage.GetAllRelated(foreignKey).ToArray()[0] : default;

                if (!RelatedEntity.IsNull()) // orphanage.Count != 0
                {
                    if (!RelatedEntity.HasEditable() || RelatedEntity.HasBusy())
                        throw new EntityRelationManagerException($"[{this}] UpdatedAllOrphan Failed, {RelatedEntity} With {RelatedEntity.State} State Is Not Supported");

                    // UpdatedOrphan ....
                    setFRef(RelatedEntity, ForeignEntity);
                    OrphanageRemove(RelatedEntity, foreignKey);

                    relatedUnique.InternalSet(RelatedEntity);

                    RelatedEntity.State |= EntityState.Busy;

                    RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                    RelatedEntity.State ^= EntityState.Busy;
                }
            }
            else
            {
                TRelatedEntity RelatedEntity = orphanage.Count(foreignKey) == 1 ? orphanage.GetAllRelated(foreignKey).ToArray()[0] : default;

                if (!RelatedEntity.IsNull()) // orphanage.Count != 0
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
        }
        public override void UpdatedAllRelated(TForeignEntity ForeignEntity)
        {
            TForeignKey? foreignKey = foreignKeyManager.GetEntityKey(ForeignEntity);

            if (propRelated != null)
            {
                EntityUnique<TForeignEntity, TRelatedEntity> relatedUnique = getRUniq(ForeignEntity);

                TRelatedEntity RelatedEntity = relatedUnique.Entity;

                if (!RelatedEntity.IsNull())
                {
                    UpdateUniqueConstraint(foreignKey, getFKey(RelatedEntity));

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
                        UpdateUniqueConstraint(foreignKey, getFKey(RelatedEntity));

                        // when an unchanged entity reference to added entity it's state change from unchanged to modified,
                        // therefore when insert added entity to database, any related entity can be added or modified.
                        if (RelatedEntity.HasBusy() || !RelatedEntity.HasChanged())
                            throw new EntityRelationManagerException($"[{this}] UpdatedAllRelated Failed, {RelatedEntity} With {RelatedEntity.State} State Is Not Supported");

                        setFKey(RelatedEntity, foreignKey);

                        RelatedEntity.State |= EntityState.Busy;

                        RelatedEntity.OnPropertyChanged(propForeignKey.Name);
                        RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                        RelatedEntity.State ^= EntityState.Busy;

                        break;
                    }
                }
        }

        public override void ResetRelated(TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityUnique<TForeignEntity, TRelatedEntity> relatedUnique = getRUniq(ForeignEntity);
#if Test || Test2
                if (relatedUnique == null)
                    throw new TestException($"[{this}] ResetRelated Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");
#endif
                relatedUnique.Reset();
            }
        }
        public override void InitializeRelated(TForeignEntity ForeignEntity)
        {
            if (propRelated != null)
            {
                EntityUnique<TForeignEntity, TRelatedEntity> relatedUnique = getRUniq(ForeignEntity);

                if (relatedUnique == null)
                    throw new EntityRelationManagerException($"[{this}] InitializeRelated Failed, {ForeignEntity} {typeof(TForeignEntity).Name}.{propRelated.Name} Is Null");

                relatedUnique.Initialize(ForeignEntity, this);
            }
        }

        #endregion Related Manager



        #region Unique Constraint

        private protected override bool HasUniqueConstraint => hasUniqueConstraint;
        private protected override bool IsUnique(TForeignKey ForeignKey)
        {
            return !uniqueKey.Contains(ForeignKey);
        }
        private protected override bool IsUnique(TForeignEntity ForeignEntity)
        {
            return !uniqueKey.Contains(foreignKeyManager.GetEntityKey(ForeignEntity));
        }

        private protected override void AddUniqueConstraint(TRelatedEntity RelatedEntity)
        {
            TForeignKey? foreignKey = getFKey(RelatedEntity);

            if (foreignKey.HasValue)
                uniqueKey.Add(foreignKey.Value);
        }
        private protected override void RemoveUniqueConstraint(TForeignKey? ForeignKey)
        {
            TForeignKey? foreignKey = ForeignKey;

            if (foreignKey.HasValue)
                uniqueKey.Remove(foreignKey.Value);
        }

        private void UpdateUniqueConstraint(TForeignKey? NewForeignKey, TForeignKey? PreviousForeignKey)
        {
            if (PreviousForeignKey.HasValue)
                uniqueKey.Remove(PreviousForeignKey.Value);

            if (NewForeignKey.HasValue)
                uniqueKey.Add(NewForeignKey.Value);
        }

        #endregion Unique Constraint



        #region Late Add

        private protected override bool HasLateAdd => false;

        private protected override void LateAddUpdateRelated(TRelatedEntity RelatedEntity)
        {
            throw new NotSupportedException($"{this} Not Support Late Add");
        }

        #endregion Late Add



        #region Other

        public override string ToString()
        {
            return $"{typeof(TForeignEntity).Name},{typeof(TRelatedEntity).Name} OneToOneRelationManager";
        }

        #endregion Other
    }

    #region interface

    internal interface IOneToOneEntityRelationManager<TForeignEntity, TRelatedEntity> : IEntityRelationManager<TForeignEntity, TRelatedEntity>, IOneToOneEntityRelationForeignManager<TRelatedEntity>, IOneToOneEntityRelationRelatedManager<TForeignEntity> where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
    }

    internal interface IOneToOneEntityRelationForeignManager<TRelatedEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
    }

    internal interface IOneToOneEntityRelationRelatedManager<TForeignEntity> where TForeignEntity : Entity<TForeignEntity>
    {
    }

    #endregion interface

}
