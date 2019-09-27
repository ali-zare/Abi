using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Abi.Data
{
    public abstract partial class EntityCollection<TRelatedEntity> : IList<TRelatedEntity>, IList, ICollection<TRelatedEntity>, ICollection, IEnumerable<TRelatedEntity>, IEnumerable, IReadOnlyList<TRelatedEntity>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public TRelatedEntity this[int Index]
        {
            get
            {
                if (storage == null)
                    throw new ArgumentOutOfRangeException();

                return storage[Index];
            }
            set
            {
                if (value == null)
                    throw new EntityCollectionIndexerException($"[{this}] this[{Index}] Failed, Entity Is Null");

                try
                {
                    TRelatedEntity RelatedEntity = this[Index];
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new EntityCollectionRemoveAtException($"[{this}] RemoveAt({Index}) Failed, Related Entity Not Found");
                }

                entityIndexer(Index, value);
            }
        }


        public int Count
        {
            get
            {
                if (storage == null)
                    return 0;

                return storage.Count;
            }
        }
        public bool IsReadOnly => false;


        public void Add(TRelatedEntity RelatedEntity)
        {
            Insert(Count, RelatedEntity);
        }
        public bool Remove(TRelatedEntity RelatedEntity)
        {
            if (RelatedEntity.IsNull()) throw new EntityCollectionRemoveException($"[{this}] Remove({RelatedEntity}) Failed, Related Entity Is Null");

            RemoveAt(IndexOf(RelatedEntity));

            return true;
        }

        public void Insert(int Index, TRelatedEntity RelatedEntity)
        {
            InsertInternal(Index, RelatedEntity);
        }
        public void RemoveAt(int Index)
        {
            TRelatedEntity RelatedEntity = default;

            try
            {
                RelatedEntity = this[Index];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new EntityCollectionRemoveAtException($"[{this}] RemoveAt({Index}) Failed, Related Entity Not Found");
            }

            if (RelatedEntity.IsLateAdd())
            {
                entityLateAddCancel(RelatedEntity);

                storageRemoveAt(Index);
            }
            else
                entityRemoveAt(Index);
        }

        public int IndexOf(TRelatedEntity RelatedEntity)
        {
            if (storage == null)
                return -1;

            return storage.IndexOf(RelatedEntity);
        }
        public bool Contains(TRelatedEntity RelatedEntity)
        {
            if (storage == null)
                return false;

            return storage.Contains(RelatedEntity);
        }

        public void Clear()
        {
            if (storage == null)
                return;

            CheckReentrancy();

            foreach (TRelatedEntity relatedEntity in storage.ToArray()) Remove(relatedEntity);

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void CopyTo(TRelatedEntity[] Destination, int DestinationIndex)
        {
            storage?.CopyTo(Destination, DestinationIndex);
        }

        public IEnumerator<TRelatedEntity> GetEnumerator()
        {
            if (storage == null)
                return Enumerable.Empty<TRelatedEntity>().GetEnumerator();

            return storage.GetEnumerator();
        }




        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                using (BlockReentrancy())
                {
                    CollectionChanged(this, e);
                }
            }
        }


        private IDisposable BlockReentrancy()
        {
            _monitor.Enter();
            return _monitor;
        }
        private void CheckReentrancy()
        {
            if (_monitor.Busy)
            {
                // we can allow changes if there's only one listener - the problem
                // only arises if reentrant changes make the original event args
                // invalid for later listeners.  This keeps existing code working
                // (e.g. Selector.SelectedItems).
                if ((CollectionChanged != null) && (CollectionChanged.GetInvocationList().Length > 1))
                    throw new InvalidOperationException();
            }
        }

        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
        private SimpleMonitor _monitor = new SimpleMonitor();



        private void InsertInternal(int Index, TRelatedEntity RelatedEntity, bool LateAdd = false)
        {
            if (!IsInitialized)
                throw new EntityCollectionInsertException($"[{this}] Insert({Index}, {RelatedEntity}) Failed, {this} Is Not Initialized");

            if (RelatedEntity.IsNull())
                throw new EntityCollectionInsertException($"[{this}] Insert({Index}, {RelatedEntity}) Failed, Related Entity Is Null");

            if (!Contains(RelatedEntity))
                if (LateAdd)
                {
                    if (!RelatedEntity.IsDetached())
                        throw new EntitySetInsertException($"{RelatedEntity} State Must Be Detached For Late Add");

                    entityLateAdd(RelatedEntity);

                    storageInsert(Index, RelatedEntity);
                }
                else
                    entityInsert(Index, RelatedEntity);
            else
                throw new EntityCollectionInsertException($"[{this}] Insert({Index}, {RelatedEntity}) Failed, {RelatedEntity} Aleardy Exist");
        }



        #region IList & IEnumerable

        private object _syncRoot;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (TRelatedEntity)value;
        }

        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

        public int Add(object value)
        {
            int index = Count;

            Insert(index, value);

            return index;
        }
        public void Remove(object value)
        {
            Remove((TRelatedEntity)value);
        }

        public void Insert(int index, object value)
        {
            InsertInternal(index, (TRelatedEntity)value, LateAdd: true);
        }

        public int IndexOf(object value)
        {
            return IndexOf((TRelatedEntity)value);
        }
        public bool Contains(object value)
        {
            return Contains((TRelatedEntity)value);
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IList & IEnumerable
    }
}
