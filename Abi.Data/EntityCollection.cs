using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Abi.Data
{
    internal sealed partial class EntityCollection<TForeignEntity, TRelatedEntity> : EntityCollection<TRelatedEntity> where TForeignEntity : Entity<TForeignEntity> where TRelatedEntity : Entity<TRelatedEntity>
    {
        internal EntityCollection()
        {
        }

        private TForeignEntity owner;
        private IOneToManyEntityRelationManager<TForeignEntity, TRelatedEntity> entityRelationManager;




        internal void InternalAdd(TRelatedEntity RelatedEntity)
        {
            storageInsert(Count, RelatedEntity);
        }
        internal void InternalRemove(TRelatedEntity RelatedEntity)
        {
            storageRemoveAt(IndexOf(RelatedEntity));
        }

        internal void Reset()
        {
            storage?.Clear(); // storge can be null if first owner entity be deleted, and then after delete detach that (owner) entity
            storage = null;

            entityRelationManager = null;
        }
        internal void Initialize(TForeignEntity Owner, IOneToManyEntityRelationManager<TForeignEntity, TRelatedEntity> EntityRelationManager)
        {
            owner = Owner;

            entityRelationManager = EntityRelationManager;

            storage = new List<TRelatedEntity>();
        }

        private protected override void entityInsert(int Index, TRelatedEntity RelatedEntity)
        {
            if (entityRelationManager.Add(RelatedEntity, owner))
                storageInsert(Index, RelatedEntity);
        }
        private protected override void entityRemoveAt(int Index)
        {
            TRelatedEntity RelatedEntity = storage[Index];

            entityRelationManager.Remove(RelatedEntity);

            storageRemoveAt(Index);
        }
        private protected override void entityIndexer(int Index, TRelatedEntity RelatedEntity)
        {
            TRelatedEntity PerviousRelatedEntity = storage[Index];

            entityRelationManager.Indexer(RelatedEntity, PerviousRelatedEntity, owner);

            storageIndexer(Index, RelatedEntity);
        }

        private protected override void entityLateAdd(TRelatedEntity RelatedEntity)
        {
            entityRelationManager.LateAdd(RelatedEntity, owner);
        }
        private protected override void entityLateAddCancel(TRelatedEntity RelatedEntity)
        {
            entityRelationManager.LateAddCancel(RelatedEntity);
        }




        private protected override bool IsInitialized => entityRelationManager != null;
        public override EntitySet EntitySet => entityRelationManager?.RelatedEntitySet;

        public override string ToString()
        {
            return $"{owner} {(entityRelationManager == null ? $"{typeof(TRelatedEntity).Name} Collection" : entityRelationManager.EntityRelation.PropRelated.Name)} Count {(storage == null ? 0 : storage.Count)}";
        }
    }








    public abstract partial class EntityCollection<TRelatedEntity> : EntityCollection where TRelatedEntity : Entity<TRelatedEntity>
    {
        private protected EntityCollection()
        {

        }




        private protected List<TRelatedEntity> storage;




        private protected void storageInsert(int Index, TRelatedEntity RelatedEntity)
        {
            CheckReentrancy();

            storage.Insert(Index, RelatedEntity);

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, RelatedEntity, Index));
        }
        private protected void storageRemoveAt(int Index)
        {
            if (storage == null)
                throw new CriticalException($"{this} Storage RemoveAt Failed, Internal Storage Is Not Initialized");

            CheckReentrancy();

            TRelatedEntity RelatedEntity = storage[Index];

            storage.RemoveAt(Index);

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, RelatedEntity, Index));
        }
        private protected void storageIndexer(int Index, TRelatedEntity RelatedEntity)
        {
            if (storage == null)
                throw new CriticalException($"{this} Storage RemoveAt Failed, Internal Storage Is Not Initialized");

            CheckReentrancy();

            TRelatedEntity PerviousRelatedEntity = storage[Index];

            storage[Index] = RelatedEntity;

            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, RelatedEntity, PerviousRelatedEntity, Index));
        }

        private protected abstract void entityInsert(int Index, TRelatedEntity RelatedEntity);
        private protected abstract void entityRemoveAt(int Index);
        private protected abstract void entityIndexer(int Index, TRelatedEntity RelatedEntity);

        private protected abstract void entityLateAdd(TRelatedEntity RelatedEntity);
        private protected abstract void entityLateAddCancel(TRelatedEntity RelatedEntity);




        private protected abstract bool IsInitialized { get; }
    }








    public abstract partial class EntityCollection
    {
        public abstract EntitySet EntitySet { get; }
    }
}
