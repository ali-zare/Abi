using System.Reflection;

namespace Abi.Data
{
    internal sealed class EntityKeyManager16<TEntity> : EntityKeyManager<TEntity, short> where TEntity : Entity<TEntity>
    {
        #region Constructor & Field

        internal EntityKeyManager16(PropertyInfo PropKey) : base(PropKey)
        {
        }

        #endregion Constructor & Field

        #region Add

        internal override bool CanAdd(TEntity Entity)
        {
            short EntityKey = GetKey(Entity);
            if (EntityKey == 0)
                if (key == short.MinValue)
                    return false;

            return true;
        }
        internal override void Add(TEntity Entity)
        {
            short EntityKey = GetKey(Entity);

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
        internal override bool IsZero(short EntityKey)
        {
            return EntityKey == 0;
        }
        internal override bool IsNew(short EntityKey)
        {
            return EntityKey < 0;
        }
        internal override bool IsExist(short EntityKey)
        {
            return EntityKey > 0;
        }

        // Utility That Just Use GetKey(T)
        internal override bool IsZero(TEntity Entity)
        {
            short EntityKey = GetKey(Entity);
            return EntityKey == 0;
        }
        internal override bool IsNew(TEntity Entity)
        {
            short EntityKey = GetKey(Entity);
            return EntityKey < 0;
        }
        internal override bool IsExist(TEntity Entity)
        {
            short EntityKey = GetKey(Entity);
            return EntityKey > 0;
        }
        internal override bool HasSameKey(TEntity Entity, short EntityKey)
        {
            return EntityKey == GetKey(Entity);
        }
        internal override bool HasDifferentKey(TEntity Entity, short EntityKey)
        {
            return EntityKey != GetKey(Entity);
        }

        // Utility Robust 
        internal override bool Find(TEntity Entity)
        {
            short entityKey = GetKey(Entity);

            if (!Find(entityKey)) return false;

            TEntity currentEntity = keys[entityKey];
            if (currentEntity != Entity)
                throw new EntityKeyManagerException($"{typeof(TEntity).Name} Key Manager Find Failed, {Entity} Has Key Same As {currentEntity}");
            else
                return true;
        }
        internal override bool Find(short EntityKey)
        {
            return keys.ContainsKey(EntityKey);
        }
        internal override short GetEntityKey(TEntity Entity)
        {
            return GetKey(Entity);
        }
        internal override TEntity GetEntity(short EntityKey)
        {
            return keys[EntityKey];
        }

        #endregion Utility

        #region Get & Set short

        protected override short GetKey(TEntity Entity)
        {
            return base.GetKey(Entity);
        }
        protected override void SetKey(TEntity Entity, short Key)
        {
            base.SetKey(Entity, Key);
        }

        protected override short GenerateNewKey()
        {
            return checked(--key);
        }

        #endregion Get & Set short

    }
}
