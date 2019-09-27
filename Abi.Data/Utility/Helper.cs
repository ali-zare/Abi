using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Abi.Data
{
    internal static class Helper
    {
        static Helper()
        {
        }

        private static Type[] propAcceptableKeyTypes => new Type[] { typeof(short), typeof(int), typeof(long) };
        private static Type[] propAcceptableTypes => new Type[] { typeof(bool), typeof(byte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal), typeof(char), typeof(string), typeof(byte[]), typeof(Guid), typeof(TimeSpan), typeof(DateTime), typeof(Types.RowVersion) };

        internal static bool HasAcceptableKeyType(this PropertyInfo Property)
        {
            foreach (Type propAcceptableKeyType in propAcceptableKeyTypes)
                if (Property.PropertyType == propAcceptableKeyType)
                    return true;

            return false;
        }

        internal static bool HasAcceptableType(this PropertyInfo Property)
        {
            foreach (Type propAcceptableType in propAcceptableTypes)
            {
                if (Property.PropertyType == propAcceptableType)
                    return true;

                if (propAcceptableType.IsValueType)
                    if (Property.IsNullableOfType(propAcceptableType))
                        return true;
            }

            return false;
        }
        internal static bool IsRelatedProperty(this PropertyInfo Property)
        {
            return Property.PropertyType.IsGenericType
                && Property.PropertyType.IsGenericTypeDefinition == false
                && (Property.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>)
                    || Property.PropertyType.GetGenericTypeDefinition() == typeof(EntityUnique<>));
        }
        internal static bool IsIgnored(this PropertyInfo Property)
        {
            return Attribute.IsDefined(Property, typeof(IgnoreAttribute));
        }
        internal static bool IsEnumerable(this PropertyInfo Property)
        {
            return typeof(IEnumerable).IsAssignableFrom(Property.PropertyType) && Property.PropertyType != typeof(string) && Property.PropertyType != typeof(byte[]);
        }

        internal static IEnumerable<PropertyInfo> GetEntityAllProperties(this Type EntityType)
        {
            return EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => (!p.DeclaringType.IsGenericType && p.DeclaringType != typeof(Entity))  // not include properties of Entity base class.
                                       || (p.DeclaringType.IsGenericType && p.DeclaringType.GetGenericTypeDefinition() != typeof(Entity<>))); // not include properties of Entity<T> base class.

            // return EntityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly); // don't use this approch, because of a class maybe has base class  other than Entity<T> or Entity (ex : Child > Parent > Entity<Child> > Entity)
        }
        internal static IEnumerable<PropertyInfo> GetEntityAllRelatedProperties(this Type EntityType)
        {
            return EntityType.GetEntityAllProperties().Where(prop => prop.IsRelatedProperty());
        }
        internal static IEnumerable<PropertyInfo> GetEntityAcceptableProperties(this Type EntityType)
        {
            return EntityType.GetEntityAllProperties().Where(prop => !prop.IsIgnored() && !prop.IsRelatedProperty() && !prop.IsEnumerable());
        }
        internal static IEnumerable<PropertyInfo> GetEntityAcceptableDataProperties(this Type EntityType)
        {
            return EntityType.GetEntityAllProperties()
                             .Where(Property => Property.HasAcceptableType())
                             .Where(Property => !Property.IsIgnored());
        }
    }
}
