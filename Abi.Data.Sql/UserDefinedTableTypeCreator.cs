using System;
using System.Linq.Expressions;

namespace Abi.Data.Sql
{
    public sealed class UserDefinedTableTypeCreator<T> 
    {
        public static UserDefinedTableType<TItem> Create<TItem>(string TableTypeName, Expression<Func<T, TItem>> Item)
        {
            if (TableTypeName == null)
                throw new ArgumentNullException("Table Create Failed, Name Argument Is Null");

            if (Item == null)
                throw new ArgumentNullException("Table Create Failed, Item Argument Is Null");

            return new UserDefinedTableType<TItem>(TableTypeName);
        }
    }








    public sealed class UserDefinedTableTypeCreator
    {
        public static UserDefinedTableType<TItem> Create<TItem>(string TableTypeName, Expression<Func<TItem>> Item)
        {
            if (TableTypeName == null)
                throw new ArgumentNullException("Table Create Failed, Name Argument Is Null");

            if (Item == null)
                throw new ArgumentNullException("Table Create Failed, Item Argument Is Null");

            return new UserDefinedTableType<TItem>(TableTypeName);
        }
        public static UserDefinedTableType<TItem> Create<TItem>(string TableTypeName = null)
        {
            return new UserDefinedTableType<TItem>(TableTypeName);
        }
    }
}
