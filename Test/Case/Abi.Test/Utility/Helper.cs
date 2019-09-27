using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using Abi.Data;

namespace Abi.Test
{
    internal static class Helper
    {
        static Helper()
        {
        }


        internal static Exception Next(this Exception exception)
        {
            exception = exception.InnerException;

            return exception;
        }





        internal static void Check<TEntity>(this TEntity entity) where TEntity : Entity<TEntity>
        {
            if (entity.State == EntityState.Detached) entity.ThrowStateException();
        }
        internal static void CheckAdded<TEntity>(this TEntity entity) where TEntity : Entity<TEntity>
        {
            if (entity.State != EntityState.Added) entity.ThrowStateException();
        }
        internal static void CheckModified<TEntity>(this TEntity entity) where TEntity : Entity<TEntity>
        {
            if (entity.State != EntityState.Modified) entity.ThrowStateException();
        }
        internal static void CheckUnchanged<TEntity>(this TEntity entity) where TEntity : Entity<TEntity>
        {
            if (entity.State != EntityState.Unchanged) entity.ThrowStateException();
        }
        internal static void CheckDeleted<TEntity>(this TEntity entity) where TEntity : Entity<TEntity>
        {
            if (entity.State != EntityState.Deleted) entity.ThrowStateException();
        }
        internal static void CheckDetached<TEntity>(this TEntity entity) where TEntity : Entity<TEntity>
        {
            if (entity.State != EntityState.Detached) entity.ThrowStateException();
        }

        private static void ThrowStateException<TEntity>(this TEntity entity) where TEntity : Entity<TEntity>
        {
            throw new NotExpectedResultException($"{entity} State Is {entity.State}");
        }




        internal static EntityCollection<TRelatedEntity> CheckCount<TRelatedEntity>(this EntityCollection<TRelatedEntity> EntityCollection, int Count) where TRelatedEntity : Entity<TRelatedEntity>
        {
            if (EntityCollection.Count != Count)
                EntityCollection.ThrowEntityCollectionException($"Expect Count {Count}");

            return EntityCollection;
        }
        internal static EntityCollection<TRelatedEntity> CheckItem<TRelatedEntity>(this EntityCollection<TRelatedEntity> EntityCollection, int Index, TRelatedEntity RelatedEntity) where TRelatedEntity : Entity<TRelatedEntity>
        {
            if (EntityCollection[Index] != RelatedEntity)
                EntityCollection.ThrowEntityCollectionException($"Expect Item With Index {Index} Be {RelatedEntity}");

            return EntityCollection;
        }
        private static void ThrowEntityCollectionException<TRelatedEntity>(this EntityCollection<TRelatedEntity> EntityCollection, string Message) where TRelatedEntity : Entity<TRelatedEntity>
        {
            throw new NotExpectedResultException($"{EntityCollection}, {Message}");
        }




        internal static void CheckItem<TRelatedEntity>(this EntityUnique<TRelatedEntity> EntityUnique, TRelatedEntity RelatedEntity) where TRelatedEntity : Entity<TRelatedEntity>
        {
            if (EntityUnique.Entity != RelatedEntity)
                EntityUnique.ThrowEntityUniqueException($"Expect Item Be {RelatedEntity}");
        }
        private static void ThrowEntityUniqueException<TRelatedEntity>(this EntityUnique<TRelatedEntity> EntityUnique, string Message) where TRelatedEntity : Entity<TRelatedEntity>
        {
            throw new NotExpectedResultException($"{EntityUnique}, {Message}");
        }




        internal static void Check<T>(this T T1, T T2)
        {
            if (!Equals(T1, T2))
                T1.ThrowEqualException(T2);
        }
        private static void ThrowEqualException<T>(this T T1, T T2)
        {
            throw new NotExpectedResultException($"{T1} Is Not Equal To {T2}");
        }



        internal static IEnumerable<T> CheckCount<T>(this IEnumerable<T> Items, int Count)
        {
            if (Items.ToArray().Length != Count) Items.ThrowEnumerationException($"Expect Count {Count}");

            return Items;
        }
        internal static IEnumerable<T> CheckFound<T>(this IEnumerable<T> Items, T Item)
        {
            foreach (T item in Items)
                if (Equals(item, Item)) return Items;

            Items.ThrowEnumerationException($"{Item} Not Found");

            return Items;
        }
        private static void ThrowEnumerationException<T>(this IEnumerable<T> Items, string Message)
        {
            throw new NotExpectedResultException($"{Items.GetType().Name}, {Message}");
        }



        internal static IEnumerable<PropertyInfo> CheckFound<TEntity>(this IEnumerable<PropertyInfo> Items, Expression<Func<TEntity, object>> ItemExpression) where TEntity : Entity<TEntity>
        {
            Expression body;

            if (ItemExpression.Body.NodeType == ExpressionType.Convert)
                body = ((UnaryExpression)ItemExpression.Body).Operand;
            else if (ItemExpression.Body.NodeType == ExpressionType.MemberAccess)
                body = ItemExpression.Body;
            else
                throw new NotSupportedException($"CheckFound {typeof(TEntity).Name} Property Failed, {ItemExpression} Is Not Supported");

            PropertyInfo Item = typeof(TEntity).GetProperty(((MemberExpression)body).Member.Name);

            return Items.CheckFound(Item);
        }

    }
}
