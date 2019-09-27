using System.Reflection;

namespace Abi.Data
{
    internal sealed class EntityKeyManager64<TEntity> : EntityKeyManager<TEntity, long> where TEntity : Entity<TEntity>
    {
        #region Constructor & Field

        internal EntityKeyManager64(PropertyInfo PropKey) : base(PropKey)
        {
        }

        #endregion Constructor & Field

        #region Add

        internal override bool CanAdd(TEntity Entity)
        {
            long EntityKey = GetKey(Entity);
            if (EntityKey == 0)
                if (key == long.MinValue)
                    return false;

            return true;
        }
        internal override void Add(TEntity Entity)
        {
            long EntityKey = GetKey(Entity);

            if (EntityKey == 0)
            {
                SetKey(Entity, GenerateNewKey());
                EntityKey = GetKey(Entity);
            }
            else if (EntityKey < 0 && EntityKey < key)
                UpdateGeneratedKey(EntityKey);

            keys.Add(EntityKey, Entity);
        }

        #endregion Add

        #region Remove

        internal override bool Remove(TEntity Entity)
        {
            return keys.Remove(GetKey(Entity));
        }

        #endregion Remove

        #region Utility

        // Utility Simple
        internal override bool IsNull(TEntity Entity)
        {
            return (Entity == null);
        }
        internal override bool IsZero(long EntityKey)
        {
            return EntityKey == 0;
        }
        internal override bool IsNew(long EntityKey)
        {
            return EntityKey < 0;
        }
        internal override bool IsExist(long EntityKey)
        {
            return EntityKey > 0;
        }

        // Utility That Just Use GetKey(T)
        internal override bool IsZero(TEntity Entity)
        {
            long EntityKey = GetKey(Entity);
            return EntityKey == 0;
        }
        internal override bool IsNew(TEntity Entity)
        {
            long EntityKey = GetKey(Entity);
            return EntityKey < 0;
        }
        internal override bool IsExist(TEntity Entity)
        {
            long EntityKey = GetKey(Entity);
            return EntityKey > 0;
        }
        internal override bool HasSameKey(TEntity Entity, long EntityKey)
        {
            return EntityKey == GetKey(Entity);
        }
        internal override bool HasDifferentKey(TEntity Entity, long EntityKey)
        {
            return EntityKey != GetKey(Entity);
        }

        // Utility Robust 
        internal override bool Find(TEntity Entity)
        {
            long entityKey = GetKey(Entity);

            if (!Find(entityKey)) return false;

            TEntity currentEntity = keys[entityKey];
            if (currentEntity != Entity)
                throw new EntityKeyManagerException($"{typeof(TEntity).Name} Key Manager Find Failed, {Entity} Has Key Same As {currentEntity}");
            else
                return true;
        }
        internal override bool Find(long EntityKey)
        {
            return keys.ContainsKey(EntityKey);
        }
        internal override long GetEntityKey(TEntity Entity)
        {
            return GetKey(Entity);
        }
        internal override TEntity GetEntity(long EntityKey)
        {
            return keys[EntityKey];
        }

        #endregion Utility

        #region Get & Set long

        protected override long GetKey(TEntity Entity)
        {
            return base.GetKey(Entity);
        }
        protected override void SetKey(TEntity Entity, long Key)
        {
            base.SetKey(Entity, Key);
        }

        protected override long GenerateNewKey()
        {
            return checked(--key);
        }

        #endregion Get & Set long

    }
}
