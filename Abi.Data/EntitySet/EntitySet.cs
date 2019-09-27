using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Abi.Data
{
    public abstract partial class EntitySet<TEntity> : EntitySet where TEntity : Entity<TEntity>
    {
        private protected EntitySet()
        {
        }
        private protected EntitySet(EntityContext Context)
        {
            this.Context = Context;

            Key = Context.Configuration.EntityKeys.GetPropEntityKey(typeof(TEntity));

            if (Key == null) throw new CriticalException($"EntitySet<{typeof(TEntity).Name}> Property Entity Key Is Null");

            local = new List<TEntity>();
            EntityChangeTracker = EntityChangeTracker<TEntity>.Create();
        }

        private List<TEntity> local;
        private Dictionary<EntityRelation, IEntityRelationForeignManager<TEntity>> entityRelationForeignManagers;
        private Dictionary<EntityRelation, IEntityRelationRelatedManager<TEntity>> entityRelationRelatedManagers;

        internal override void Initialize()
        {
            entityRelationForeignManagers = new Dictionary<EntityRelation, IEntityRelationForeignManager<TEntity>>();
            foreach (EntityRelation relation in Context.Configuration.EntityRelations.GetAllForeign(typeof(TEntity)))
                entityRelationForeignManagers.Add(relation, (IEntityRelationForeignManager<TEntity>)Context.GetEntityRelationManager(relation));

            entityRelationRelatedManagers = new Dictionary<EntityRelation, IEntityRelationRelatedManager<TEntity>>();
            foreach (EntityRelation relation in Context.Configuration.EntityRelations.GetAllRelated(typeof(TEntity)))
                entityRelationRelatedManagers.Add(relation, (IEntityRelationRelatedManager<TEntity>)Context.GetEntityRelationManager(relation));
        }

        public override PropertyInfo Key { get; }
        public override EntityContext Context { get; }
        public override Type KeyType => Key.PropertyType;
        public override Type EntityType => typeof(TEntity);

        public IEnumerable<TEntity> Changes => Context.ChangeTracker.Enumerate<TEntity>();

        internal EntityKeyManager<TEntity> EntityKeyManager { get; private protected set; }
        internal EntityChangeTracker<TEntity> EntityChangeTracker { get; }




        private void AddImp(TEntity Entity)
        {
            CanAdd(Entity, out Dictionary<IEntityRelationForeignManager<TEntity>, ForeignStatus> ForeignStatuses);

            AddInternal(Entity, ForeignStatuses);
        }
        private void CanAdd(TEntity Entity, out Dictionary<IEntityRelationForeignManager<TEntity>, ForeignStatus> ForeignStatuses)
        {
            if (!Entity.IsDetached()) throw new EntitySetCanAddException($"{EntityType.Name} EntitySet Add Failed, {Entity} State Must Be Detached", 111);

            if (Entity.EntitySet != null) throw new CriticalException($"{EntityType.Name} EntitySet Add Failed, {Entity} Is Contained In Other EntitySet");

            if (EntityKeyManager.Find(Entity)) throw new CriticalException($"{EntityType.Name} EntitySet Add Failed, Key Manager Find {Entity}");

            if (!EntityKeyManager.CanAdd(Entity)) throw new EntitySetCanAddException($"{EntityType.Name} EntitySet Add Failed, Key Manager, New Key For {Entity} Has OverFlow", 112);

            ForeignStatuses = GetAllForeignStatuses(Entity);

            if (!ForeignStatuses.IsValid()) throw new EntitySetCanAddException($"{Entity} Has InValid Foreign[es] : " + string.Join(" | ", ForeignStatuses.Values), 113);
        }
        private void AddInternal(TEntity Entity, Dictionary<IEntityRelationForeignManager<TEntity>, ForeignStatus> ForeignStatuses)
        {
            Entity.EntitySet = this;

            EntityKeyManager.Add(Entity);

            AddInternalUpdateAllForeign(Entity, ForeignStatuses);
            AddInternalUpdateAllRelated(Entity);

            if (EntityKeyManager.IsExist(Entity))
            {
                Entity.State = EntityState.Unchanged;

                UpdatedAllOrphan(Entity);

                Entity.OnPropertyChanged(Key.Name);
            }
            else if (EntityKeyManager.IsNew(Entity))
            {
                Entity.State = EntityState.Added;

                Context.ChangeTracker.Track(Entity);

                Entity.OnPropertyChanged(Key.Name);
            }
            else
                throw new CriticalException($"AddInternal {Entity}, Can Not Set EntityState");
        }
        private void AddInternalUpdateAllForeign(TEntity Entity, Dictionary<IEntityRelationForeignManager<TEntity>, ForeignStatus> ForeignStatuses)
        {
            foreach (KeyValuePair<IEntityRelationForeignManager<TEntity>, ForeignStatus> pair in ForeignStatuses)
            {
                ForeignStatus foreignStatus = pair.Value;
                IEntityRelationForeignManager<TEntity> entityRelationForeignManager = pair.Key;

                entityRelationForeignManager.UpdateOnAdd(Entity, foreignStatus);
            }
        }
        private void AddInternalUpdateAllRelated(TEntity Entity)
        {
            foreach (IEntityRelationRelatedManager<TEntity> entityRelationRelatedManager in entityRelationRelatedManagers.Values)
                entityRelationRelatedManager.InitializeRelated(Entity);
        }

        private void RemoveImp(TEntity Entity)
        {
            CanRemove(Entity);

            RemoveInternal(Entity);
        }
        private void CanRemove(TEntity Entity)
        {
            if (!Entity.HasEditable() || Entity.HasBusy()) throw new EntitySetCanRemoveException($"{EntityType.Name} EntitySet Remove Failed, {Entity} State Must Be Unchanged Or Added Or Modified", 121);

            if (Entity.EntitySet != this) throw new CriticalException($"{EntityType.Name} EntitySet Remove Failed, {Entity} Is Contained In Other EntitySet");

            if (!EntityKeyManager.Find(Entity)) throw new CriticalException($"{EntityType.Name} EntitySet Remove Failed, Key Manager Not Find {Entity}");

            if (IsRelated(Entity)) throw new EntitySetCanRemoveException($"{EntityType.Name} EntitySet Remove Failed, {Entity} Is Related", 123);
        }
        private void RemoveInternal(TEntity Entity)
        {
            RemoveInternalUpdateAllRelated(Entity);

            if (Entity.HasTrackable())
            {
                EntityChangeTracker.UnTrack(Entity);

                if (Entity.IsModified())
                    Context.ChangeTracker.UnTrack(Entity); // remove from modified_entities

                Entity.State = EntityState.Deleted;

                Context.ChangeTracker.Track(Entity); // this line of code must be executed after, state changes to deleted
            }
            else if (Entity.IsAdded())
            {
                Context.ChangeTracker.UnTrack(Entity);

                EntityKeyManager.Remove(Entity);

                Entity.Detached();
            }
            else
                throw new CriticalException($"RemoveInternal {Entity}, State Must Be Unchanged Or Added Or Modified");
        }
        private void RemoveInternalUpdateAllRelated(TEntity Entity)
        {
            foreach (IEntityRelationRelatedManager<TEntity> entityRelationRelatedManager in entityRelationRelatedManagers.Values)
                entityRelationRelatedManager.ResetRelated(Entity);

            foreach (IEntityRelationForeignManager<TEntity> entityRelationForeignManager in entityRelationForeignManagers.Values)
                entityRelationForeignManager.UpdateOnRemove(Entity);
        }

        private void EditImp<TProperty>(TEntity Entity, EditedEntity<TEntity, TProperty> Edited)
        {
            CanEdit(Entity, Edited, out ForeignStatus ForeignStatus);

            EditInternal(Entity, Edited, ForeignStatus);
        }
        private void CanEdit<TProperty>(TEntity Entity, EditedEntity<TEntity, TProperty> Edited, out ForeignStatus ForeignStatus)
        {
            if (!typeof(TEntity).Contains(Edited.Property))
                throw new CriticalException($"{Entity.GetType()} Not Contains Property {Edited.Property.Name}");

            if (Edited.Property == Key)
                throw new EntitySetEditException($"Can't Edit Key Property, {Entity.GetType()}.{Edited.Property.Name}");

            ForeignStatus = ForeignStatus.Unkhown;

            if (Edited.EntityRelationForeignManager != null)
                CanEditForeign(Entity, Edited, out ForeignStatus);
        }
        private void CanEditForeign<TProperty>(TEntity Entity, EditedEntity<TEntity, TProperty> Edited, out ForeignStatus ForeignStatus)
        {
            ForeignStatus = GetForeignStatus(Entity, Edited);

            if (!ForeignStatus.IsValid()) throw new EntitySetCanEditException($"{Entity} Property {Edited.Property.Name} {ForeignStatus}");
        }
        private void EditInternal<TProperty>(TEntity Entity, EditedEntity<TEntity, TProperty> Edited, ForeignStatus ForeignStatus)
        {
            Entity.State = Entity.State.BeginEdit();

            if (Edited.EntityRelationForeignManager == null)
            {
                Entity.Data.Set(Edited.Property, Edited.Value.New);

                if (Edited.TrakMode.IsTrack())
                    EntityChangeTracker.Track(Entity, Edited.Property);
                else if (Edited.TrakMode.IsUnTrack())
                    EntityChangeTracker.UnTrack(Entity, Edited.Property);

                Entity.OnPropertyChanged(Edited.Property.Name);
            }
            else
                Edited.EntityRelationForeignManager.UpdateOnEdit(Entity, Edited, ForeignStatus);

            Entity.State = Entity.State.EndEdit();

            EditInternalNotifyAllRelated(Entity);
        }
        private void EditInternalNotifyAllRelated(TEntity Entity)
        {
            NotifyAllRelated(Entity);
        }

        private void DetachImp(TEntity Entity)
        {
            CanDetach(Entity);

            DetachInternal(Entity);
        }
        private void CanDetach(TEntity Entity)
        {
            if (Entity.IsDetached() || Entity.HasBusy()) throw new EntitySetCanDetachException($"{EntityType.Name} EntitySet Detach Failed, {Entity} With {Entity.State} State Is Not Supported", 131);

            if (Entity.EntitySet != this) throw new EntitySetCanDetachException($"{EntityType.Name} EntitySet Detach Failed, {Entity} Is Contained In Other EntitySet", 132);

            if (!EntityKeyManager.Find(Entity)) throw new CriticalException($"{EntityType.Name} EntitySet Detach Failed, Key Manager Not Find {Entity}");

            if (IsRelated(Entity)) throw new EntitySetCanDetachException($"{EntityType.Name} EntitySet Detach Failed, {Entity} Is Related", 133);
        }
        private void DetachInternal(TEntity Entity)
        {
            DetachInternalUpdateAllRelated(Entity);

            EntityKeyManager.Remove(Entity);

            EntityChangeTracker.UnTrack(Entity);

            if (!Entity.IsUnchanged())
                Context.ChangeTracker.UnTrack(Entity);

            Entity.Detached();
        }
        private void DetachInternalUpdateAllRelated(TEntity Entity)
        {
            RemoveInternalUpdateAllRelated(Entity);
        }


        internal void Accept(TEntity Entity)
        {
            if (Entity.EntitySet != this)
                throw new EntitySetAcceptEntityException($"{Entity} EntitySet Not Equal To This EntitySet");

            if (!Entity.HasEditable())
                throw new EntitySetAcceptEntityException($"{Entity} Is Not Editable, State Must Be Unchanged Or Added Or Modified");

            if (!EntityKeyManager.Find(Entity))
                throw new EntitySetAcceptEntityException($"{typeof(TEntity).Name} Key Manager Not Find {Entity}");
        }
        internal bool IsRelated(TEntity Entity)
        {
            foreach (IEntityRelationRelatedManager<TEntity> entityRelationRelatedManager in entityRelationRelatedManagers.Values)
                if (entityRelationRelatedManager.IsRelated(Entity))
                    return true;

            return false;
        }


        private ForeignStatus GetForeignStatus<TProperty>(TEntity Entity, EditedEntity<TEntity, TProperty> Edited)
        {
            return Edited.EntityRelationForeignManager.GetForeignStatus(Entity, Edited);
        }
        private Dictionary<IEntityRelationForeignManager<TEntity>, ForeignStatus> GetAllForeignStatuses(TEntity Entity, bool BearkIfFindInvalid = true)
        {
            Dictionary<IEntityRelationForeignManager<TEntity>, ForeignStatus> foreignStatuses = new Dictionary<IEntityRelationForeignManager<TEntity>, ForeignStatus>();

            foreach (IEntityRelationForeignManager<TEntity> entityRelationForeignManager in entityRelationForeignManagers.Values)
            {
                ForeignStatus foreignStatus = entityRelationForeignManager.GetForeignStatus(Entity);

                foreignStatuses.Add(entityRelationForeignManager, foreignStatus);

                if ((foreignStatus & ForeignStatus.Invalid) == ForeignStatus.Invalid)
                    if (BearkIfFindInvalid)
                        return foreignStatuses;
            }

            return foreignStatuses;
        }



        private void UpdatedAllOrphan(TEntity Entity)
        {
            foreach (IEntityRelationRelatedManager<TEntity> entityRelationRelatedManager in entityRelationRelatedManagers.Values)
                entityRelationRelatedManager.UpdatedAllOrphan(Entity);
        }

        internal void UpdatedAllRelated(TEntity Entity)
        {
            foreach (IEntityRelationRelatedManager<TEntity> entityRelationRelatedManager in entityRelationRelatedManagers.Values)
                entityRelationRelatedManager.UpdatedAllRelated(Entity);
        }

        internal void NotifyAllRelated(TEntity Entity)
        {
            foreach (IEntityRelationRelatedManager<TEntity> entityRelationRelatedManager in entityRelationRelatedManagers.Values)
                entityRelationRelatedManager.NotifyAllRelated(Entity);
        }
        internal void RestoreAllRelated(TEntity Entity)
        {
            foreach (IEntityRelationRelatedManager<TEntity> entityRelationRelatedManager in entityRelationRelatedManagers.Values)
                entityRelationRelatedManager.RestoreAllRelated(Entity);
        }



        public void Detach(TEntity Entity)
        {
            if (Entity.IsNull()) throw new EntitySetDetachException($"EntitySet<{typeof(TEntity).Name}> Detach Failed, Entity Is Null");

            CheckReentrancy();

            DetachImp(Entity);

            int Index = IndexOf(Entity);

            if (Index >= 0) // deleted entity also can be detached, to prevent remove from database, this entity doesn't exist in local
            {
                local.RemoveAt(Index);

                OnPropertyChanged(new PropertyChangedEventArgs(CountString));
                OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Entity, Index));
            }
        }
        
        internal void Edit<TProperty>(TEntity Entity, EditedEntity<TEntity, TProperty> Edited)
        {
            EntityRelation relation = GetEntityRelation(Edited.Property);
            Edited.EntityRelationForeignManager = relation == null ? null : entityRelationForeignManagers[relation];

            EditImp(Entity, Edited);
        }
        internal void CallBack<TProperty>(TEntity Entity, EditedEntity<TEntity, TProperty> Edited)
        {
            IEntityRelationForeignManager<TEntity> EntityRelationForeignManager = entityRelationForeignManagers[GetEntityRelation(Edited.Property)];

            ForeignStatus foreignStatus = EntityRelationForeignManager.GetForeignStatus(Entity, Edited);

            EntityRelationForeignManager.UpdateOnEdit(Entity, Edited, foreignStatus);
        }
        internal void Cancel(TEntity Entity, EntityProxy<TEntity> NotCommittedData)
        {
            foreach (IEntityRelationForeignManager<TEntity> entityRelationForeignManager in entityRelationForeignManagers.Values)
                entityRelationForeignManager.UpdateOnCancel(Entity, NotCommittedData);
        }
        internal void LateAddInsert(TEntity Entity)
        {
            int Index = local.Count;
            local.Insert(Index, Entity);

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Entity, Index));
        }
        internal void LateAddCommit(TEntity Entity)
        {
            Entity.Detached();

            foreach (IEntityRelationForeignManager<TEntity> entityRelationForeignManager in entityRelationForeignManagers.Values)
                if (entityRelationForeignManager is IOneToManyEntityRelationForeignManager<TEntity> oneToManyEntityRelationForeignManager)
                    oneToManyEntityRelationForeignManager.LateAddCommit(Entity);

            AddImp(Entity);
        }
        internal void LateAddCancel(TEntity Entity)
        {
            Entity.Detached();

            int Index = local.IndexOf(Entity);
            local.RemoveAt(Index);

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Entity, Index));
        }
        internal int GetDependencyDepth(TEntity Entity)
        {
            int Depth = Context.ChangeTracker.GetDependencyDepth(Entity);

            if (Depth > -1) return Depth;

            Depth = 0;

            foreach (IEntityRelationForeignManager<TEntity> entityRelationForeignManager in entityRelationForeignManagers.Values)
            {
                int ForeignDepth = entityRelationForeignManager.GetDependencyDepth(Entity);

                if (ForeignDepth > -1 && Depth <= ForeignDepth)
                    Depth = ForeignDepth + 1;
            }

            Context.ChangeTracker.SetDependencyDepth(Entity, Depth);

            return Depth;
        }
        internal Entity GetDependency(TEntity Entity, PropertyInfo Property)
        {
            EntityRelation relation = GetEntityRelation(Property);

            if (relation == null)
                return null;
            
            IEntityRelationForeignManager<TEntity> EntityRelationForeignManager = entityRelationForeignManagers[relation];

            return EntityRelationForeignManager.GetDependency(Entity);
        }

        private EntityRelation GetEntityRelation(PropertyInfo Property)
        {
            return Context.Configuration.EntityRelations.GetEntityRelation(Property);
        }



        public override string ToString()
        {
            return $"{EntityType.Name} EntitySet, Items Count {local.Count}";
        }
    }

    public abstract partial class EntitySet
    {
        internal EntitySet() { }

        internal abstract void Initialize();

        public abstract Type KeyType { get; }
        public abstract Type EntityType { get; }
        public abstract PropertyInfo Key { get; }
        public abstract EntityContext Context { get; }
    }
}
