using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    internal abstract class EntityRelationManager<TForeignEntity, TForeignKey, TRelatedEntity> : IEntityRelationManager<TForeignEntity, TRelatedEntity> where TForeignEntity : Entity<TForeignEntity> where TForeignKey : struct where TRelatedEntity : Entity<TRelatedEntity>
    {
        #region Constructors and Variables

        static EntityRelationManager()
        {
            accessors = new Dictionary<EntityRelation, Delegate[]>();
        }
        private EntityRelationManager()
        {
        }

        internal EntityRelationManager(EntityRelation<TForeignEntity, TRelatedEntity> EntityRelation, EntitySet<TForeignEntity> ForeignEntitySet, EntitySet<TRelatedEntity> RelatedEntitySet)
        {
            entityRelation = EntityRelation;

            relatedEntitySet = RelatedEntitySet;
            foreignEntitySet = ForeignEntitySet;

            relatedKeyManager = RelatedEntitySet.EntityKeyManager;
            foreignKeyManager = (EntityKeyManager<TForeignEntity, TForeignKey>)ForeignEntitySet.EntityKeyManager;

            propRelated = entityRelation.PropRelated;
            propForeignKey = entityRelation.PropForeignKey;
            propForeignReference = entityRelation.PropForeignReference;

            orphanage = new EntityRelationOrphanManager<TRelatedEntity, TForeignKey>();

            isforeignKeyNullable = propForeignKey.PropertyType.IsGenericType && propForeignKey.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

            #region Generate Property Setter And Getter

            if (!accessors.TryGetValue(EntityRelation, out Delegate[] accessor))
            {
                Type typeRelatedEntity = typeof(TRelatedEntity);
                Type typeRelatedEntityData = EntityProxy<TRelatedEntity>.DataType;
                Type typeRelatedEntityProxy = typeof(EntityProxy<TRelatedEntity>);
                Type typeRelatedEntityProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeRelatedEntity, typeRelatedEntityData });

                MethodInfo propDataGetMthd = typeRelatedEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                FieldInfo fldItem = typeRelatedEntityProxyData.GetField("item");
                FieldInfo fldFKey = typeRelatedEntityData.GetField(propForeignKey.Name);
                FieldInfo fldFRef = typeRelatedEntityData.GetField(propForeignReference.Name);

                ParameterExpression argRelatedEntity = Expression.Parameter(typeRelatedEntity, "RelatedEntity");

                MemberExpression expFKey = Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(argRelatedEntity, propDataGetMthd), typeRelatedEntityProxyData), fldItem), fldFKey);
                MemberExpression expFRef = Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(argRelatedEntity, propDataGetMthd), typeRelatedEntityProxyData), fldItem), fldFRef);


                accessors.Add(EntityRelation, accessor = new Delegate[6]);


                // Get Foreign Key Accessor
                if (isforeignKeyNullable)
                    accessor[0] = Expression.Lambda<Func<TRelatedEntity, TForeignKey?>>(expFKey, argRelatedEntity).Compile();
                else
                    accessor[0] = Expression.Lambda<Func<TRelatedEntity, TForeignKey?>>(Expression.Convert(expFKey, typeof(TForeignKey?)), argRelatedEntity).Compile();

                // Get Foreign Ref Accessor
                accessor[1] = Expression.Lambda<Func<TRelatedEntity, TForeignEntity>>(expFRef, argRelatedEntity).Compile();

                ParameterExpression argFKey = Expression.Parameter(typeof(TForeignKey?), "ForeignKey");
                ParameterExpression argFRef = Expression.Parameter(typeof(TForeignEntity), "ForeignReference");

                // Set Foreign Key Accessor
                if (isforeignKeyNullable)
                    accessor[2] = Expression.Lambda<Action<TRelatedEntity, TForeignKey?>>(Expression.Assign(expFKey, argFKey), argRelatedEntity, argFKey).Compile();
                else
                    accessor[2] = Expression.Lambda<Action<TRelatedEntity, TForeignKey?>>(Expression.Assign(expFKey, Expression.Call(argFKey, typeof(TForeignKey?).GetProperty("Value").GetGetMethod())), argRelatedEntity, argFKey).Compile();


                // Set Foreign Ref Accessor
                accessor[3] = Expression.Lambda<Action<TRelatedEntity, TForeignEntity>>(Expression.Assign(expFRef, argFRef), argRelatedEntity, argFRef).Compile();



                // ---- Proxy ----
                ParameterExpression argRelatedEntityProxy = Expression.Parameter(typeRelatedEntityProxy, "RelatedEntityProxy");

                MemberExpression expPrxFKey = Expression.Field(Expression.Field(Expression.TypeAs(argRelatedEntityProxy, typeRelatedEntityProxyData), fldItem), fldFKey);
                MemberExpression expPrxFRef = Expression.Field(Expression.Field(Expression.TypeAs(argRelatedEntityProxy, typeRelatedEntityProxyData), fldItem), fldFRef);

                // Get Proxy Foreign Key Accessor
                if (isforeignKeyNullable)
                    accessor[4] = Expression.Lambda<Func<EntityProxy<TRelatedEntity>, TForeignKey?>>(expPrxFKey, argRelatedEntityProxy).Compile();
                else
                    accessor[4] = Expression.Lambda<Func<EntityProxy<TRelatedEntity>, TForeignKey?>>(Expression.Convert(expPrxFKey, typeof(TForeignKey?)), argRelatedEntityProxy).Compile();

                // Get Proxy Foreign Ref Accessor
                accessor[5] = Expression.Lambda<Func<EntityProxy<TRelatedEntity>, TForeignEntity>>(expPrxFRef, argRelatedEntityProxy).Compile();
            }

            getFKey = (Func<TRelatedEntity, TForeignKey?>)accessor[0];
            getFRef = (Func<TRelatedEntity, TForeignEntity>)accessor[1];
            setFKey = (Action<TRelatedEntity, TForeignKey?>)accessor[2];
            setFRef = (Action<TRelatedEntity, TForeignEntity>)accessor[3];

            getPrxFKey = (Func<EntityProxy<TRelatedEntity>, TForeignKey?>)accessor[4];
            getPrxFRef = (Func<EntityProxy<TRelatedEntity>, TForeignEntity>)accessor[5];

            #endregion Generate Property Settery And Getter
        }


        private static Dictionary<EntityRelation, Delegate[]> accessors;

        private protected Func<TRelatedEntity, TForeignKey?> getFKey;
        private protected Func<TRelatedEntity, TForeignEntity> getFRef;
        private protected Action<TRelatedEntity, TForeignKey?> setFKey;
        private protected Action<TRelatedEntity, TForeignEntity> setFRef;

        private protected Func<EntityProxy<TRelatedEntity>, TForeignKey?> getPrxFKey;
        private protected Func<EntityProxy<TRelatedEntity>, TForeignEntity> getPrxFRef;

        private protected EntitySet<TRelatedEntity> relatedEntitySet;
        private protected EntitySet<TForeignEntity> foreignEntitySet;
        private protected EntityKeyManager<TRelatedEntity> relatedKeyManager;
        private protected EntityKeyManager<TForeignEntity, TForeignKey> foreignKeyManager;
        private protected EntityRelation<TForeignEntity, TRelatedEntity> entityRelation;

        private protected bool isforeignKeyNullable;

        private protected PropertyInfo propRelated;
        private protected PropertyInfo propForeignKey;
        private protected PropertyInfo propForeignReference;

        private protected EntityRelationOrphanManager<TRelatedEntity, TForeignKey> orphanage;

        #endregion Constructors and Variables



        #region Foreign Manager

        #region Add

        public ForeignStatus GetForeignStatus(TRelatedEntity RelatedEntity)
        {
            TForeignEntity foreignEntity = getFRef(RelatedEntity);
            TForeignKey? foreignKey = getFKey(RelatedEntity);

            ForeignStatus foreignStatus = GetForeignStatus(foreignEntity, foreignKey);

            // Preview entity that wanted to be added to entity set
            // if is an exist database entity (is't key is greater than zero), it must be reference to other exist foreign entity in database (it's key is greater than zero)
            // therefore before adding this entity to entity set, it can't reference to foreign entity, that's not exist in database (it's key is less than zero)
            // expected result : (1) entityset.add(exist entity that not ref to not exist entity) -> [state -> Unchanged] (2) edit foreign property of exist entity -> [state -> Modified]
            // if in wrong way, exist database entity (is't key is greater than zero) with reference to foreign entity that's not exist in database (it's key is less than zero), the unexpected result will be occur
            // unexpected result : (1) entityset.add(exist entity that ref to not exist entity) -> [state -> Unchanged], what happen ??? we want state to be Modified

            if (foreignStatus.IsValid())
                if (RelatedEntity.IsDetached())
                {
                    if (relatedKeyManager.IsExist(RelatedEntity))
                    {
                        TForeignKey key = default;

                        switch (foreignStatus)
                        {
                            case ForeignStatus.Null:
                            case ForeignStatus.Orphan:
                                return foreignStatus;

                            case ForeignStatus.UpdateRef:
                                key = foreignKey.Value;
                                break;

                            case ForeignStatus.UpdateKey:
                            case ForeignStatus.Complete:
                                key = foreignKeyManager.GetEntityKey(foreignEntity);
                                break;
                        }

                        if (!foreignKeyManager.IsExist(key))
                            return ForeignStatus.RefToNotExistEntity; // Invalid
                    }
                }
                else
                    throw new CriticalException($"[{this}] GetForeignStatus : {RelatedEntity} Sate Must Be Detached");

            return foreignStatus;
        }
        public void UpdateOnAdd(TRelatedEntity RelatedEntity, ForeignStatus ForeignStatus)
        {
            switch (ForeignStatus)
            {
                case ForeignStatus.Null:
                case ForeignStatus.Orphan:
                case ForeignStatus.Complete:
                    break;
                case ForeignStatus.UpdateRef:
                    setFRef(RelatedEntity, foreignKeyManager.GetEntity(getFKey(RelatedEntity).Value));
                    break;
                case ForeignStatus.UpdateKey:
                    setFKey(RelatedEntity, foreignKeyManager.GetEntityKey(getFRef(RelatedEntity)));
                    break;
                default:
                    throw new CriticalException($"[{this}] UpdateOnAdd For {RelatedEntity} By {ForeignStatus} ForeignStatus Is Not Supported");
            }

            if (HasUniqueConstraint)
                AddUniqueConstraint(RelatedEntity);

            if (HasLateAdd)
                LateAddUpdateRelated(RelatedEntity);

            // value must be get at this point, because ref or key value may be updated in previous code lines
            TForeignEntity foreignEntity = getFRef(RelatedEntity);
            TForeignKey? foreignKey = getFKey(RelatedEntity);

            switch (ForeignStatus)
            {
                case ForeignStatus.Null:
                    break;
                case ForeignStatus.Orphan:
                    OrphanageAdd(RelatedEntity, foreignKey);
                    break;
                case ForeignStatus.Complete:
                case ForeignStatus.UpdateKey:
                case ForeignStatus.UpdateRef:
                    RelatedAdd(RelatedEntity, foreignEntity);
                    break;
                default:
                    throw new CriticalException($"[{this}] UpdateOnAdd For {RelatedEntity} By {ForeignStatus} ForeignStatus Is Not Supported");
            }

            if (ForeignStatus.IsUpdateKey())
                RelatedEntity.OnPropertyChanged(propForeignKey.Name);
            else if (ForeignStatus.IsUpdateRef())
                RelatedEntity.OnPropertyChanged(propForeignReference.Name);
        }

        #endregion Add

        #region Remove

        public void UpdateOnRemove(TRelatedEntity RelatedEntity)
        {
            TForeignEntity foreignEntity = getFRef(RelatedEntity);
            TForeignKey? foreignKey = getFKey(RelatedEntity);

            RemovePreviousRelated(RelatedEntity, foreignEntity, foreignKey);
        }

        #endregion

        #region Edit

        public ForeignStatus GetForeignStatus<TProperty>(TRelatedEntity RelatedEntity, EditedEntity<TRelatedEntity, TProperty> Edited)
        {
            TForeignKey? foreignKey = default;
            TForeignEntity foreignEntity = default;

            if (Edited.Property == propForeignKey)
                foreignKey = (TForeignKey?)(object)Edited.Value.New;
            else if (Edited.Property == propForeignReference)
                foreignEntity = (TForeignEntity)(object)Edited.Value.New;
            else
                throw new CriticalException($"[{this}] GetForeignStatus Failed, {RelatedEntity} Non Foreign Property {Edited.Property.Name} Is Not Supported");

            return GetForeignStatus(foreignEntity, foreignKey);
        }
        public void UpdateOnEdit<TProperty>(TRelatedEntity RelatedEntity, EditedEntity<TRelatedEntity, TProperty> Edited, ForeignStatus ForeignStatus)
        {
            // Remove This Related From [Current Foreign Collection Property] OR [Orphanage]

            TForeignKey? foreignKey_Current = getFKey(RelatedEntity);
            TForeignEntity foreignEntity_Current = getFRef(RelatedEntity);

            RemovePreviousRelated(RelatedEntity, foreignEntity_Current, foreignKey_Current);

            // Set ForeignKey And ForeignReference
            TForeignKey? foreignKey_New;
            TForeignEntity foreignEntity_New;

            if (Edited.Property == propForeignKey)
            {
                foreignKey_New = (TForeignKey?)(object)Edited.Value.New;

                setFKey(RelatedEntity, foreignKey_New);

                if (ForeignStatus.IsNull() || ForeignStatus.IsOrphan())
                    setFRef(RelatedEntity, default);
                else if (ForeignStatus.IsUpdateRef())
                    setFRef(RelatedEntity, foreignKeyManager.GetEntity(foreignKey_New.Value));
                else
                    throw new CriticalException($"[{this}] Edit For {RelatedEntity} By {ForeignStatus} ForeignStatus Is Not Supported");
            }
            else if (Edited.Property == propForeignReference)
            {
                foreignEntity_New = (TForeignEntity)(object)Edited.Value.New;

                setFRef(RelatedEntity, foreignEntity_New);

                if (ForeignStatus.IsNull())
                    setFKey(RelatedEntity, default);
                else if (ForeignStatus.IsUpdateKey())
                    setFKey(RelatedEntity, foreignKeyManager.GetEntityKey(foreignEntity_New));
                else
                    throw new CriticalException($"[{this}] Edit For {RelatedEntity} By {ForeignStatus} ForeignStatus Is Not Supported");
            }
            else
                throw new CriticalException($"[{this}] Edit For {RelatedEntity} By {ForeignStatus} ForeignStatus Is Not Supported");

            // value must be get at this point, because null value is not be assigned to local variable
            foreignKey_New = getFKey(RelatedEntity);
            foreignEntity_New = getFRef(RelatedEntity);

            if (Edited.TrakMode.IsTrack())
                relatedEntitySet.EntityChangeTracker.Track(RelatedEntity, propForeignKey);
            else if (Edited.TrakMode.IsUnTrack())
                relatedEntitySet.EntityChangeTracker.UnTrack(RelatedEntity, propForeignKey);

            // Add This Related To [New Foreign Collection Property] OR [Orphanage]

            if (ForeignStatus.IsUpdateKey() || ForeignStatus.IsUpdateRef())
                RelatedAdd(RelatedEntity, foreignEntity_New);
            else if (ForeignStatus.IsOrphan())
                OrphanageAdd(RelatedEntity, foreignKey_New);
            else if (!ForeignStatus.IsNull())
                throw new CriticalException($"[{this}] Edit For {RelatedEntity} By {ForeignStatus} ForeignStatus Is Not Supported");

            if (HasUniqueConstraint)
                AddUniqueConstraint(RelatedEntity);

            if (Edited.Property == propForeignKey)
            {
                RelatedEntity.OnPropertyChanged(propForeignKey.Name);

                if (!Equals(foreignEntity_Current, foreignEntity_New))
                    RelatedEntity.OnPropertyChanged(propForeignReference.Name);
            }
            else if (Edited.Property == propForeignReference)
            {
                RelatedEntity.OnPropertyChanged(propForeignReference.Name);

                if (!Equals(foreignKey_Current, foreignKey_New))
                    RelatedEntity.OnPropertyChanged(propForeignKey.Name);
            }
        }

        #endregion Edit

        #region Change

        public Entity GetDependency(TRelatedEntity RelatedEntity)
        {
            return getFRef(RelatedEntity);
        }

        public int GetDependencyDepth(TRelatedEntity RelatedEntity)
        {
            TForeignEntity foreignEntity = getFRef(RelatedEntity);

            if (foreignEntity.IsNull() || !foreignEntity.IsAdded())
                return -1;
            else
                return foreignEntitySet.GetDependencyDepth(foreignEntity);
        }

        #endregion Change

        #region Cancel

        public void UpdateOnCancel(TRelatedEntity RelatedEntity, EntityProxy<TRelatedEntity> NotCommittedData)
        {
            // Remove This Related From [Current Foreign Collection Property] OR [Orphanage]

            TForeignKey? foreignKey_NotCommitted = getPrxFKey(NotCommittedData);
            TForeignEntity foreignEntity_NotCommitted = getPrxFRef(NotCommittedData);

            TForeignKey? foreignKey_Original = getFKey(RelatedEntity);
            TForeignEntity foreignEntity_Original = getFRef(RelatedEntity);

            if (!Equals(foreignKey_NotCommitted, foreignKey_Original))
            {
                RemovePreviousRelated(RelatedEntity, foreignEntity_NotCommitted, foreignKey_NotCommitted);

                if (HasUniqueConstraint)
                    AddUniqueConstraint(RelatedEntity);

                ForeignStatus foreignStatus = GetForeignStatus(foreignEntity_Original, foreignKey_Original);

                if (foreignStatus.IsComplete())
                    RelatedAdd(RelatedEntity, foreignEntity_Original);
                else if (foreignStatus.IsOrphan())
                    OrphanageAdd(RelatedEntity, foreignKey_Original);
                else if (!foreignStatus.IsNull())
                    throw new CriticalException($"[{this}] UpdateOnCancel For {RelatedEntity} By {foreignStatus} ForeignStatus Is Not Supported");
            }
            else if (foreignEntity_NotCommitted != foreignEntity_Original)
                throw new CriticalException($"[{this}] UpdateOnCancel For {RelatedEntity} Failed, NotCommitted And Original Foreign Key Are Equal, But Foreign Entity Are Not Equal");
        }

        #endregion Cancel

        #region Other

        private ForeignStatus GetForeignStatus(TForeignEntity ForeignEntity, TForeignKey? ForeignKey)
        {
            ForeignStatus foreignStatus = ForeignStatus.Unkhown;

            // two important check base setting ....
            bool foreignKeyIsNullable = isforeignKeyNullable;
            bool foreignEntityIsNull = foreignKeyManager.IsNull(ForeignEntity);
            bool foreignEntityKeyIsNull = !ForeignKey.HasValue;
            bool foreignEntityKeyIsZero = (!foreignKeyIsNullable || !foreignEntityKeyIsNull) && !(!foreignKeyIsNullable && foreignEntityKeyIsNull) ? foreignKeyManager.IsZero(ForeignKey.Value) : true; // -> true value is pseud-true (always is require to check !foreignKeyIsNullable in if statement, before use foreignEntityKeyIsZero) ;

            if (foreignEntityIsNull)
            {
                if (foreignKeyIsNullable && foreignEntityKeyIsNull)
                    return ForeignStatus.Null;

                else if (foreignEntityKeyIsZero)
                    return ForeignStatus.RefKeyIsZero; // Invalid

                else // (1) [ foreignKeyIsNullable && !foreignEntityKeyIsNull] 
                     // (2) [!foreignKeyIsNullable && !foreignEntityKeyIsNull] --> Not Expected : Non Nullable Foreign Key Always Return Value [for example : rerurn zero value by default]
                     // (3) [!foreignKeyIsNullable && !foreignEntityKeyIsZero]
                     // (4) [ foreignKeyIsNullable && !foreignEntityKeyIsZero]
                {
                    if (foreignKeyManager.Find(ForeignKey.Value))
                        foreignStatus = ForeignStatus.UpdateRef;
                    else
                    {
                        if (foreignKeyManager.IsExist(ForeignKey.Value))
                            if (HasUniqueConstraint && !IsUnique(ForeignKey.Value))
                                return ForeignStatus.RefToNonUniqueEntity; // Invalid
                            else
                                return ForeignStatus.Orphan;
                        else if (foreignKeyManager.IsNew(ForeignKey.Value))
                            return ForeignStatus.KeyNotFound; // Invalid
                        else
                            throw new CriticalException($"[{this}] GetForeignStatus, Can Not Return ForeignStatus For Null Entity & Zero Key");
                    }
                }
            }
            else // !foreignEntityIsNull
            {
                if (foreignKeyIsNullable && foreignEntityKeyIsNull)
                    foreignStatus = ForeignStatus.UpdateKey;

                else if (!foreignKeyIsNullable && foreignEntityKeyIsZero)
                    foreignStatus = ForeignStatus.UpdateKey;

                else // (1) [ foreignKeyIsNullable && !foreignEntityKeyIsNull] 
                     // (2) [!foreignKeyIsNullable && !foreignEntityKeyIsNull] --> Not Expected : Non Nullable Foreign Key Always Return Value [for example : rerurn zero value by default]
                     // (3) [!foreignKeyIsNullable && !foreignEntityKeyIsZero]
                     // (4) [ foreignKeyIsNullable && !foreignEntityKeyIsZero]
                {
                    if (foreignKeyManager.HasSameKey(ForeignEntity, ForeignKey.Value))
                        foreignStatus = ForeignStatus.Complete;
                    else if (foreignKeyManager.HasDifferentKey(ForeignEntity, ForeignKey.Value))
                        return ForeignStatus.RefKeyNotEqualKey; // Invalid
                    else
                        throw new CriticalException($"[{this}] GetForeignStatus For {ForeignEntity} Foreign Entity And {ForeignKey} Foreign Entity Key, Haven't Same Or Different Key");
                }
            }

            TForeignEntity tempEntity = default;

            switch (foreignStatus)
            {
                case ForeignStatus.UpdateRef:
                    tempEntity = foreignKeyManager.GetEntity(ForeignKey.Value);
                    break;

                case ForeignStatus.UpdateKey:
                case ForeignStatus.Complete:
                    if (!foreignKeyManager.Find(ForeignEntity))
                        return ForeignStatus.RefNotFound; // Invalid               

                    tempEntity = ForeignEntity;
                    break;
            }

            if (tempEntity.IsNull())
                throw new CriticalException($"[{this}] GetForeignStatus : Temp Entity Is Null");

            if (tempEntity.EntitySet != foreignEntitySet)
                throw new CriticalException($"[{this}] GetForeignStatus : {tempEntity} Foreign Entity, Haven't Same EntitySet As This Context Foreign EntitySet");

            if (!tempEntity.HasEditable())
                return ForeignStatus.RefStateNotEditable; // Invalid

            if (HasUniqueConstraint && !IsUnique(tempEntity))
                return ForeignStatus.RefToNonUniqueEntity; // Invalid

            if (foreignStatus == ForeignStatus.Unkhown)
                throw new CriticalException($"[{this}] GetForeignStatus : ForeignStatus Is Unkhown");

            return foreignStatus;
        }

        private void RemovePreviousRelated(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity, TForeignKey? ForeignKey)
        {
            TForeignEntity foreignEntity = ForeignEntity;
            TForeignKey? foreignKey = ForeignKey;

            if (HasUniqueConstraint)
                RemoveUniqueConstraint(foreignKey); // Constraint must be remove before, call GetForeignStatus

            ForeignStatus foreignStatus = GetForeignStatus(foreignEntity, foreignKey);

            switch (foreignStatus)
            {
                case ForeignStatus.Null:
                    break;
                case ForeignStatus.Orphan:
                    OrphanageRemove(RelatedEntity, foreignKey);
                    break;
                case ForeignStatus.Complete:
                    RelatedRemove(RelatedEntity, foreignEntity);
                    break;
                default:
                    throw new CriticalException($"[{this}] RemovePreviousRelated For {RelatedEntity} By {foreignStatus} ForeignStatus Is Not Supported");
            }
        }

        private protected abstract void RelatedAdd(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity);
        private protected abstract void RelatedRemove(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity);

        private void OrphanageAdd(TRelatedEntity RelatedEntity, TForeignKey? ForeignKey)
        {
            orphanage.Add(ForeignKey.Value, RelatedEntity);
        }
        private protected void OrphanageRemove(TRelatedEntity RelatedEntity, TForeignKey? ForeignKey)
        {
            orphanage.Remove(ForeignKey.Value, RelatedEntity);
        }

        private void UpdateForeign(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity = default)
        {
            RelatedEntity.State = RelatedEntity.State.BeginEdit();

            TForeignEntity foreignEntity = ForeignEntity;
            TForeignKey? foreignKey = foreignEntity.IsNull() ? default(TForeignKey?) : foreignKeyManager.GetEntityKey(foreignEntity);

            setFRef(RelatedEntity, foreignEntity);
            setFKey(RelatedEntity, foreignKey);

            relatedEntitySet.EntityChangeTracker.Track(RelatedEntity, propForeignKey);

            RelatedEntity.OnPropertyChanged(propForeignKey.Name);
            RelatedEntity.OnPropertyChanged(propForeignReference.Name);

            RelatedEntity.State = RelatedEntity.State.EndEdit();
        }

        #endregion

        #endregion Foreign Manager



        #region Related Manager

        public abstract bool IsRelated(TForeignEntity ForeignEntity);

        public abstract void NotifyAllRelated(TForeignEntity ForeignEntity);
        public abstract void RestoreAllRelated(TForeignEntity ForeignEntity);

        public abstract void UpdatedAllOrphan(TForeignEntity ForeignEntity);
        public abstract void UpdatedAllRelated(TForeignEntity ForeignEntity);

        public abstract void ResetRelated(TForeignEntity ForeignEntity);
        public abstract void InitializeRelated(TForeignEntity ForeignEntity);



        public bool Add(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
#if Test || Test7
            if (RelatedEntity.IsNull()) throw new TestException($"[{this}], Add Failed, RelatedEntity Is Null");
            if (ForeignEntity.IsNull()) throw new TestException($"[{this}], Add Failed, Entity Is Null");
#endif
            bool attach = false;

            if (RelatedEntity.IsDetached())
            {
                TForeignEntity foreignEntity = getFRef(RelatedEntity);
                TForeignKey? foreignKey = getFKey(RelatedEntity);

                byte vote = 0;

                if (foreignEntity.IsNull() || foreignEntity == ForeignEntity)
                    vote++;

                if (!foreignKey.HasValue
                 || (foreignKey.HasValue && foreignKeyManager.IsZero(foreignKey.Value))
                 || (foreignKey.HasValue && foreignKeyManager.HasSameKey(ForeignEntity, foreignKey.Value)))
                    vote++;

                if (vote == 2)
                    attach = true;
            }

            // AttachImp : Change State From Detached To Added or Unchanged
            // AddImp : First, Change State From Detached To Added or Unchanged Then If State Unchanged Set State As Modified
            // AddImp : Change State From Unchanged To Modified

            if (attach)
                AttachImp(RelatedEntity, ForeignEntity);
            else
                AddImp(RelatedEntity, ForeignEntity);

            return !attach;
        }
        public void Remove(TRelatedEntity RelatedEntity)
        {
#if Test || Test7
            if (RelatedEntity.IsNull()) throw new TestException($"[{this}], Remove Failed, RelatedEntity Is Null");
#endif
            RemoveImp(RelatedEntity);
        }
        public void Indexer(TRelatedEntity RelatedEntity, TRelatedEntity PerviousRelatedEntity, TForeignEntity ForeignEntity)
        {
#if Test || Test7
            if (RelatedEntity.IsNull()) throw new TestException($"[{this}], Indexer Failed, RelatedEntity Is Null");
            if (ForeignEntity.IsNull()) throw new TestException($"[{this}], Indexer Failed, Entity Is Null");
#endif
            Remove(PerviousRelatedEntity);

            Add(RelatedEntity, ForeignEntity);
        }




        private void AddImp(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            CanAdd(RelatedEntity, ForeignEntity);

            AddInternal(RelatedEntity, ForeignEntity);
        }
        private void CanAdd(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
#if Test || Test10
            if (!ForeignEntity.HasEditable())
                throw new TestException($"[{this}] Can Not Add {RelatedEntity}, {ForeignEntity} With {ForeignEntity.State} State Is Not Editable");
#endif
            try
            {
                if (!RelatedEntity.IsDetached())
                    relatedEntitySet.Accept(RelatedEntity);
            }
            catch (Exception e)
            {
                throw new EntityRelationManagerCanAddException($"[{this}] Can Not Add {RelatedEntity}", e, 211);
            }
        }
        private void AddInternal(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            if (RelatedEntity.IsDetached())
                relatedEntitySet.Add(RelatedEntity);

            AddInternalUpdateForeign(RelatedEntity, ForeignEntity);
        }
        private void AddInternalUpdateForeign(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            UpdateOnRemove(RelatedEntity); // RemovePreviousRelated must be called at first, because after call UpdateForeign previous foreign properties value will be losed
            UpdateForeign(RelatedEntity, ForeignEntity);
        }




        private void RemoveImp(TRelatedEntity RelatedEntity)
        {
            CanRemove(RelatedEntity);

            RemoveInternal(RelatedEntity);
        }
        private void CanRemove(TRelatedEntity RelatedEntity)
        {
            if (!isforeignKeyNullable)
                throw new EntityRelationManagerCanRemoveException($"[{this}], Can Not Remove {RelatedEntity}, [{propForeignReference.Name} Property] Is Not Nullable", 221);
        }
        private void RemoveInternal(TRelatedEntity RelatedEntity)
        {
            RemoveInternalUpdateForeign(RelatedEntity);
        }
        private void RemoveInternalUpdateForeign(TRelatedEntity RelatedEntity)
        {
            UpdateForeign(RelatedEntity);
        }




        private void AttachImp(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            AttachInternal(RelatedEntity, ForeignEntity);
        }
        private void AttachInternal(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity)
        {
            TForeignEntity foreignEntity = ForeignEntity;

            setFRef(RelatedEntity, foreignEntity);

            RelatedEntity.OnPropertyChanged(propForeignReference.Name);

            relatedEntitySet.Add(RelatedEntity);
        }


        #endregion Related Manager



        #region Unique Constraint

        private protected abstract bool HasUniqueConstraint { get; }
        private protected abstract bool IsUnique(TForeignKey ForeignKey);
        private protected abstract bool IsUnique(TForeignEntity ForeignEntity);

        private protected abstract void AddUniqueConstraint(TRelatedEntity RelatedEntity);
        private protected abstract void RemoveUniqueConstraint(TForeignKey? ForeignKey);

        #endregion Unique Constraint



        #region Late Add

        private protected abstract bool HasLateAdd { get; }

        private protected abstract void LateAddUpdateRelated(TRelatedEntity RelatedEntity);

        #endregion Late Add



        #region Other

        public EntityRelation EntityRelation => entityRelation;
        public EntitySet RelatedEntitySet => relatedEntitySet;
        public EntitySet ForeignEntitySet => foreignEntitySet;

        public override string ToString()
        {
            return $"{typeof(TForeignEntity).Name},{typeof(TRelatedEntity).Name} RelationManager";
        }

        #endregion Other
    }

    #region interface

    internal interface IEntityRelationManager
    {
        EntityRelation EntityRelation { get; }
        EntitySet RelatedEntitySet { get; }
        EntitySet ForeignEntitySet { get; }

    }

    internal interface IEntityRelationManager<TForeignEntity, TRelatedEntity> : IEntityRelationManager, IEntityRelationForeignManager<TRelatedEntity>, IEntityRelationRelatedManager<TForeignEntity> where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
        bool Add(TRelatedEntity RelatedEntity, TForeignEntity ForeignEntity);
        void Remove(TRelatedEntity RelatedEntity);
        void Indexer(TRelatedEntity RelatedEntity, TRelatedEntity PerviousRelatedEntity, TForeignEntity ForeignEntity);
    }

    internal interface IEntityRelationForeignManager<TRelatedEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
        #region Add

        ForeignStatus GetForeignStatus(TRelatedEntity RelatedEntity);
        void UpdateOnAdd(TRelatedEntity RelatedEntity, ForeignStatus ForeignStatus);

        #endregion Add

        #region Remove

        void UpdateOnRemove(TRelatedEntity RelatedEntity);

        #endregion Remove

        #region Edit

        ForeignStatus GetForeignStatus<TProperty>(TRelatedEntity RelatedEntity, EditedEntity<TRelatedEntity, TProperty> Edited);
        void UpdateOnEdit<TProperty>(TRelatedEntity RelatedEntity, EditedEntity<TRelatedEntity, TProperty> Edited, ForeignStatus ForeignStatus);

        #endregion Edit

        #region Change

        Entity GetDependency(TRelatedEntity RelatedEntity);

        int GetDependencyDepth(TRelatedEntity RelatedEntity);

        #endregion Change

        #region Cancel

        void UpdateOnCancel(TRelatedEntity RelatedEntity, EntityProxy<TRelatedEntity> NotCommittedData);

        #endregion Cancel
    }

    internal interface IEntityRelationRelatedManager<TForeignEntity> where TForeignEntity : Entity<TForeignEntity>
    {
        bool IsRelated(TForeignEntity ForeignEntity);

        void NotifyAllRelated(TForeignEntity ForeignEntity);
        void RestoreAllRelated(TForeignEntity ForeignEntity);

        void UpdatedAllOrphan(TForeignEntity ForeignEntity);
        void UpdatedAllRelated(TForeignEntity ForeignEntity);

        void ResetRelated(TForeignEntity ForeignEntity);
        void InitializeRelated(TForeignEntity ForeignEntity);
    }

    #endregion interface
}
