using System;
using System.Threading;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    internal sealed class DatabaseReaderSelect<T> : DatabaseReader
    {
        static DatabaseReaderSelect()
        {
            CallBackDataSetters = new Dictionary<CommandToken<T>, Func<SqlDataReader, T>>();
        }
        internal DatabaseReaderSelect() { }


        private static Dictionary<CommandToken<T>, Func<SqlDataReader, T>> CallBackDataSetters;


        internal static T[] ToArray(SqlDataReader Reader)
        {
            int size = 64;
            int index = 0;
            T[] items = new T[size];

            CommandToken<T> token = new CommandToken<T>(Reader.GetSchemaTable());

            if (!CallBackDataSetters.TryGetValue(token, out Func<SqlDataReader, T> SetCallBackData))
                CallBackDataSetters.Add(token, SetCallBackData = token.GenerateCallBackDataSetter());

            while (Reader.Read())
            {
                T item = SetCallBackData(Reader);

                items[index++] = item;

                if (index == size)
                {
                    size *= 2;
                    T[] new_items = new T[size];
                    Array.Copy(items, 0, new_items, 0, items.Length);
                    items = new_items;
                }
            }

            T[] result_items = new T[index];
            Array.Copy(items, 0, result_items, 0, index);
            items = result_items;

            return items;
        }
        internal static T First(SqlDataReader Reader)
        {
            CommandToken<T> token = new CommandToken<T>(Reader.GetSchemaTable());

            if (!CallBackDataSetters.TryGetValue(token, out Func<SqlDataReader, T> SetCallBackData))
                CallBackDataSetters.Add(token, SetCallBackData = token.GenerateCallBackDataSetter());

            Reader.Read();

            return SetCallBackData(Reader);
        }


        internal static T[] ToArrayAsync(SqlDataReader Reader, CancellationToken CancellationToken)
        {
            int size = 64;
            int index = 0;
            T[] items = new T[size];

            CommandToken<T> token = new CommandToken<T>(Reader.GetSchemaTable());

            if (!CallBackDataSetters.TryGetValue(token, out Func<SqlDataReader, T> SetCallBackData))
                CallBackDataSetters.Add(token, SetCallBackData = token.GenerateCallBackDataSetter());

            while (Reader.ReadAsync(CancellationToken).Result)
            {
                T item = SetCallBackData(Reader);

                items[index++] = item;

                if (index == size)
                {
                    size *= 2;
                    T[] new_items = new T[size];
                    Array.Copy(items, 0, new_items, 0, items.Length);
                    items = new_items;
                }
            }

            T[] result_items = new T[index];
            Array.Copy(items, 0, result_items, 0, index);
            items = result_items;

            return items;
        }
        internal static T FirstAsync(SqlDataReader Reader, CancellationToken CancellationToken)
        {
            CommandToken<T> token = new CommandToken<T>(Reader.GetSchemaTable());

            if (!CallBackDataSetters.TryGetValue(token, out Func<SqlDataReader, T> SetCallBackData))
                CallBackDataSetters.Add(token, SetCallBackData = token.GenerateCallBackDataSetter());

            Reader.ReadAsync(CancellationToken).Wait();

            return SetCallBackData(Reader);
        }



        public override string ToString()
        {
            return $"{typeof(T).Name} Database Reader Select";
        }
    }
}
