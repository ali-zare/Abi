using System;
using System.Data;
using System.Data.SqlClient;
using Abi.Data;
using Abi.Data.Sql;
using Enterprise.Model;

namespace Abi.Test
{
    internal static class EnterpriseDatabase
    {
        static EnterpriseDatabase()
        {
            con_str = new SqlConnectionStringBuilder { DataSource = "(local)", InitialCatalog = "Abi.Enterprise.Database", IntegratedSecurity = true }.ConnectionString;
            con_str_mars = new SqlConnectionStringBuilder { DataSource = "(local)", InitialCatalog = "Abi.Enterprise.Database", IntegratedSecurity = true, MultipleActiveResultSets = true }.ConnectionString;
        }

        private static string con_str;
        private static string con_str_mars;



        internal static IEntityDatabaseContext<EnterpriseContext> WithCallBack(this (EnterpriseContext context, EntityDatabaseConnection connection) execution)
        {
            return EntityDatabase<EnterpriseContext>.Connect(execution.connection)
                                                    .Context(execution.context)
                                                    .Table<Customer>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency()
                                                    .Table<Order>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency()
                                                    .Table<OrderDetail>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency()
                                                    .Table<Product>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency()
                                                    .Table<Shipment>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency()
                                                    .Table<Address>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency();
        }
        internal static IEntityDatabaseContext<EnterpriseContext> WithoutCallBack(this (EnterpriseContext context, EntityDatabaseConnection connection) execution)
        {
            return EntityDatabase<EnterpriseContext>.Connect(execution.connection)
                                                    .Context(execution.context);
        }
        internal static IEntityDatabaseContext<EnterpriseContext> WithCheckConcurrency(this (EnterpriseContext context, EntityDatabaseConnection connection) execution)
        {
            return EntityDatabase<EnterpriseContext>.Connect(execution.connection)
                                                    .Context(execution.context)
                                                    .Table<Customer>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                       .Delete().WithCheckConcurrency(e => new { e.RV })
                                                    .Table<Order>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                       .Delete().WithCheckConcurrency(e => new { e.RV })
                                                    .Table<OrderDetail>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                       .Delete().WithCheckConcurrency(e => new { e.RV })
                                                    .Table<Product>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                       .Delete().WithCheckConcurrency(e => new { e.RV })
                                                    .Table<Shipment>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                       .Delete().WithCheckConcurrency(e => new { e.RV })
                                                    .Table<Address>()
                                                       .Insert().WithCallBack(e => new { e.RV })
                                                       .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                       .Delete().WithCheckConcurrency(e => new { e.RV });
        }



        internal static EntityDatabaseConnection GetConnection(bool mars = false)
        {
            if (mars)
                return new EntityDatabaseConnection(con_str_mars);
            else
                return new EntityDatabaseConnection(con_str);
        }
        internal static void Execute(this EnterpriseContext context, Action<EnterpriseContext> save)
        {
            save(context);
        }
        internal static void Execute<TEntity>(this EnterpriseContext context, Action<EnterpriseContext, TEntity[]> save, params TEntity[] entities) where TEntity : Entity<TEntity>
        {
            save(context, entities);
        }

        internal static void Execute(this EntityDatabaseConnection connection, Action<bool, bool> save)
        {
            connection.Open();
            connection.BeginTransaction(IsolationLevel.Snapshot);

            try
            {
                save(false, true);

                connection.CommitTransaction();
                connection.Close();
            }
            catch (Exception e)
            {
                connection.RollbackTransaction();
                connection.Close();

                throw e;
            }
        }
        internal static void ExecuteBatch(this EntityDatabaseConnection connection, Action<bool, bool> save)
        {
            connection.Open();
            connection.BeginTransaction(IsolationLevel.Snapshot);

            try
            {
                save(true, true);

                connection.CommitTransaction();
                connection.Close();
            }
            catch (Exception e)
            {
                connection.RollbackTransaction();
                connection.Close();

                throw e;
            }
        }
    }
}
