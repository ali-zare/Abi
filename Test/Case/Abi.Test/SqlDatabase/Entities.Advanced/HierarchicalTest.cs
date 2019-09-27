using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Enterprise.Model;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.Entities.Advanced
{
    [TestClass]
    public class HierarchicalTest
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void Save_WithCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.Execute((context, connection).WithCallBack().Save);
            }

            Hierarchical(Save);
        }
        [TestMethod]
        public void Save_WithoutCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.Execute((context, connection).WithoutCallBack().Save);
            }

            Hierarchical(Save);
        }
        [TestMethod]
        public void Save_WithCheckConcurrency()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.Execute((context, connection).WithCheckConcurrency().Save);
            }

            Hierarchical(Save);
        }


        [TestMethod]
        public void SaveBatch_WithCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.ExecuteBatch((context, connection).WithCallBack().Save);
            }

            Hierarchical(Save);
        }
        [TestMethod]
        public void SaveBatch_WithoutCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.ExecuteBatch((context, connection).WithoutCallBack().Save);
            }

            Hierarchical(Save);
        }
        [TestMethod]
        public void SaveBatch_WithCheckConcurrency()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.ExecuteBatch((context, connection).WithCheckConcurrency().Save);
            }

            Hierarchical(Save);
        }




        private void Hierarchical(Action<EnterpriseContext> save)
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Customer customer5 = new Customer();
            customer5.FirstName = "customer5";
            context.Customers.Add(customer5);

            Customer customer6 = new Customer();
            customer6.FirstName = "customer6";
            context.Customers.Add(customer6);

            Customer customer3 = new Customer();
            customer3.FirstName = "customer3";
            context.Customers.Add(customer3);

            Customer customer1 = new Customer();
            customer1.FirstName = "customer1";
            context.Customers.Add(customer1);

            Customer customer2 = new Customer();
            customer2.FirstName = "customer2";
            context.Customers.Add(customer2);

            Customer customer4 = new Customer();
            customer4.FirstName = "customer4";
            context.Customers.Add(customer4);

            order.Customer = customer1;

            customer1.FirstPartner = customer2;
            customer1.SecondPartner = customer5;

            customer2.FirstPartner = customer3;
            customer2.SecondPartner = customer6;

            customer3.FirstPartner = customer4;

            customer5.FirstPartner = customer6;

            #region check

            order.CheckAdded();

            customer1.CheckAdded();
            customer2.CheckAdded();
            customer3.CheckAdded();
            customer4.CheckAdded();
            customer5.CheckAdded();
            customer6.CheckAdded();

            context.Changes.CheckCount(7)
                           .CheckFound(order)
                           .CheckFound(customer1)
                           .CheckFound(customer2)
                           .CheckFound(customer3)
                           .CheckFound(customer4)
                           .CheckFound(customer5)
                           .CheckFound(customer6);

            #endregion check

            // save changes
            context.Execute(save);

            #region check

            context.Changes.CheckCount(0);

            order.CheckUnchanged();

            customer1.CheckUnchanged();
            customer2.CheckUnchanged();
            customer3.CheckUnchanged();
            customer4.CheckUnchanged();
            customer5.CheckUnchanged();
            customer6.CheckUnchanged();

            order.Customer.Check(customer1);
            order.CustomerID.Check(customer1.ID);

            customer1.FirstPartner.Check(customer2);
            customer1.FirstPartnerID.Check(customer2.ID);
            customer1.SecondPartner.Check(customer5);
            customer1.SecondPartnerID.Check(customer5.ID);

            customer2.FirstPartner.Check(customer3);
            customer2.FirstPartnerID.Check(customer3.ID);
            customer2.SecondPartner.Check(customer6);
            customer2.SecondPartnerID.Check(customer6.ID);

            customer3.FirstPartner.Check(customer4);
            customer3.FirstPartnerID.Check(customer4.ID);

            customer5.FirstPartner.Check(customer6);
            customer5.FirstPartnerID.Check(customer6.ID);

            #endregion check




            // Modify order
            order.Customer = null;

            order.CheckModified();
            order.Customer.Check(null);
            order.CustomerID.Check(null);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            // Delete customer1
            context.Customers.Remove(customer1);

            customer1.CheckDeleted();

            // Add customer7
            Customer customer7 = new Customer();
            customer7.FirstName = "customer7";
            context.Customers.Add(customer7);

            customer7.CheckAdded();

            // Modify order
            order.Customer = customer7;

            #region check

            order.CheckModified();
            order.Customer.Check(customer7);
            order.CustomerID.Check(customer7.ID);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Changes.CheckCount(3)
                           .CheckFound(order)
                           .CheckFound(customer1)
                           .CheckFound(customer7);

            // Modify customer2
            customer2.FirstPartner = customer4;

            customer2.CheckModified();
            customer2.GetChangedProperties().CheckCount(1).CheckFound<Customer>(o => o.FirstPartnerID);

            // Modify customer5
            customer5.FirstPartner = customer3;

            customer5.CheckModified();
            customer5.GetChangedProperties().CheckCount(1).CheckFound<Customer>(o => o.FirstPartnerID);

            // Modify customer4
            customer4.FirstPartner = customer5;

            customer4.CheckModified();
            customer4.GetChangedProperties().CheckCount(1).CheckFound<Customer>(o => o.FirstPartnerID);

            // Modify customer5
            customer5.SecondPartner = customer2;

            customer5.CheckModified();
            customer5.GetChangedProperties().CheckCount(2).CheckFound<Customer>(o => o.FirstPartnerID)
                                                          .CheckFound<Customer>(o => o.SecondPartnerID);

            #endregion check

            // save changes
            context.Execute(save);

            #region check

            context.Changes.CheckCount(0);

            order.CheckUnchanged();

            customer1.CheckDetached();
            customer2.CheckUnchanged();
            customer4.CheckUnchanged();
            customer5.CheckUnchanged();

            order.Customer.Check(customer7);
            order.CustomerID.Check(customer7.ID);

            customer2.FirstPartner.Check(customer4);
            customer2.FirstPartnerID.Check(customer4.ID);

            customer4.FirstPartner.Check(customer5);
            customer4.FirstPartnerID.Check(customer5.ID);

            customer5.FirstPartner.Check(customer3);
            customer5.FirstPartnerID.Check(customer3.ID);

            customer5.SecondPartner.Check(customer2);
            customer5.SecondPartnerID.Check(customer2.ID);

            #endregion check
        }
    }
}
