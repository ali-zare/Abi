using System;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    internal abstract class EntityKeyManager<TEntity, TKey> : EntityKeyManager<TEntity> where TEntity : Entity<TEntity>
    {
        #region Constructor & Field

        static EntityKeyManager()
        {
            accessors = new Dictionary<PropertyInfo, Delegate[]>();
            snapshot_key_of_entities = new Dictionary<TEntity, TKey>();
        }
        protected EntityKeyManager(PropertyInfo PropKey)
        {
            propKey = PropKey;

            keys = new Dictionary<TKey, TEntity>();

            #region Generate Property Setter And Getter

            if (!accessors.TryGetValue(PropKey, out Delegate[] accessor))
            {
                Type typeEntity = typeof(TEntity);
                Type typeData = EntityProxy<TEntity>.DataType;
                Type typeProxyData = typeof(EntityProxy<,>).MakeGenericType(new[] { typeEntity, typeData });

                MethodInfo propDataGetMthd = typeEntity.GetProperty("Data", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
                FieldInfo fldItem = typeProxyData.GetField("item");
                FieldInfo fldKey = typeData.GetField(PropKey.Name);

                ParameterExpression argEntity = Expression.Parameter(typeEntity, "Entity");

                MemberExpression expKey = Expression.Field(Expression.Field(Expression.TypeAs(Expression.Call(argEntity, propDataGetMthd), typeProxyData), fldItem), fldKey);

                accessors.Add(PropKey, accessor = new Delegate[2]);

                // Get Key Accessor
                accessor[0] = Expression.Lambda<Func<TEntity, TKey>>(expKey, argEntity).Compile();

                ParameterExpression argKey = Expression.Parameter(typeof(TKey), "Key");

                // Set Key Accessor
                accessor[1] = Expression.Lambda<Action<TEntity, TKey>>(Expression.Assign(expKey, argKey), argEntity, argKey).Compile();
            }

            getKey = (Func<TEntity, TKey>)accessor[0];
            setKey = (Action<TEntity, TKey>)accessor[1];

            #endregion Generate Property Settery And Getter

        }


        private static Dictionary<PropertyInfo, Delegate[]> accessors;
        private static Dictionary<TEntity, TKey> snapshot_key_of_entities;

        private Action<TEntity, TKey> setKey;
        private Func<TEntity, TKey> getKey;

        protected PropertyInfo propKey;

        protected TKey key;
        protected Dictionary<TKey, TEntity> keys;

        #endregion Constructor & Field

        #region Snapshot

        internal override void TakeSnapshot(TEntity Entity)
        {
            snapshot_key_of_entities.Add(Entity, getKey(Entity));
        }
        internal override void RestoreSnapshot(TEntity Entity)
        {
            Remove(Entity);

            setKey(Entity, snapshot_key_of_entities[Entity]);

            Add(Entity);

            snapshot_key_of_entities.Remove(Entity);
        }
        internal override void RemoveSnapshot(TEntity Entity)
        {
            snapshot_key_of_entities.Remove(Entity);
        }

        #endregion

        #region Utility

        // Utility Simple
        internal abstract bool IsNull(TEntity Entity);
        internal abstract bool IsZero(TKey EntityKey);
        internal abstract bool IsNew(TKey EntityKey);
        internal abstract bool IsExist(TKey EntityKey);

        // Utility That Just Use GetEntityKey(T)
        internal abstract bool HasSameKey(TEntity Entity, TKey EntityKey);
        internal abstract bool HasDifferentKey(TEntity Entity, TKey EntityKey);

        // Utility Robust 
        internal abstract bool Find(TKey EntityKey);
        internal abstract TKey GetEntityKey(TEntity Entity);
        internal abstract TEntity GetEntity(TKey EntityKey);

        #endregion Utility

        #region Get & Set TKey

        protected virtual TKey GetKey(TEntity Entity)
        {
            return getKey(Entity);
        }
        protected virtual void SetKey(TEntity Entity, TKey Key)
        {
            setKey(Entity, Key);
        }

        protected abstract TKey GenerateNewKey();
        protected void UpdateGeneratedKey(TKey LastKey)
        {
            key = LastKey;
        }

        #endregion Get & Set TKey

        #region Implement Interfaces

        public override IEnumerator<TEntity> GetEnumerator()
        {
            return keys.Values.GetEnumerator();
        }

        #endregion Implement Interfaces

        #region Other

        public override string ToString()
        {
            return $"{typeof(TEntity).Name} EntityKeyManager, ({propKey.Name} Key) Count {keys.Count}";
        }

        #endregion Other
    }

    internal abstract class EntityKeyManager<TEntity> : IEnumerable<TEntity> where TEntity : Entity<TEntity>
    {
        #region Constructor & Field

        protected EntityKeyManager()
        {
        }

        #endregion Constructor & Field

        #region Add

        internal abstract bool CanAdd(TEntity Entity);
        internal abstract void Add(TEntity Entity);

        #endregion Add

        #region Remove

        internal abstract bool Remove(TEntity Entity);

        #endregion Remove

        #region Snapshot

        internal abstract void TakeSnapshot(TEntity Entity);
        internal abstract void RestoreSnapshot(TEntity Entity);
        internal abstract void RemoveSnapshot(TEntity Entity);

        #endregion

        #region Utility

        // Utility That Just Use GetEntityKey(T)
        internal abstract bool IsZero(TEntity Entity);
        internal abstract bool IsNew(TEntity Entity);
        internal abstract bool IsExist(TEntity Entity);

        // Utility Robust 
        internal abstract bool Find(TEntity Entity);

        #endregion Utility

        #region Implement Interfaces

        public abstract IEnumerator<TEntity> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Implement Interfaces

        #region Other

        public override string ToString()
        {
            return $"{typeof(TEntity).Name} EntityKeyManager";
        }

        #endregion Other
    }
}
