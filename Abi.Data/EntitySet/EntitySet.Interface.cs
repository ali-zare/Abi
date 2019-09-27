using System;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Abi.Data
{
    public abstract partial class EntitySet<TEntity> : IList<TEntity>, IList, ICollection<TEntity>, ICollection, IEnumerable<TEntity>, IEnumerable, IReadOnlyList<TEntity>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public TEntity this[int Index]
        {
            get => local[Index];
            set
            {
                CheckReentrancy();

                if (value.IsNull())
                    throw new EntitySetIndexerException($"EntitySet<{typeof(TEntity).Name}> this[{Index}] Failed, Value Is Null");

                TEntity Entity = local[Index];

                if (Entity == value) return;

                DetachImp(Entity);

                AddImp(value);

                local[Index] = value;

                OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, Entity, Index));
            }
        }


        public int Count => local.Count;
        public bool IsReadOnly => false;


        public void Add(TEntity Entity)
        {
            Insert(Count, Entity);
        }
        public bool Remove(TEntity Entity)
        {
            if (Entity.IsNull()) throw new EntitySetRemoveException($"EntitySet<{typeof(TEntity).Name}> Remove Failed, Entity Is Null");

            int index = IndexOf(Entity);

            if (index < 0) throw new EntitySetRemoveException($"EntitySet<{typeof(TEntity).Name}> Remove Failed, Entity Not Found");

            RemoveAt(index);

            return true;
        }
        public void Insert(int Index, TEntity Entity)
        {
            InsertInternal(Index, Entity);
        }
        public void RemoveAt(int Index)
        {
            CheckReentrancy();

            TEntity Entity = local[Index];

            if (Entity.IsLateAdd())
                Entity.Detached();
            else
                RemoveImp(Entity);

            local.RemoveAt(Index);

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Entity, Index));
        }

        public int IndexOf(TEntity Entity)
        {
            return local.IndexOf(Entity);
        }
        public bool Contains(TEntity Entity)
        {
            bool find = EntityKeyManager.Find(Entity);

            //this code cause bug : wpf datagrid PreviewExecuted > DeleteCommand > context.entityset.Remove(entity)
            //if (find && Entity.IsDeleted())
            //    throw new EntitySetContainsException($"EntitySet<{typeof(TEntity).Name}> Contains {Entity}, But It's State Is Deleted");

            if (find && Entity.IsDeleted()) return false;

            return find;
        }

        public void Clear()
        {
            CheckReentrancy();

            foreach (TEntity entity in EntityKeyManager)
                if (IsRelated(entity)) throw new EntitySetClearException($"EntitySet<{typeof(TEntity).Name}> Clear Failed, {entity} Is Related");

            foreach (TEntity entity in EntityKeyManager.ToArray())
            {
                entity.FailOnLock();

                RemoveInternalUpdateAllRelated(entity);

                EntityKeyManager.Remove(entity);

                entity.Detached();
            }

            local.Clear();
            EntityChangeTracker.Clear();
            Context.ChangeTracker.Clear<TEntity>();

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void CopyTo(TEntity[] Destination, int DestinationIndex)
        {
            local.CopyTo(Destination, DestinationIndex);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return local.GetEnumerator();
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



        private void InsertInternal(int Index, TEntity Entity, bool LateAdd = false)
        {
            CheckReentrancy();

            if (Entity.IsNull()) throw new EntitySetInsertException($"EntitySet<{typeof(TEntity).Name}> InsertInternal Failed, Entity Is Null");

            if (LateAdd)
            {
                if (!Entity.IsDetached())
                    throw new EntitySetInsertException($"{Entity} State Must Be Detached For Late Add");

                Entity.State = EntityState.LateAdd;
                Entity.EntitySet = this;
            }
            else
                AddImp(Entity);

            local.Insert(Index, Entity);

            OnPropertyChanged(new PropertyChangedEventArgs(CountString));
            OnPropertyChanged(new PropertyChangedEventArgs(IndexerName));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Entity, Index));
        }



        #region IList & IEnumerable

        private object _syncRoot;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (TEntity)value;
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

        public override int Add(object value)
        {
            int index = Count;

            Insert(index, value);

            return index;
        }
        public override void Remove(object value)
        {
            Remove((TEntity)value);
        }

        public void Insert(int index, object value)
        {
            InsertInternal(index, (TEntity)value, true);
        }

        public int IndexOf(object value)
        {
            return IndexOf((TEntity)value);
        }
        public bool Contains(object value)
        {
            return Contains((TEntity)value);
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


    public abstract partial class EntitySet
    {
        public abstract int Add(object value);
        public abstract void Remove(object value);
    }
}
