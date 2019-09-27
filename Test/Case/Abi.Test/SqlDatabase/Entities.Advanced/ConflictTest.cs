using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System;
using Enterprise.Model;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.Entities.Advanced
{
    [TestClass]
    public class ConflictTest
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

            ConflictOnUpdate(Save);
            ConflictOnDelete(Save);
        }
        [TestMethod]
        public void Save_WithoutCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.Execute((context, connection).WithoutCallBack().Save);
            }

            ConflictOnUpdate(Save);
            ConflictOnDelete(Save);
        }
        [TestMethod]
        public void Save_WithCheckConcurrency()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.Execute((context, connection).WithCheckConcurrency().Save);
            }

            ConflictOnUpdate(Save);
            ConflictOnDelete(Save);
        }


        [TestMethod]
        public void SaveBatch_WithCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.ExecuteBatch((context, connection).WithCallBack().Save);
            }

            ConflictOnUpdate(Save);
            ConflictOnDelete(Save);
        }
        [TestMethod]
        public void SaveBatch_WithoutCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.ExecuteBatch((context, connection).WithoutCallBack().Save);
            }

            ConflictOnUpdate(Save);
            ConflictOnDelete(Save);
        }
        [TestMethod]
        public void SaveBatch_WithCheckConcurrency()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                connection.ExecuteBatch((context, connection).WithCheckConcurrency().Save);
            }

            ConflictOnUpdate(Save);
            ConflictOnDelete(Save);
        }




        private void ConflictOnUpdate(Action<EnterpriseContext> save)
        {
            EnterpriseContext context = new EnterpriseContext();

            // Add order1
            Order order1 = new Order();
            context.Orders.Add(order1);

            // Add orderDetail1
            OrderDetail orderDetail1 = new OrderDetail();
            orderDetail1.Order = order1;
            context.OrderDetails.Add(orderDetail1);

            // Add customer1
            Customer customer1 = new Customer();
            customer1.FirstName = "customer1";
            context.Customers.Add(customer1);

            customer1.Orders.Add(order1);

            #region check

            order1.CheckAdded();
            customer1.CheckAdded();
            orderDetail1.CheckAdded();

            context.Changes.CheckCount(3)
                           .CheckFound(order1)
                           .CheckFound(customer1)
                           .CheckFound(orderDetail1);

            #endregion check

            // save changes
            context.Execute(save);

            #region check

            context.Changes.CheckCount(0);

            order1.CheckUnchanged();
            customer1.CheckUnchanged();
            orderDetail1.CheckUnchanged();

            customer1.Orders.CheckCount(1).CheckItem(0, order1);

            order1.Customer.Check(customer1);
            order1.CustomerID.Check(customer1.ID);
            order1.OrderDetails.CheckCount(1).CheckItem(0, orderDetail1);

            orderDetail1.Order.Check(order1);
            orderDetail1.OrderID.Check(order1.ID);

            order1.GetChangedProperties().CheckCount(0);
            customer1.GetChangedProperties().CheckCount(0);
            orderDetail1.GetChangedProperties().CheckCount(0);

            #endregion check

            // Add shipment1
            Shipment shipment1 = new Shipment();
            context.Shipments.Add(shipment1);

            // Add order2
            Order order2 = new Order();
            context.Orders.Add(order2);

            // Modify orderDetail1
            orderDetail1.Order = order2;

            // Add orderDetail2
            OrderDetail orderDetail2 = new OrderDetail();
            orderDetail2.Order = order2;
            context.OrderDetails.Add(orderDetail2);

            // Modify order1
            order1.Customer = null;

            // Add customer2
            Customer customer2 = new Customer();
            customer2.FirstName = "customer2";
            context.Customers.Add(customer2);

            // Modify customer1
            customer1.SecondPartner = customer2;

            // Modify order1
            order1.Customer = customer2;

            shipment1.Order = order2;

            // Add customer3
            Customer customer3 = new Customer();
            customer3.FirstName = "customer3";
            context.Customers.Add(customer3);

            customer2.FirstPartner = customer3;

            // Delete customer1
            context.Customers.Remove(customer1);

            order2.Customer = customer2;

            // Modify order1
            order1.Customer = customer3;

            #region check

            order2.CheckAdded();
            shipment1.CheckAdded();
            customer2.CheckAdded();
            customer3.CheckAdded();
            orderDetail2.CheckAdded();

            customer1.CheckDeleted();

            order1.CheckModified();
            order1.Customer.Check(customer3);
            order1.CustomerID.Check(customer3.ID);
            order1.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            orderDetail1.CheckModified();
            orderDetail1.Order.Check(order2);
            orderDetail1.OrderID.Check(order2.ID);
            orderDetail1.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Changes.CheckCount(8)
                           .CheckFound(order1)
                           .CheckFound(order2)
                           .CheckFound(shipment1)
                           .CheckFound(customer1)
                           .CheckFound(customer2)
                           .CheckFound(customer3)
                           .CheckFound(orderDetail1)
                           .CheckFound(orderDetail2);

            #endregion check

            #region update order1

            async Task update()
            {
                EntityDatabaseConnection connection = GetConnection();

                await connection.OpenAsync();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                await Database.Connect(connection)
                              .Prepared()
                              .WithParameters(new { OrderID = order1.ID, ShippingDate = "1398/06/30".ToDateTime() })
                              .Query(@"set nocount on;

                                       update Orders
                                          set ShippingDate = @ShippingDate
                                       where ID = @OrderID;

                                       waitfor delay '00:00:00.5'

                                      ")
                              .ExecuteAsync();

                connection.CommitTransaction();
                connection.Close();
            }

            Task task = update();

            #endregion update order1 

            try
            {
                // save changes
                context.Execute(save);
            }
            catch (SqlException e) when (e.Number == 3960)
            {
                // it's ok, conflict was occurred
            }

            task.Wait();

            #region check

            order2.CheckAdded();
            shipment1.CheckAdded();
            customer2.CheckAdded();
            customer3.CheckAdded();
            orderDetail2.CheckAdded();

            customer1.CheckDeleted();

            order1.CheckModified();
            order1.Customer.Check(customer3);
            order1.CustomerID.Check(customer3.ID);
            order1.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            orderDetail1.CheckModified();
            orderDetail1.Order.Check(order2);
            orderDetail1.OrderID.Check(order2.ID);
            orderDetail1.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Changes.CheckCount(8)
                           .CheckFound(order1)
                           .CheckFound(order2)
                           .CheckFound(shipment1)
                           .CheckFound(customer1)
                           .CheckFound(customer2)
                           .CheckFound(customer3)
                           .CheckFound(orderDetail1)
                           .CheckFound(orderDetail2);

            #endregion check
        }
        private void ConflictOnDelete(Action<EnterpriseContext> save)
        {
            EnterpriseContext context = new EnterpriseContext();

            // Add order1
            Order order1 = new Order();
            context.Orders.Add(order1);

            // Add orderDetail1
            OrderDetail orderDetail1 = new OrderDetail();
            orderDetail1.Order = order1;
            context.OrderDetails.Add(orderDetail1);

            // Add customer1
            Customer customer1 = new Customer();
            customer1.FirstName = "customer1";
            context.Customers.Add(customer1);

            customer1.Orders.Add(order1);

            #region check

            order1.CheckAdded();
            customer1.CheckAdded();
            orderDetail1.CheckAdded();

            context.Changes.CheckCount(3)
                           .CheckFound(order1)
                           .CheckFound(customer1)
                           .CheckFound(orderDetail1);

            #endregion check

            // save changes
            context.Execute(save);

            #region check

            context.Changes.CheckCount(0);

            order1.CheckUnchanged();
            customer1.CheckUnchanged();
            orderDetail1.CheckUnchanged();

            customer1.Orders.CheckCount(1).CheckItem(0, order1);

            order1.Customer.Check(customer1);
            order1.CustomerID.Check(customer1.ID);
            order1.OrderDetails.CheckCount(1).CheckItem(0, orderDetail1);

            orderDetail1.Order.Check(order1);
            orderDetail1.OrderID.Check(order1.ID);

            order1.GetChangedProperties().CheckCount(0);
            customer1.GetChangedProperties().CheckCount(0);
            orderDetail1.GetChangedProperties().CheckCount(0);

            #endregion check

            // Add shipment1
            Shipment shipment1 = new Shipment();
            context.Shipments.Add(shipment1);

            // Add order2
            Order order2 = new Order();
            context.Orders.Add(order2);

            // Modify orderDetail1
            orderDetail1.Order = order2;

            // Add orderDetail2
            OrderDetail orderDetail2 = new OrderDetail();
            orderDetail2.Order = order2;
            context.OrderDetails.Add(orderDetail2);

            // Modify order1
            order1.Customer = null;

            // Add customer2
            Customer customer2 = new Customer();
            customer2.FirstName = "customer2";
            context.Customers.Add(customer2);

            // Modify customer1
            customer1.SecondPartner = customer2;

            // Modify order1
            order1.Customer = customer2;

            shipment1.Order = order2;

            // Add customer3
            Customer customer3 = new Customer();
            customer3.FirstName = "customer3";
            context.Customers.Add(customer3);

            customer2.FirstPartner = customer3;

            // Delete customer1
            context.Customers.Remove(customer1);

            order2.Customer = customer2;

            // Modify order1
            order1.Customer = customer3;

            #region check

            order2.CheckAdded();
            shipment1.CheckAdded();
            customer2.CheckAdded();
            customer3.CheckAdded();
            orderDetail2.CheckAdded();

            customer1.CheckDeleted();

            order1.CheckModified();
            order1.Customer.Check(customer3);
            order1.CustomerID.Check(customer3.ID);
            order1.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            orderDetail1.CheckModified();
            orderDetail1.Order.Check(order2);
            orderDetail1.OrderID.Check(order2.ID);
            orderDetail1.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Changes.CheckCount(8)
                           .CheckFound(order1)
                           .CheckFound(order2)
                           .CheckFound(shipment1)
                           .CheckFound(customer1)
                           .CheckFound(customer2)
                           .CheckFound(customer3)
                           .CheckFound(orderDetail1)
                           .CheckFound(orderDetail2);

            #endregion check

            #region delete order1

            async Task delete()
            {
                EntityDatabaseConnection connection = GetConnection();

                await connection.OpenAsync();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                await Database.Connect(connection)
                              .Prepared()
                              .WithParameters(new { OrderID = order1.ID })
                              .Query(@"set nocount on;

                                       delete Orders
                                       where ID = @OrderID;

                                       waitfor delay '00:00:00.5'

                                      ")
                              .ExecuteAsync()
                              .ContinueWith(t =>
                              {
                                  if (!t.IsFaulted)
                                      throw new Exception("Expected Exception, Was Not Thrown");

                                  connection.RollbackTransaction();
                                  connection.Close();
                              });
            }

            Task task = delete();

            #endregion delete order1 

            // save changes
            context.Execute(save);

            task.Wait();

            #region check

            order1.CheckUnchanged();
            order2.CheckUnchanged();
            shipment1.CheckUnchanged();
            customer2.CheckUnchanged();
            customer3.CheckUnchanged();
            orderDetail1.CheckUnchanged();
            orderDetail2.CheckUnchanged();

            customer1.CheckDetached();

            order1.Customer.Check(customer3);
            order1.CustomerID.Check(customer3.ID);
            order1.GetChangedProperties().CheckCount(0);
            order1.OrderDetails.CheckCount(0);
            order1.Shipment.CheckItem(null);

            order2.Customer.Check(customer2);
            order2.CustomerID.Check(customer2.ID);
            order2.GetChangedProperties().CheckCount(0);
            order2.OrderDetails.CheckCount(2).CheckItem(0, orderDetail1).CheckItem(1, orderDetail2);
            order2.Shipment.CheckItem(shipment1);

            shipment1.Order.Check(order2);
            shipment1.OrderID.Check(order2.ID);
            shipment1.GetChangedProperties().CheckCount(0);

            customer2.FirstPartner.Check(customer3);
            customer2.FirstPartnerID.Check(customer3.ID);
            customer2.SecondPartner.Check(null);
            customer2.SecondPartnerID.Check(null);
            customer2.GetChangedProperties().CheckCount(0);
            customer2.Orders.CheckCount(1).CheckItem(0, order2);
            customer2.FirstPartners.CheckCount(0);
            customer2.SecondPartners.CheckCount(0);

            customer3.FirstPartner.Check(null);
            customer3.FirstPartnerID.Check(null);
            customer3.SecondPartner.Check(null);
            customer3.SecondPartnerID.Check(null);
            customer2.GetChangedProperties().CheckCount(0);
            customer3.Orders.CheckCount(1).CheckItem(0, order1);
            customer3.FirstPartners.CheckCount(1).CheckItem(0, customer2);
            customer3.SecondPartners.CheckCount(0);

            orderDetail1.Order.Check(order2);
            orderDetail1.OrderID.Check(order2.ID);
            orderDetail1.GetChangedProperties().CheckCount(0);

            orderDetail2.Order.Check(order2);
            orderDetail2.OrderID.Check(order2.ID);
            orderDetail2.GetChangedProperties().CheckCount(0);

            context.Changes.CheckCount(0);

            #endregion check
        }
    }
}
