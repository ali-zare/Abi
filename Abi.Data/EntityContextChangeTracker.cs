using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    internal sealed class EntityContextChangeTracker : EntityContextChanges, IEnumerable<Entity>
    {
        static EntityContextChangeTracker()
        {
            get_dependency_depth_funcs = new Dictionary<Type, Func<Entity, int>>();
        }
        internal EntityContextChangeTracker()
        {
            added_entities = new List<Entity>();
            deleted_entities = new List<Entity>();
            modified_entities = new List<Entity>();
        }



        private static Dictionary<Type, Func<Entity, int>> get_dependency_depth_funcs;

        private List<Entity> added_entities;
        private List<Entity> deleted_entities;
        private List<Entity> modified_entities;

        private Dictionary<Entity, int> dependency_depth_of_entities;



        public override int Count => added_entities.Count + modified_entities.Count + deleted_entities.Count;



        internal void Track(Entity Entity)
        {
            Entity.FailOnLock();

            if (Entity.IsAdded())
                added_entities.Add(Entity);

            else if (Entity.IsEditingModified())
                modified_entities.Add(Entity);

            else if (Entity.IsDeleted())
                deleted_entities.Add(Entity);

            else
                throw new CriticalException($"ContextChangeTracker Track {Entity} Failed, {Entity.State} State Is Not Supported");

            OnPropertyChanged(nameof(Count));
        }
        internal void UnTrack(Entity Entity)
        {
            Entity.FailOnLock();

            Remove(Entity);
        }
        internal void Remove(Entity Entity)
        {
            if (Entity.HasAdded())
                added_entities.Remove(Entity);

            else if (Entity.HasModified())
                modified_entities.Remove(Entity);

            else if (Entity.HasDeleted())
                deleted_entities.Remove(Entity);

            else
                throw new CriticalException($"ContextChangeTracker Remove {Entity} Failed, {Entity.State} State Is Not Supported");

            OnPropertyChanged(nameof(Count));
        }
        internal void Restore(Entity Entity)
        {
            if (Entity.IsAdded())
                added_entities.Add(Entity);
            else if (Entity.IsModified())
                modified_entities.Add(Entity);
            else if (Entity.IsDeleted())
                deleted_entities.Add(Entity);
            else
                throw new CriticalException($"ContextChangeTracker Track {Entity} Failed, {Entity.State} State Is Not Supported");

            OnPropertyChanged(nameof(Count));
        }

        internal void Clear<TEntity>() where TEntity : Entity<TEntity>
        {
            foreach (TEntity entity in Enumerate<TEntity>().ToArray())
            {
                if (entity.IsAdded())
                    added_entities.Remove(entity);

                else if (entity.IsModified())
                    modified_entities.Remove(entity);

                else if (entity.IsDeleted())
                    deleted_entities.Remove(entity);

                else
                    throw new CriticalException($"ContextChangeTracker Clear {entity} Failed, {entity.State} State Is Not Supported");
            }

            OnPropertyChanged(nameof(Count));
        }

        internal int GetDependencyDepth(Entity Entity)
        {
            if (dependency_depth_of_entities.TryGetValue(Entity, out int Depth))
                return Depth;
            else
                return -1;
        }
        internal void SetDependencyDepth(Entity Entity, int Depth)
        {
            dependency_depth_of_entities.Add(Entity, Depth);
        }



        private static Func<Entity, int> generate_get_dependency_depth_func(Type type)
        {
            Type typeEntity = type;
            Type typeEntitySet = typeof(EntitySet<>).MakeGenericType(typeEntity);

            MethodInfo propEntitySetGetMthd = typeEntity.GetProperty("EntitySet", typeEntitySet).GetGetMethod(true);
            MethodInfo mthdGetDependencyDepth = typeEntitySet.GetMethod("GetDependencyDepth", BindingFlags.NonPublic | BindingFlags.Instance);

            ParameterExpression argEntity = Expression.Parameter(typeof(Entity), "Entity");

            return Expression.Lambda<Func<Entity, int>>(Expression.Call(Expression.Call(Expression.TypeAs(argEntity, typeEntity), propEntitySetGetMthd), mthdGetDependencyDepth, Expression.TypeAs(argEntity, typeEntity)), argEntity).Compile();
        }

        internal IEnumerable<TEntity> Enumerate<TEntity>() where TEntity : Entity<TEntity>
        {
            IEnumerator<Entity> changed_entities = GetEnumerator();

            while (changed_entities.MoveNext())
            {
                Entity entity = changed_entities.Current;

                if (entity.EntityType == typeof(TEntity))
                    yield return (TEntity)entity;
            }
        }


        #region IEnumerable

        public override IEnumerator<Entity> GetEnumerator()
        {
            dependency_depth_of_entities = new Dictionary<Entity, int>();

            foreach (Entity added_entity in added_entities)
            {
                if (!get_dependency_depth_funcs.TryGetValue(added_entity.EntityType, out Func<Entity, int> get_dependency_depth))
                    get_dependency_depth_funcs.Add(added_entity.EntityType, get_dependency_depth = generate_get_dependency_depth_func(added_entity.EntityType));

                get_dependency_depth(added_entity);
            }

            IEnumerable<Entity> ordered_added_entities = added_entities.OrderBy(e => GetDependencyDepth(e)).ToList();

            dependency_depth_of_entities.Clear();
            dependency_depth_of_entities = null;

            IEnumerator<Entity> added_entities_enumerator = ordered_added_entities.GetEnumerator();
            while (added_entities_enumerator.MoveNext())
                yield return added_entities_enumerator.Current;

            IEnumerator<Entity> modified_entities_enumerator = modified_entities.GetEnumerator();
            while (modified_entities_enumerator.MoveNext())
                yield return modified_entities_enumerator.Current;

            IEnumerator<Entity> deleted__entities_enumerator = deleted_entities.GetEnumerator();
            while (deleted__entities_enumerator.MoveNext())
                yield return deleted__entities_enumerator.Current;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable

        #region Other

        public override string ToString()
        {
            return $"ContextChangeTracker, Changes Count {Count}";
        }

        #endregion Other
    }








    public abstract class EntityContextChanges : IEnumerable<Entity>, INotifyPropertyChanged
    {
        private protected EntityContextChanges()
        {
        }



        public abstract int Count { get; }



        public event PropertyChangedEventHandler PropertyChanged;
        private protected void OnPropertyChanged(string PropertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(PropertyName));
        }
        private protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }


        #region IEnumerable

        public abstract IEnumerator<Entity> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable
    }
}
