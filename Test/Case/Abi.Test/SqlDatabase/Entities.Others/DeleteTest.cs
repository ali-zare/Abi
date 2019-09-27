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
    public class DeleteTest
    {
        [TestMethod]
        public void Delete_WithoutCallBack()
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
                                                        .Delete().WithoutCheckConcurrency()
                                                           .Write(customer2)
                                                           .Write(customer1);

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

            ConcurrencyOnUpdate(Save, false, false);
            ConcurrencyOnDelete(Save, true, false);
        }
        [TestMethod]
        public void Delete_WithoutCallBack_MultiRow()
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
                                                        .Delete().WithoutCheckConcurrency()
                                                           .Write(customer2, customer1);

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

            ConcurrencyOnUpdate(Save, false, false);
            ConcurrencyOnDelete(Save, true, false);
        }

        [TestMethod]
        public void Delete_WithCheckConcurrency()
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
                                                        .Delete().WithCheckConcurrency(e => new { e.RV })
                                                           .Write(customer2)
                                                           .Write(customer1);

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

            ConcurrencyOnUpdate(Save, true, false);
            ConcurrencyOnDelete(Save, true, false);
        }
        [TestMethod]
        public void Delete_WithCheckConcurrency_MultiRow()
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
                                                        .Delete().WithCheckConcurrency(e => new { e.RV })
                                                           .Write(customer2, customer1);

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

            ConcurrencyOnUpdate(Save, true, true);
            ConcurrencyOnDelete(Save, true, false);
        }




        private void ConcurrencyOnUpdate(Action<EnterpriseContext, Customer[]> save, bool hasConcurrency, bool hasConflict)
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

            customer2.FirstPartner = customer1;

            #region check

            customer1.CheckAdded();
            customer2.CheckAdded();

            context.Changes.CheckCount(2)
                           .CheckFound(customer1)
                           .CheckFound(customer2);

            #endregion check

            #region insert customer1 and customer2

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>().Insert().WithCallBack(e => new { e.RV })
                                                 .Write(customer1)
                                                 .Write(customer2);
                connection.Close();
            }

            #endregion insert customer1 and customer2

            #region check

            context.Changes.CheckCount(0);

            customer1.CheckUnchanged();
            customer2.CheckUnchanged();

            customer1.FirstPartners.CheckCount(1).CheckItem(0, customer2);

            customer2.FirstPartner.Check(customer1);
            customer2.FirstPartnerID.Check(customer1.ID);

            customer1.GetChangedProperties().CheckCount(0);
            customer2.GetChangedProperties().CheckCount(0);

            #endregion check

            // Delete customer2
            context.Customers.Remove(customer2);

            // Delete customer1
            context.Customers.Remove(customer1);

            #region check

            customer1.CheckDeleted();
            customer2.CheckDeleted();

            context.Changes.CheckCount(2)
                           .CheckFound(customer1)
                           .CheckFound(customer2);

            #endregion check

            #region update customer2

            void update()
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.Open();

                Database.Connect(connection)
                        .Prepared()
                        .WithParameters(new { CustomerID = customer2.ID, FName = customer2.FirstName })
                        .Query(@"set nocount on;

                                 update Customers 
                                   set FName = @FName 
                                 where ID = @CustomerID

                                ")
                        .Execute();

                connection.Close();
            }

            update();

            #endregion update customer2

            try
            {
                customer1.TakeSnapshot();
                customer2.TakeSnapshot();

                context.Execute(save, customer1, customer2);

                customer1.RemoveSnapshot();
                customer2.RemoveSnapshot();
            }
            catch (EntityDatabaseConcurrencyException) when (hasConcurrency && !hasConflict)
            {
                customer1.RestoreSnapshot();
                customer2.RestoreSnapshot();
            }
            catch (EntityDatabaseConcurrencyException e) when (hasConcurrency && hasConflict && ((SqlException)e.InnerException).Number == 547) // The UPDATE statement conflicted with the FOREIGN KEY ...
            {
                customer1.RestoreSnapshot();
                customer2.RestoreSnapshot();
            }

            #region check

            if (hasConcurrency)
            {
                customer1.CheckDeleted();
                customer2.CheckDeleted();

                context.Changes.CheckCount(2)
                               .CheckFound(customer1)
                               .CheckFound(customer2);
            }
            else
            {
                context.Changes.CheckCount(0);

                customer1.CheckDetached();
                customer2.CheckDetached();
            }

            #endregion check

        }
        private void ConcurrencyOnDelete(Action<EnterpriseContext, Customer[]> save, bool hasConcurrency, bool hasConflict)
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

            customer2.FirstPartner = customer1;

            #region check

            customer1.CheckAdded();
            customer2.CheckAdded();

            context.Changes.CheckCount(2)
                           .CheckFound(customer1)
                           .CheckFound(customer2);

            #endregion check

            #region insert customer1 and customer2

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>().Insert().WithCallBack(e => new { e.RV })
                                                 .Write(customer1)
                                                 .Write(customer2);
                connection.Close();
            }

            #endregion insert customer1 and customer2

            #region check

            context.Changes.CheckCount(0);

            customer1.CheckUnchanged();
            customer2.CheckUnchanged();

            customer1.FirstPartners.CheckCount(1).CheckItem(0, customer2);

            customer2.FirstPartner.Check(customer1);
            customer2.FirstPartnerID.Check(customer1.ID);

            customer1.GetChangedProperties().CheckCount(0);
            customer2.GetChangedProperties().CheckCount(0);

            #endregion check

            // Delete customer2
            context.Customers.Remove(customer2);

            // Delete customer1
            context.Customers.Remove(customer1);

            #region check

            customer1.CheckDeleted();
            customer2.CheckDeleted();

            context.Changes.CheckCount(2)
                           .CheckFound(customer1)
                           .CheckFound(customer2);

            #endregion check

            #region delete customer2

            void update()
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.Open();

                Database.Connect(connection)
                        .Prepared()
                        .WithParameters(new { CustomerID = customer2.ID })
                        .Query(@"set nocount on;

                                 delete from Customers 
                                 where ID = @CustomerID

                                ")
                        .Execute();

                connection.Close();
            }

            update();

            #endregion delete customer2

            try
            {
                customer1.TakeSnapshot();
                customer2.TakeSnapshot();

                context.Execute(save, customer1, customer2);

                customer1.RemoveSnapshot();
                customer2.RemoveSnapshot();
            }
            catch (EntityDatabaseConcurrencyException) when (hasConcurrency && !hasConflict)
            {
                customer1.RestoreSnapshot();
                customer2.RestoreSnapshot();
            }
            catch (EntityDatabaseConcurrencyException e) when (hasConcurrency && hasConflict && ((SqlException)e.InnerException).Number == 547) // The UPDATE statement conflicted with the FOREIGN KEY ...
            {
                customer1.RestoreSnapshot();
                customer2.RestoreSnapshot();
            }

            #region check

            if (hasConcurrency)
            {
                customer1.CheckDeleted();
                customer2.CheckDeleted();

                context.Changes.CheckCount(2)
                               .CheckFound(customer1)
                               .CheckFound(customer2);
            }
            else
            {
                context.Changes.CheckCount(0);

                customer1.CheckDetached();
                customer2.CheckDetached();
            }

            #endregion check

        }

    }
}
