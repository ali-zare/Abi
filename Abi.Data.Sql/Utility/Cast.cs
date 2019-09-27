using System;
using System.Linq.Expressions;

namespace Abi.Data.Sql
{
    internal class Cast<T>
    {
        static Cast()
        {
            ParameterExpression expValue = Expression.Parameter(typeof(object), "Value");

            cast = Expression.Lambda<Func<object, T>>(Expression.Call(typeof(T).GetExecuteScalarGetValueMethod(), expValue), expValue).Compile();
        }


        private static Func<object, T> cast;


        internal static T Scalar(object Value) => cast(Value);
    }
}
