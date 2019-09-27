using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;


namespace Abi.Data
{
    internal sealed class EntityChangeTracker<TEntity, TBitBuffer> : EntityChangeTracker<TEntity> where TEntity : Entity<TEntity> where TBitBuffer : unmanaged
    {
        static EntityChangeTracker()
        {
            trackableProperties = Entity<TEntity>.AcceptableDataProperties;

            Type typeBitBuffer = typeof(TBitBuffer);

            var instance = Expression.Parameter(typeBitBuffer.MakeByRefType(), "buffer");

            clear = Expression.Lambda<ActionUnMngRef<TBitBuffer>>(Expression.Call(instance, typeBitBuffer.GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance)), instance).Compile();
            isSet = Expression.Lambda<FuncUnMng<TBitBuffer, bool>>(Expression.Call(instance, typeBitBuffer.GetMethod("IsSet", BindingFlags.Public | BindingFlags.Instance)), instance).Compile();

            patterns = new Dictionary<TBitBuffer, int>();

            original_buffer_of_entities = new Dictionary<TEntity, TBitBuffer>();
            snapshot_buffer_of_entities = new Dictionary<TEntity, TBitBuffer>();
        }
        internal EntityChangeTracker()
        {
            changedEntities = new Dictionary<TEntity, TBitBuffer>();
        }

        private static int patternID;
        private static Dictionary<TBitBuffer, int> patterns;
        private static PropertyInfo[] trackableProperties;
        private static ActionUnMngRef<TBitBuffer> clear;
        private static FuncUnMng<TBitBuffer, bool> isSet;
        private static Dictionary<TEntity, TBitBuffer> original_buffer_of_entities;
        private static Dictionary<TEntity, TBitBuffer> snapshot_buffer_of_entities;

        private Dictionary<TEntity, TBitBuffer> changedEntities;

        internal override void Clear()
        {
            changedEntities.Clear();
        }

        internal override void Remove(TEntity Entity)
        {
            //if (changedEntities.ContainsKey(Entity))
            changedEntities.Remove(Entity);
        }

        internal override bool IsTracked(TEntity Entity, PropertyInfo Property)
        {
            if (Entity.HasModified())
                if (bitPatternOfProperties.TryGetValue(Property, out int bitIndex))
                {
                    TBitBuffer bitBuffer = changedEntities[Entity];
                    return bitBuffer.IsSetted(bitIndex);
                }

            return false;
        }
        internal override void Track(TEntity Entity, PropertyInfo Property)
        {
            Entity.FailOnLock();

            if (Entity.HasTrackable())
                if (!Entity.EntitySet.Context.IsExceptedToTrack(Property))
                    if (bitPatternOfProperties.TryGetValue(Property, out int bitIndex))
                    {
                        if (!changedEntities.TryGetValue(Entity, out TBitBuffer bitBuffer))
                            changedEntities.Add(Entity, bitBuffer = default);

                        bitBuffer.Set(bitIndex);
                        changedEntities[Entity] = bitBuffer;

                        if (Entity.HasUnchanged())
                        {
                            Entity.State ^= EntityState.Unchanged;
                            Entity.State |= EntityState.Modified;
                            Entity.EntitySet.Context.ChangeTracker.Track(Entity);
                        }
                    }
        }
        internal override void UnTrack(TEntity Entity, PropertyInfo Property)
        {
            Entity.FailOnLock();

            if (Entity.HasModified()) // it means that changedEntities must contains Entity
                if (!Entity.EntitySet.Context.IsExceptedToTrack(Property))
                    if (bitPatternOfProperties.TryGetValue(Property, out int bitIndex))
                    {
                        TBitBuffer bitBuffer = changedEntities[Entity];
                        bitBuffer.Reset(bitIndex);
                        changedEntities[Entity] = bitBuffer;

                        if (!isSet(ref bitBuffer))
                        {
                            changedEntities.Remove(Entity);

                            Entity.EntitySet.Context.ChangeTracker.UnTrack(Entity); // this line of code must be executed first, before state change to (Editing | Unchanged)
                            Entity.State = (Entity.State & EntityState.Editing) | EntityState.Unchanged;
                        }
                    }
        }
        internal override void UnTrack(TEntity Entity)
        {
            Entity.FailOnLock();

            if (Entity.IsModified()) // it means that changedEntities must contains Entity
                Remove(Entity);
        }

