// Source Code Site : (Some Part of code are deleted or changed)
// Address : http://fastreflection.codeplex.com/
// Title   : Fast Reflection - Home

using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="P"></typeparam>
    internal class FastField<T, P> where T : struct
    {
        public FieldInfo Field { get; protected set; }
        public Type InstanceType { get; private set; }
        public Type ReturnType { get; private set; }

        public static FastField<T, P> Make(FieldInfo field)
        {
            if (_Cache.ContainsKey(field))
            {
                return _Cache[field];
            }
            else
            {
                var instanceType = typeof(T);
                var returnType = typeof(P);

                // make sure that (T)instance is valid
                if (!instanceType.IsAssignableFrom(field.DeclaringType))
                {
                    throw new Exception(string.Format("The type {0} cannot be assigned to values of type {1}", field.DeclaringType.Name, typeof(T).Name));
                }

                // make sure that (P)value is valid
                if (!returnType.IsAssignableFrom(field.FieldType))
                {
                    throw new Exception(string.Format("The type {0} cannot be assigned to values of type {1}", field.FieldType.Name, typeof(P).Name));
                }

                // create our fast field instance
                FastField<T, P> fastField = new FastField<T, P>(field, instanceType, returnType);

                // add it to the cache
                _Cache.Add(field, fastField);

                return fastField;
            }
        }
        private FastField(FieldInfo field, Type instanceType, Type returnType)
        {
            Field = field;
            InstanceType = instanceType;
            ReturnType = returnType;

            MakeDelegates();
        }

        private void MakeDelegates()
        {
            var instance = Expression.Parameter(InstanceType.MakeByRefType(), "instance");
            var value = Expression.Parameter(ReturnType, "value");

            Expression instance_field = Expression.MakeMemberAccess(instance, Field);

            Expression getExpression = null;
            Expression setExpression = null;

            if (ReturnType == Field.FieldType)
            {
                setExpression = Expression.Assign(instance_field, value);
                getExpression = instance_field;
            }
            else
            {
                if (Field.FieldType.IsValueType)
                {
                    setExpression = Expression.Assign(instance_field, Expression.Convert(value, Field.FieldType));
                    getExpression = Expression.Convert(instance_field, ReturnType);
                }
                else
                {
                    setExpression = Expression.Assign(getExpression, Expression.TypeAs(value, Field.FieldType));
                    getExpression = Expression.TypeAs(instance_field, ReturnType);
                }
            }

            Get = Expression.Lambda<FuncRef<T, P>>(getExpression, instance).Compile();
            Set = Expression.Lambda<ActionRef<T, P>>(setExpression, instance, value).Compile();
        }

        public FuncRef<T, P> Get { get; private set; }
        public ActionRef<T, P> Set { get; private set; }

        private static readonly Dictionary<FieldInfo, FastField<T, P>> _Cache = new Dictionary<FieldInfo, FastField<T, P>>();

    }

    internal delegate TResult FuncRef<T, TResult>(ref T arg) where T : struct;
    internal delegate void ActionRef<T1, T2>(ref T1 arg1, T2 arg2) where T1 : struct;
}