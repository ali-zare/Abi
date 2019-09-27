using System;
using System.Collections.Generic;
using System.Text;

namespace Abi.Data
{
    public abstract partial class Entity<T>
    {
        public override void TakeSnapshot()
        {
            T entity = (T)this;

            if (entity.IsAdded())
            {
                snapshot_entityState_of_entities.Add(entity, entity.State);
                entity.EntitySet.EntityKeyManager.TakeSnapshot(entity);
                entity.Data.TakeSnapshot(entity);
            }
            else if (entity.IsModified())
            {
                snapshot_entityState_of_entities.Add(entity, entity.State);
                entity.EntitySet.EntityChangeTracker.TakeSnapshot(entity);
                entity.Data.TakeSnapshot(entity);
            }
            else if (entity.IsDeleted())
            {
                snapshot_entityState_of_entities.Add(entity, entity.State);
                snapshot_entitySet_of_entities.Add(entity, entity.EntitySet);
                entity.EntitySet.EntityKeyManager.TakeSnapshot(entity); // key of detached entity can be changed
            }
            else
                throw new EntitySnapshotException($"Entity<{Type.Name}> TakeSnapshot Failed, {entity} With {entity.State} State Is Not Supported");
        }

        public override void RestoreSnapshot()
        {
            T entity = (T)this;

            EntityState current_state = entity.State;

            if (!snapshot_entityState_of_entities.TryGetValue(entity, out EntityState snapshot_state))
                throw new EntitySnapshotException($"Entity<{Type.Name}> RestoreSnapshot Failed, Snapshot From {entity} Is Not Exist");

            switch (current_state)
            {
                case EntityState.Unchanged:

                    entity.State = snapshot_state;

                    if (entity.IsAdded())
                    {
                        entity.EntitySet.EntityKeyManager.RestoreSnapshot(entity);
                        entity.Data.RestoreSnapshot(entity);
                        entity.EntitySet.Context.ChangeTracker.Restore(entity);
                        entity.EntitySet.RestoreAllRelated(entity);

                        snapshot_entityState_of_entities.Remove(entity);

                        OnPropertyChanged("");
                    }
                    else if (entity.IsModified())
                    {

                        entity.EntitySet.EntityChangeTracker.RestoreSnapshot(entity);
                        entity.Data.RestoreSnapshot(entity);
                        entity.EntitySet.Context.ChangeTracker.Restore(entity);
                        entity.EntitySet.NotifyAllRelated(entity);

                        snapshot_entityState_of_entities.Remove(entity);

                        OnPropertyChanged("");
                    }
                    else
                        throw new EntitySnapshotException($"Entity<{Type.Name}> RestoreSnapshot Failed, {entity} With {entity.State} State Is Not Supported");
                    break;

                case EntityState.Detached:

                    entity.State = snapshot_state;

                    if (entity.IsDeleted())
                    {
                        entity.EntitySet = snapshot_entitySet_of_entities[entity];

                        entity.EntitySet.EntityKeyManager.RestoreSnapshot(entity);
                        entity.EntitySet.Context.ChangeTracker.Restore(entity);

                        snapshot_entityState_of_entities.Remove(entity);
                        snapshot_entitySet_of_entities.Remove(entity);
                    }
                    else
                        throw new EntitySnapshotException($"Entity<{Type.Name}> RestoreSnapshot Failed, {entity} With {entity.State} State Is Not Supported");

                    break;

                case EntityState.Added:
                case EntityState.Modified:
                case EntityState.Deleted:
                    break;

                default:
                    throw new EntitySnapshotException($"Entity<{Type.Name}> RestoreSnapshot Failed, {entity} With {entity.State} State Is Not Supported");
            }
        }

        public override void RemoveSnapshot()
        {
            T entity = (T)this;

            if (!snapshot_entityState_of_entities.TryGetValue(entity, out EntityState state))
                throw new EntitySnapshotException($"Entity<{Type.Name}> RemoveSnapshot Failed, Snapshot From {entity} Is Not Exist");

            switch (state)
            {
                case EntityState.Added:
                    entity.EntitySet.EntityKeyManager.RemoveSnapshot(entity);
                    entity.Data.RemoveSnapshot(entity);
                    break;

                case EntityState.Modified:
                    entity.EntitySet.EntityChangeTracker.RemoveSnapshot(entity);
                    entity.Data.RemoveSnapshot(entity);
                    break;

                case EntityState.Deleted:
                    EntitySet<T> entitySet = snapshot_entitySet_of_entities[entity];
                    entitySet.EntityKeyManager.RemoveSnapshot(entity);
                    snapshot_entitySet_of_entities.Remove(entity);
                    break;

                default:
                    throw new EntitySnapshotException($"Entity<{Type.Name}> RemoveSnapshot Failed, {entity} With {entity.State} State Is Not Supported");
            }

            snapshot_entityState_of_entities.Remove(entity);
        }
    }

    public abstract partial class Entity
    {
        public abstract void TakeSnapshot();
        public abstract void RestoreSnapshot();
        public abstract void RemoveSnapshot();
    }
}
