using System.ComponentModel;

namespace Abi.Data
{
    public abstract partial class Entity<T> : IEditableObject, INotifyPropertyChanged
    {
        public virtual void BeginEdit()
        {
            T entity = (T)this;

            if (entity.HasBusy())
                throw new CriticalException($"Entity<{Type.Name}> BeginEdit Failed, {entity} With {entity.State} State Is Not Supported");

            if (entity.HasEditable())
            {
                if (!editing_entities.Contains(entity))
                {
                    editing_entities.Add(entity);

                    Data.TransformBegin(entity);

                    EntitySet.EntityChangeTracker.TransformBegin(entity);
                }
            }
            else if (!entity.IsLateAdd())
                throw new CriticalException($"Entity<{Type.Name}> BeginEdit Failed, {entity} With {entity.State} State Is Not Supported");
        }
        public virtual void CancelEdit()
        {
            T entity = (T)this;

            if (editing_entities.Contains(entity))
            {
                editing_entities.Remove(entity);

                Data.TransformCancel(entity);

                EntitySet.EntityChangeTracker.TransformCancel(entity);

                OnPropertyChanged("");
            }
        }
        public virtual void EndEdit()
        {
            T entity = (T)this;

            if (entity.HasEditable())
            {
                editing_entities.Remove(entity);

                Data.TransformEnd(entity);

                EntitySet.EntityChangeTracker.TransformEnd(entity);
            }
            else if (entity.IsLateAdd())
                EntitySet.LateAddCommit(entity);
            else
                throw new CriticalException($"Entity<{Type.Name}> EndEdit Failed, {entity} With {entity.State} State Is Not Supported");
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

    }
}
