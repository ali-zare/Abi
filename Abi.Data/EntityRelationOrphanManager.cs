using System.Collections.Generic;

namespace Abi.Data
{
    internal sealed class EntityRelationOrphanManager<TRelatedEntity, TForeignKey> where TRelatedEntity : Entity<TRelatedEntity>
    {
        internal EntityRelationOrphanManager()
        {
            internalManager = new Dictionary<TForeignKey, HashSet<TRelatedEntity>>();
        }

        private Dictionary<TForeignKey, HashSet<TRelatedEntity>> internalManager;

        internal void Add(TForeignKey EntityKey, TRelatedEntity OrphanEntity)
        {
            if (!internalManager.TryGetValue(EntityKey, out HashSet<TRelatedEntity> orphanage))
                internalManager.Add(EntityKey, orphanage = new HashSet<TRelatedEntity>());

            orphanage.Add(OrphanEntity);
        }
        internal void Remove(TForeignKey EntityKey, TRelatedEntity OrphanEntity)
        {
            internalManager[EntityKey].Remove(OrphanEntity);

            if (internalManager[EntityKey].Count == 0)
                internalManager.Remove(EntityKey);
        }

        internal IEnumerable<TRelatedEntity> GetAllRelated(TForeignKey EntityKey)
        {
            if (internalManager.TryGetValue(EntityKey, out HashSet<TRelatedEntity> orphanage))
                foreach (TRelatedEntity Entity in orphanage)
                    yield return Entity;

            yield break;
        }
        internal int Count(TForeignKey EntityKey)
        {
            if (internalManager.TryGetValue(EntityKey, out HashSet<TRelatedEntity> orphanage))
                return orphanage.Count;

            return 0;
        }

        public override string ToString()
        {
            return $"{typeof(TRelatedEntity).Name} Orphange Count {internalManager.Count}";
        }
    }
}
