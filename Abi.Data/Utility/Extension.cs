using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    internal static class Extension
    {
        internal static PropertyInfo GetPropInfo<T>(this Expression<Func<T, object>> expression)
        {
            if (expression == null) throw new ArgumentNullException();

            Expression body = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
                body = ((UnaryExpression)expression.Body).Operand;
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
                body = expression.Body;
            else
                throw new NotSupportedException();

            //return ((PropertyInfo)((MemberExpression)body).Member); // it return PropertyInfo with ReflectedType of base type (ex : return BaseType<Derived>.Property instead of DerivedType.Property)
            return typeof(T).GetProperty(((MemberExpression)body).Member.Name);
        }
        internal static PropertyInfo GetPropInfo<T, P>(this Expression<Func<T, P>> expression)
        {
            if (expression == null) throw new ArgumentNullException();

            return ((PropertyInfo)((MemberExpression)expression.Body).Member);
        }



        internal static F Get<T, F>(this ref T instance, FieldInfo field) where T : struct
        {
            return Get(ref instance, FastField<T, F>.Make(field));
        }
        internal static F Get<T, F>(this ref T instance, FastField<T, F> fastField) where T : struct
        {
            return fastField.Get(ref instance);
        }

        internal static void Set<T, F>(this ref T instance, FieldInfo field, F value) where T : struct
        {
            Set(ref instance, FastField<T, F>.Make(field), value);
        }
        internal static void Set<T, F>(this ref T instance, FastField<T, F> fastField, F value) where T : struct
        {
            fastField.Set(ref instance, value);
        }



        internal static bool Contains(this Type type, PropertyInfo Property)
        {
            foreach (PropertyInfo prop in type.GetProperties())
                if (prop == Property) return true;

            return false;
        }



        internal static bool IsTrack(this TrakMode EditMode)
        {
            return EditMode == TrakMode.Track;
        }
        internal static bool IsUnTrack(this TrakMode EditMode)
        {
            return EditMode == TrakMode.UnTrack;
        }



        internal static bool IsNull(this ForeignStatus ForeignStatus)
        {
            return ForeignStatus == ForeignStatus.Null;
        }
        internal static bool IsOrphan(this ForeignStatus ForeignStatus)
        {
            return ForeignStatus == ForeignStatus.Orphan;
        }
        internal static bool IsUpdateRef(this ForeignStatus ForeignStatus)
        {
            return ForeignStatus == ForeignStatus.UpdateRef;
        }
        internal static bool IsUpdateKey(this ForeignStatus ForeignStatus)
        {
            return ForeignStatus == ForeignStatus.UpdateKey;
        }
        internal static bool IsComplete(this ForeignStatus ForeignStatus)
        {
            return ForeignStatus == ForeignStatus.Complete;
        }
        internal static bool IsValid(this ForeignStatus ForeignStatus)
        {
            return !((ForeignStatus & ForeignStatus.Invalid) == ForeignStatus.Invalid);
        }
        internal static bool IsValid<TEntity>(this Dictionary<IEntityRelationForeignManager<TEntity>, ForeignStatus> ForeignStatuses) where TEntity : Entity<TEntity>
        {
            foreach (ForeignStatus foreignStatus in ForeignStatuses.Values)
                if ((foreignStatus & ForeignStatus.Invalid) == ForeignStatus.Invalid) return false;

            return true;
        }



        internal static EntityState BeginEdit(this EntityState State)
        {
            if ((State & EntityState.Editable) == EntityState.Editable
                && (State & EntityState.Busy) != EntityState.Busy)
                return State | EntityState.Editing;
            else
                throw new CriticalException($"Set State To Editing Failed, {State} State Is Not Supported");
        }
        internal static EntityState EndEdit(this EntityState State)
        {
            if ((State & EntityState.Editing) == EntityState.Editing)
                return State ^ EntityState.Editing;
            else
                throw new CriticalException($"Exit State From Editing Failed, {State} State Is Not Supported");
        }

        internal static bool HasEditable<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return (Entity.State & EntityState.Editable) == EntityState.Editable;
        }
        internal static bool HasTrackable<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return (Entity.State & EntityState.Trackable) == EntityState.Trackable;
        }
        internal static bool HasChanged<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return (Entity.State & EntityState.Changed) == EntityState.Changed;
        }
        internal static bool HasBusy<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return (Entity.State & EntityState.Busy) == EntityState.Busy;
        }
        internal static bool HasUnchanged<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return (Entity.State & EntityState.Unchanged) == EntityState.Unchanged;
        }
        internal static bool HasModified<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return (Entity.State & EntityState.Modified) == EntityState.Modified;
        }
        internal static bool HasAdded<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return (Entity.State & EntityState.Added) == EntityState.Added;
        }
        internal static bool HasDeleted<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return (Entity.State & EntityState.Deleted) == EntityState.Deleted;
        }

        internal static bool IsDetached<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return Entity.State == EntityState.Detached;
        }
        internal static bool IsUnchanged<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return Entity.State == EntityState.Unchanged;
        }
        internal static bool IsModified<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return Entity.State == EntityState.Modified;
        }
        internal static bool IsAdded<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return Entity.State == EntityState.Added;
        }
        internal static bool IsLateAdd<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return Entity.State == EntityState.LateAdd;
        }
        internal static bool IsDeleted<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return Entity.State == EntityState.Deleted;
        }
        internal static bool IsNull<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            return Entity == default(TEntity);
        }

        internal static bool HasModified(this Entity Entity)
        {
            return (Entity.State & EntityState.Modified) == EntityState.Modified;
        }
        internal static bool HasAdded(this Entity Entity)
        {
            return (Entity.State & EntityState.Added) == EntityState.Added;
        }
        internal static bool HasDeleted(this Entity Entity)
        {
            return (Entity.State & EntityState.Deleted) == EntityState.Deleted;
        }

        internal static bool IsEditingModified(this Entity Entity)
        {
            return Entity.State == (EntityState.Modified | EntityState.Editing);
        }
        internal static bool IsModified(this Entity Entity)
        {
            return Entity.State == EntityState.Modified;
        }
        internal static bool IsAdded(this Entity Entity)
        {
            return Entity.State == EntityState.Added;
        }
        internal static bool IsDeleted(this Entity Entity)
        {
            return Entity.State == EntityState.Deleted;
        }



        internal static void FailOnLock<TEntity>(this TEntity Entity) where TEntity : Entity<TEntity>
        {
            Entity.EntitySet?.Context.FailOnLock();
        }

        internal static void FailOnLock(this Entity Entity)
        {
            Entity.EntitySet?.Context.FailOnLock();
        }



        internal static bool HasSameTypeAs(this PropertyInfo property1, PropertyInfo property2)
        {
            return property1.PropertyType == property2.PropertyType;
        }
        internal static bool HasSameTypeAsNullable(this PropertyInfo property1, PropertyInfo property2)
        {
            return property1.PropertyType.IsGenericType                                         // ex : Nullable<> or Nullable<int> are acceptable
                && property1.PropertyType.IsGenericTypeDefinition == false                      // ex : Nullable<int> is just acceptable
                && property1.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)      // ex : Nullable<int> GenericTypeDefinition -> Nullable<>
                && property1.PropertyType.GenericTypeArguments[0] == property2.PropertyType;    // ex : GenericTypeArguments[0] -> int (review TestProject -> GenericTest -> PropertyVsMethod Test)
        }

        internal static bool IsNullableOfType(this PropertyInfo property, Type type)
        {
            return property.PropertyType.IsGenericType                                         // ex : Nullable<> or Nullable<int> are acceptable
                && property.PropertyType.IsGenericTypeDefinition == false                      // ex : Nullable<int> is just acceptable
                && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)      // ex : Nullable<int> GenericTypeDefinition -> Nullable<>
                && property.PropertyType.GenericTypeArguments[0] == type;                      // ex : GenericTypeArguments[0] -> int (review TestProject -> GenericTest -> PropertyVsMethod Test)
        }



        internal static bool IsOneToMany(this EntityRelationType EntityRelationType)
        {
            return EntityRelationType == EntityRelationType.OneToMany;
        }
        internal static bool IsOneToOne(this EntityRelationType EntityRelationType)
        {
            return EntityRelationType == EntityRelationType.OneToOne;
        }
    }
}