        internal override void TransformBegin(TEntity Entity)
        {
            if (Entity.HasTrackable())
                if (changedEntities.TryGetValue(Entity, out TBitBuffer bitBuffer)) // if changedEntities contains Entity, it means that Entity is Modified
                    original_buffer_of_entities.Add(Entity, bitBuffer);
        }
        internal override void TransformEnd(TEntity Entity)
        {
            original_buffer_of_entities.Remove(Entity);
        }
        internal override void TransformCancel(TEntity Entity)
        {
            if (Entity.HasTrackable())
                if (original_buffer_of_entities.TryGetValue(Entity, out TBitBuffer bitBuffer)) // if original_buffer_of_entities contains Entity, it means that Entity is Modified
                {
                    original_buffer_of_entities.Remove(Entity);

                    if (Entity.IsUnchanged()) // Unchanged -> Modified
                    {
                        changedEntities.Add(Entity, bitBuffer);

                        Entity.State = EntityState.Modified | EntityState.Editing;

                        Entity.EntitySet.Context.ChangeTracker.Track(Entity);

                        Entity.State = EntityState.Modified;
                    }
                    else if (Entity.IsModified()) // Modified -> Modified
                        changedEntities[Entity] = bitBuffer;
                    else
                        throw new CriticalException($"EntityChangeTracker<{typeof(TEntity).Name}> CancelEdit Failed, {Entity} With {Entity.State} State Is Not Supported");
                }
                else if (Entity.IsModified()) // Modified -> Unchanged
                {
                    changedEntities.Remove(Entity);

                    Entity.EntitySet.Context.ChangeTracker.UnTrack(Entity); // this line of code must be executed first, before state change

                    Entity.State = EntityState.Unchanged;
                }
        }

        internal override void TakeSnapshot(TEntity Entity)
        {
            snapshot_buffer_of_entities.Add(Entity, changedEntities[Entity]);
        }
        internal override void RestoreSnapshot(TEntity Entity)
        {
            if (changedEntities.ContainsKey(Entity))
                changedEntities[Entity] = snapshot_buffer_of_entities[Entity];
            else
                changedEntities.Add(Entity, snapshot_buffer_of_entities[Entity]);

            snapshot_buffer_of_entities.Remove(Entity);
        }
        internal override void RemoveSnapshot(TEntity Entity)
        {
            snapshot_buffer_of_entities.Remove(Entity);
        }

        internal override IEnumerable<PropertyInfo> GetChanges(TEntity Entity)
        {
            if (changedEntities.TryGetValue(Entity, out TBitBuffer bitBuffer))
            {
                return bitBuffer.ToByteArray().GetSetted(trackableProperties);
            }
            else
                return Enumerable.Empty<PropertyInfo>();
        }


        internal override int GetPatternID(TEntity Entity)
        {
            TBitBuffer bitBuffer = changedEntities[Entity];

            if (!patterns.TryGetValue(bitBuffer, out int id))
                patterns.Add(bitBuffer, id = patternID++);

            return id;
        }



        public override string ToString()
        {
            return $"{typeof(TEntity).Name} EntityChangeTracker, ({typeof(TBitBuffer).Name}) Count {changedEntities.Count}";
        }
    }








    internal abstract class EntityChangeTracker<TEntity> where TEntity : Entity<TEntity>
    {
        static EntityChangeTracker()
        {
            Type typeEntity = Entity<TEntity>.Type;

            PropertyInfo[] trackableProperties = Entity<TEntity>.AcceptableDataProperties;

            // Step 1 :

            bitPatternOfProperties = new Dictionary<PropertyInfo, int>();
            propertiesBitPattern = new Dictionary<int, PropertyInfo>();

            int index = 0;

            foreach (PropertyInfo prop in trackableProperties)
            {
                bitPatternOfProperties.Add(prop, index);
                propertiesBitPattern.Add(index++, prop);
            }

            // Step 2 :

            Type typeBitBuffer = BitBufferHelper.GetBitBufferType(trackableProperties.Length);

            Type entityChangeTracker = typeof(EntityChangeTracker<,>).MakeGenericType(typeEntity, typeBitBuffer);
            ConstructorInfo ctr = entityChangeTracker.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);

            funcChangeTrackerCreator = Expression.Lambda<Func<EntityChangeTracker<TEntity>>>(Expression.New(ctr)).Compile();
        }

        private protected static Dictionary<PropertyInfo, int> bitPatternOfProperties;
        private protected static Dictionary<int, PropertyInfo> propertiesBitPattern;

        private static Func<EntityChangeTracker<TEntity>> funcChangeTrackerCreator;


        internal static EntityChangeTracker<TEntity> Create()
        {
            return funcChangeTrackerCreator();
        }


        internal abstract void Clear();

        internal abstract void Remove(TEntity Entity);

        internal abstract bool IsTracked(TEntity Entity, PropertyInfo Property);
        internal abstract void Track(TEntity Entity, PropertyInfo Property);
        internal abstract void UnTrack(TEntity Entity, PropertyInfo Property);
        internal abstract void UnTrack(TEntity Entity);

        internal abstract void TransformBegin(TEntity Entity);
        internal abstract void TransformEnd(TEntity Entity);
        internal abstract void TransformCancel(TEntity Entity);

        internal abstract void TakeSnapshot(TEntity Entity);
        internal abstract void RestoreSnapshot(TEntity Entity);
        internal abstract void RemoveSnapshot(TEntity Entity);

        internal abstract IEnumerable<PropertyInfo> GetChanges(TEntity Entity);


        internal abstract int GetPatternID(TEntity Entity);



        public override string ToString()
        {
            return $"{typeof(TEntity).Name} EntityChangeTracker";
        }
    }








    internal delegate void ActionUnMngRef<T>(ref T arg) where T : unmanaged;
    internal delegate TResult FuncUnMng<T, TResult>(ref T arg) where T : unmanaged;
}
