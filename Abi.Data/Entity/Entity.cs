using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Abi.Data
{
    public abstract partial class Entity<T> : Entity where T : Entity<T>
    {
        protected Entity()
        {
            Data = EntityProxy<T>.Create();

            initializeRelatedProperties?.Invoke((T)this);
        }

        internal EntityProxy<T> Data { get; }



        protected P Get<P>(Expression<Func<T, P>> Expression) => Get<P>(Expression.GetPropInfo());
        protected P Get<P>([CallerMemberName] string PropertyName = "") => Get<P>(GetType().GetProperty(PropertyName));
        internal protected P Get<P>(PropertyInfo Property) => Data.Get<P>(Property);

        protected void Set<P>(Expression<Func<T, P>> Expression, P Value) => Set(Expression.GetPropInfo(), Value);
        protected void Set<P>(P Value, [CallerMemberName] string PropertyName = "") => Set(GetType().GetProperty(PropertyName), Value);
        protected void Set<P>(PropertyInfo Property, P Value)
        {
            T entity = (T)this;

            if (entity.HasBusy() || entity.IsDeleted())
                throw new EntityPropertySetException($"{entity} Set {Type.Name}.{Property.Name} Failed, It's State Is {entity.State}");

            P currentValue = Data.Get<P>(Property);

            if (!Equals(Value, currentValue))
            {
                if (entity.HasEditable())
                    EntitySet.Edit(entity, new EditedEntity<T, P>(new EditedProperty<P>(Property, currentValue, Value)));
                else
                {
                    Data.Set(Property, Value);
                    OnPropertyChanged(Property.Name);
                }
            }
        }



        public void Reset<P>(Expression<Func<T, P>> Expression, P Value) => Reset(Expression.GetPropInfo(), Value);
        public void Reset<P>(PropertyInfo Property, P Value)
        {
            T entity = (T)this;

            if (!entity.HasTrackable() || entity.HasBusy()) // Trackable Can Be Busy
                throw new EntityPropertySetException($"{entity} Reset {Type.Name}.{Property.Name} Failed, It's State Is {entity.State}");

            P currentValue = Data.Get<P>(Property);

            if (!Equals(Value, currentValue))
                EntitySet.Edit(entity, new EditedEntity<T, P>(new EditedProperty<P>(Property, currentValue, Value), TrakMode.UnTrack));
            else // ex : in case load property with same value as previously loaded couse property to be unchanged.
                EntitySet.EntityChangeTracker.UnTrack(entity, Property);
        }



        public void Load<P>(Expression<Func<T, P>> Expression, P Value) => Load(Expression.GetPropInfo(), Value);
        public void Load<P>(PropertyInfo Property, P Value)
        {
            T entity = (T)this;

            if (!entity.HasTrackable() || entity.HasBusy()) // Trackable Can Be Busy
                throw new EntityPropertySetException($"{entity} Load {Type.Name}.{Property.Name} Failed, It's State Is {entity.State}");

            P currentValue = Data.Get<P>(Property);

            if (!Equals(Value, currentValue))
                if (!EntitySet.EntityChangeTracker.IsTracked(entity, Property))
                    EntitySet.Edit(entity, new EditedEntity<T, P>(new EditedProperty<P>(Property, currentValue, Value), TrakMode.None));
        }



        internal void Detached()
        {
            EntitySet = null;
            State = EntityState.Detached;
        }
        internal void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(PropertyName));
        }



        private protected override EntitySet GetEntitySet()
        {
            return EntitySet;
        }

        public override bool IsModified(PropertyInfo Property)
        {
            T entity = (T)this;

            if (entity.HasBusy())
                throw new EntityPropertySetException($"{entity} Set {Type.Name}.{Property.Name} Failed, It's State Is {entity.State}");

            return entity.HasModified() ? entity.EntitySet.EntityChangeTracker.IsTracked(entity, Property) : false;
        }
        public override IEnumerable<PropertyInfo> GetChangedProperties()
        {
            T entity = (T)this;

            if (entity.HasModified())
                return entity.EntitySet.EntityChangeTracker.GetChanges(entity);
            else
                return Enumerable.Empty<PropertyInfo>();
        }



        public override bool IsRelated
        {
            get
            {
                if (EntitySet != null)
                    return EntitySet.IsRelated((T)this);
                else
                    return false;
            }
        }
        public override Type EntityType => Type;
        public override EntityState State { get; internal set; }
        public new EntitySet<T> EntitySet { get; internal set; }
    }

    public abstract partial class Entity
    {
        private protected abstract EntitySet GetEntitySet();

        public abstract bool IsModified(PropertyInfo Property);
        public abstract IEnumerable<PropertyInfo> GetChangedProperties();

        public abstract bool IsRelated { get; }
        public abstract Type EntityType { get; }
        public abstract EntityState State { get; internal set; }

        public EntitySet EntitySet => GetEntitySet();
    }
}
