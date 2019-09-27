using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Data;
using System;
using Enterprise.Model;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.Entities.Others
{
    [TestClass]
    public class UpdateTest
    {
        [TestMethod]
        public void Update_WithCallBack()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                Customer customer1 = entities[0];
                Customer customer2 = entities[1];

                try
                {
                    EntityDatabase<EnterpriseContext>.Connect(connection)
                                                     .Table<Customer>()
                                                        .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency()
                                                           .Write(customer1)
                                                           .Write(customer2);

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

            Concurrency(Save);
        }
        [TestMethod]
        public void Update_WithCallBack_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                Customer customer1 = entities[0];
                Customer customer2 = entities[1];

                try
                {
                    EntityDatabase<EnterpriseContext>.Connect(connection)
                                                     .Table<Customer>()
                                                        .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency()
                                                           .Write(customer1, customer2);

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

            Concurrency(Save);
        }

        [TestMethod]
        public void Update_WithoutCallBack()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                Customer customer1 = entities[0];
                Customer customer2 = entities[1];

                try
                {
                    EntityDatabase<EnterpriseContext>.Connect(connection)
                                                     .Table<Customer>()
                                                        .Update().WithoutCallBack()
                                                           .Write(customer1)
                                                           .Write(customer2);

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

            Concurrency(Save);
        }
        [TestMethod]
        public void Update_WithoutCallBack_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                Customer customer1 = entities[0];
                Customer customer2 = entities[1];

                try
                {
                    EntityDatabase<EnterpriseContext>.Connect(connection)
                                                     .Table<Customer>()
                                                        .Update().WithoutCallBack()
                                                           .Write(customer1, customer2);

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

            Concurrency(Save);
        }

        [TestMethod]
        public void Update_WithCheckConcurrency()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                Customer customer1 = entities[0];
                Customer customer2 = entities[1];

                try
                {
                    EntityDatabase<EnterpriseContext>.Connect(connection)
                                                     .Table<Customer>()
                                                        .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                           .Write(customer1)
                                                           .Write(customer2);

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

            Concurrency(Save);
        }
        [TestMethod]
        public void Update_WithCheckConcurrency_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                Customer customer1 = entities[0];
                Customer customer2 = entities[1];

                try
                {
                    EntityDatabase<EnterpriseContext>.Connect(connection)
                                                     .Table<Customer>()
                                                        .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                           .Write(customer1, customer2);

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

            Concurrency(Save);
        }




        private void Concurrency(Action<EnterpriseContext, Customer[]> save)
        {
            EnterpriseContext context = new EnterpriseContext();

            // Add customer1
            Customer customer1 = new Customer();
            customer1.FirstName = "customer1";
            context.Customers.Add(customer1);

            // Add customer2
            Customer customer2 = new Customer();
            customer2.FirstName = "customer2";
            context.Customers.Add(customer2);

            // Add customer3
            Customer customer3 = new Customer();
            customer3.FirstName = "customer3";
            context.Customers.Add(customer3);

            customer2.FirstPartner = customer1;

            #region check

            customer1.CheckAdded();
            customer2.CheckAdded();
            customer3.CheckAdded();

            context.Changes.CheckCount(3)
                           .CheckFound(customer1)
                           .CheckFound(customer2)
                           .CheckFound(customer3);

            #endregion check

            #region insert customer1, customer2 and customer3

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>().Insert().WithCallBack(e => new { e.RV })
                                                 .Write(customer1)
                                                 .Write(customer2)
                                                 .Write(customer3);
                connection.Close();
            }

            #endregion insert customer1, customer2 and customer3

            #region check

            context.Changes.CheckCount(0);

            customer1.CheckUnchanged();
            customer2.CheckUnchanged();
            customer3.CheckUnchanged();

            customer1.FirstPartners.CheckCount(1).CheckItem(0, customer2);

            customer2.FirstPartner.Check(customer1);
            customer2.FirstPartnerID.Check(customer1.ID);

            customer1.GetChangedProperties().CheckCount(0);
            customer2.GetChangedProperties().CheckCount(0);
            customer3.GetChangedProperties().CheckCount(0);

            #endregion check

            // Modify customer1
            customer1.FirstPartner = customer3;

            // Modify customer2
            customer2.FirstPartner = customer3;

            #region check

            customer1.CheckModified();
            customer1.FirstPartner.Check(customer3);
            customer1.FirstPartnerID.Check(customer3.ID);
            customer1.GetChangedProperties().CheckCount(1).CheckFound<Customer>(o => o.FirstPartnerID);

            customer2.CheckModified();
            customer2.FirstPartner.Check(customer3);
            customer2.FirstPartnerID.Check(customer3.ID);
            customer2.GetChangedProperties().CheckCount(1).CheckFound<Customer>(o => o.FirstPartnerID);

            context.Changes.CheckCount(2)
                           .CheckFound(customer1)
                           .CheckFound(customer2);

            #endregion check

            #region delete customer3

            void delete()
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.Open();

                Database.Connect(connection)
                        .Prepared()
                        .WithParameters(new { CustomerID = customer3.ID })
                        .Query(@"set nocount on;

                                 delete 
                                 from Customers
                                 where ID = @CustomerID

                                ")
                        .Execute();

                connection.Close();
            }

            delete();

            #endregion delete customer3

            try
            {
                customer1.TakeSnapshot();
                customer2.TakeSnapshot();

                context.Execute(save, customer1, customer2);

                customer1.RemoveSnapshot();
                customer2.RemoveSnapshot();
            }
            catch (SqlException e) when (e.Number == 547) // The UPDATE statement conflicted with the FOREIGN KEY ...
            {
                customer1.RestoreSnapshot();
                customer2.RestoreSnapshot();
            }

            #region check

            customer1.CheckModified();
            customer1.FirstPartner.Check(customer3);
            customer1.FirstPartnerID.Check(customer3.ID);
            customer1.GetChangedProperties().CheckCount(1).CheckFound<Customer>(o => o.FirstPartnerID);

            customer2.CheckModified();
            customer2.FirstPartner.Check(customer3);
            customer2.FirstPartnerID.Check(customer3.ID);
            customer2.GetChangedProperties().CheckCount(1).CheckFound<Customer>(o => o.FirstPartnerID);

            context.Changes.CheckCount(2)
                           .CheckFound(customer1)
                           .CheckFound(customer2);

            #endregion check

        }
    }
}
