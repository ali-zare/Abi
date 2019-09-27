using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Abi.Data.Sql
{
    public sealed class DatabaseSelect
    {
        internal DatabaseSelect(EntityDatabaseConnection Connection, string CommandText)
        {
            connection = Connection;
            commandText = CommandText;
        }

        private string commandText;
        private EntityDatabaseConnection connection;


        public T[] ToArray<T>()
        {
            T[] callback;

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                {
                    callback = DatabaseReaderSelect<T>.ToArray(reader);

                    reader.Close();
                }
            }

            return callback;
        }
        public TCallBack[] ToArray<TCallBack>(Expression<Func<TCallBack>> CallBack)
        {
            return ToArray<TCallBack>();
        }

        public Task<T[]> ToArrayAsync<T>()
        {
            return ToArrayAsync<T>(CancellationToken.None);
        }
        public Task<TCallBack[]> ToArrayAsync<TCallBack>(Expression<Func<TCallBack>> CallBack)
        {
            return ToArrayAsync<TCallBack>(CancellationToken.None);
        }

        public Task<T[]> ToArrayAsync<T>(CancellationToken CancellationToken)
        {
            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                return command.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken)
                              .ContinueWith(task =>
                                {
                                    T[] callback;

                                    using (SqlDataReader reader = task.Result)
                                    {
                                        callback = DatabaseReaderSelect<T>.ToArrayAsync(reader, CancellationToken);

                                        reader.Close();
                                    }

                                    return callback;
                                });
            }
        }
        public Task<TCallBack[]> ToArrayAsync<TCallBack>(Expression<Func<TCallBack>> CallBack, CancellationToken CancellationToken)
        {
            return ToArrayAsync<TCallBack>(CancellationToken);
        }




        public T First<T>()
        {
            bool read = false;
            T callback = default;

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.HasRows)
                        read = true;

                    if (read)
                        callback = DatabaseReaderSelect<T>.First(reader);

                    reader.Close();
                }
            }

            if (!read)
                throw new EntityDatabaseException($"Database Select First {typeof(T).Name} Failed, Nothing Returned With Query : \n\n {commandText}");

            return callback;
        }
        public TCallBack First<TCallBack>(Expression<Func<TCallBack>> CallBack)
        {
            return First<TCallBack>();
        }

        public Task<T> FirstAsync<T>()
        {
            return FirstAsync<T>(CancellationToken.None);
        }
        public Task<TCallBack> FirstAsync<TCallBack>(Expression<Func<TCallBack>> CallBack)
        {
            return FirstAsync<TCallBack>(CancellationToken.None);
        }

        public Task<T> FirstAsync<T>(CancellationToken CancellationToken)
        {
            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                return command.ExecuteReaderAsync(CommandBehavior.SingleRow, CancellationToken)
                              .ContinueWith(task =>
                                {
                                    bool read = false;
                                    T callback = default;

                                    using (SqlDataReader reader = task.Result)
                                    {
                                        if (reader.HasRows)
                                            read = true;

                                        if (read)
                                            callback = DatabaseReaderSelect<T>.FirstAsync(reader, CancellationToken);

                                        reader.Close();

                                        if (!read)
                                            throw new EntityDatabaseException($"Database Select FirstAsync {typeof(T).Name} Failed, Nothing Returned With Query : \n\n {commandText}");

                                        return callback;
                                    }
                                });
            }
        }
        public Task<TCallBack> FirstAsync<TCallBack>(Expression<Func<TCallBack>> CallBack, CancellationToken CancellationToken)
        {
            return FirstAsync<TCallBack>(CancellationToken);
        }




        public T FirstOrDefault<T>()
        {
            T callback = default;

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (reader.HasRows)
                        callback = DatabaseReaderSelect<T>.First(reader);

                    reader.Close();
                }
            }

            return callback;
        }
        public TCallBack FirstOrDefault<TCallBack>(Expression<Func<TCallBack>> CallBack)
        {
            return FirstOrDefault<TCallBack>();
        }

        public Task<T> FirstOrDefaultAsync<T>()
        {
            return FirstOrDefaultAsync<T>(CancellationToken.None);
        }
        public Task<TCallBack> FirstOrDefaultAsync<TCallBack>(Expression<Func<TCallBack>> CallBack)
        {
            return FirstOrDefaultAsync<TCallBack>(CancellationToken.None);
        }

        public Task<T> FirstOrDefaultAsync<T>(CancellationToken CancellationToken)
        {

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                return command.ExecuteReaderAsync(CommandBehavior.SingleRow, CancellationToken)
                              .ContinueWith(task =>
                                {
                                    T callback = default;

                                    using (SqlDataReader reader = task.Result)
                                    {
                                        if (reader.HasRows)
                                            callback = DatabaseReaderSelect<T>.FirstAsync(reader, CancellationToken);

                                        reader.Close();
                                    }

                                    return callback;
                                });
            }

        }
        public Task<TCallBack> FirstOrDefaultAsync<TCallBack>(Expression<Func<TCallBack>> CallBack, CancellationToken CancellationToken)
        {
            return FirstOrDefaultAsync<TCallBack>(CancellationToken);
        }




        public T Single<T>()
        {
            bool read = false;
            T result = default;

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                object o = command.ExecuteScalar();

                if (o != null)
                    read = true;

                if (read)
                    result = Cast<T>.Scalar(o);
            }

            if (!read)
                throw new EntityDatabaseException($"Database Select Single {typeof(T).Name} Failed, Nothing Returned With Query : \n\n {commandText}");

            return result;
        }
        public T SingleOrDefault<T>()
        {
            T result = default;

            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                object o = command.ExecuteScalar();

                result = Cast<T>.Scalar(o);
            }

            return result;
        }

        public Task<T> SingleAsync<T>()
        {
            return SingleAsync<T>(CancellationToken.None);
        }
        public Task<T> SingleOrDefaultAsync<T>()
        {
            return SingleOrDefaultAsync<T>(CancellationToken.None);
        }

        public Task<T> SingleAsync<T>(CancellationToken CancellationToken)
        {
            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                return command.ExecuteScalarAsync(CancellationToken)
                              .ContinueWith(task =>
                                {
                                    bool read = false;
                                    T result = default;

                                    object o = task.Result;

                                    if (o != null)
                                        read = true;

                                    if (read)
                                        result = Cast<T>.Scalar(o);

                                    if (!read)
                                        throw new EntityDatabaseException($"Database Select Single {typeof(T).Name} Failed, Nothing Returned With Query : \n\n {commandText}");

                                    return result;
                                });
            }
        }
        public Task<T> SingleOrDefaultAsync<T>(CancellationToken CancellationToken)
        {
            using (SqlCommand command = connection.GetCommand(commandText))
            {
                DatabaseTrace.Append(command.CommandText, Constant.Query);

                if (connection.HasTransaction)
                    command.Transaction = connection.Transaction;

                return command.ExecuteScalarAsync(CancellationToken)
                              .ContinueWith(task =>
                                {
                                    T result = default;

                                    object o = task.Result;

                                    result = Cast<T>.Scalar(o);

                                    return result;
                                });
            }
        }
    }
}
