using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System;
using Enterprise.Model;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.Entities
{
    [TestClass]
    public class ContextTest
    {
        public TestContext TestContext { get; set; }




        [TestMethod]
        public void Select_Prepared_Without_Parameter_With_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();


            using (EntityDatabaseConnection connection = GetConnection())
            {
                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Select(@"select * from Customers
                                                           select * from Orders
                                                           select * from Products")
                                                 .To(context)
                                                 .Fill<Customer>(e => new { e.LastName, e.NationalIdentityNumber, e.FirstName, e.ID })
                                                 .Fill<Order>(e => new { e.RV, e.ID, e.CustomerID })
                                                 .Fill<Product>(e => new { e.ID, e.Name, e.Manufacturer })
                                                 .Execute();
            }
        }
        [TestMethod]
        public void Select_Prepared_Without_Parameter_Without_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();


            using (EntityDatabaseConnection connection = GetConnection())
            {
                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Select(@"select * from Customers
                                                           select * from Orders
                                                           select * from Products")
                                                 .To(context)
                                                 .Fill<Customer>()
                                                 .Fill<Order>()
                                                 .Fill<Product>()
                                                 .Execute();
            }
        }

        [TestMethod]
        public void Select_Adhoc_Without_Parameter_With_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();


            using (EntityDatabaseConnection connection = GetConnection())
            {
                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Adhoc()
                                                 .Select(@"select * from Customers
                                                           select * from Orders
                                                           select * from Products")
                                                 .To(context)
                                                 .Fill<Customer>(e => new { e.LastName, e.NationalIdentityNumber, e.FirstName, e.ID })
                                                 .Fill<Order>(e => new { e.RV, e.ID, e.CustomerID })
                                                 .Fill<Product>(e => new { e.ID, e.Name, e.Manufacturer })
                                                 .Execute();
            }
        }
        [TestMethod]
        public void Select_Adhoc_Without_Parameter_Without_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();


            using (EntityDatabaseConnection connection = GetConnection())
            {
                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Adhoc()
                                                 .Select(@"select * from Customers
                                                           select * from Orders
                                                           select * from Products")
                                                 .To(context)
                                                 .Fill<Customer>()
                                                 .Fill<Order>()
                                                 .Fill<Product>()
                                                 .Execute();
            }
        }

        [TestMethod]
        public void Select_Prepared_With_Parameter_With_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();

            var consideredCustomers = UserDefinedTableTypeCreator.Create("BigIntIDs", () => new { ID = default(long) })
                                                                 .Add(new { ID = 15L })
                                                                 .Add(new { ID = 19L });

            using (EntityDatabaseConnection connection = GetConnection())
            {
                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Select(@"select * 
                                                           from Customers c
                                                             inner join @ConsideredCustomers cc on cc.ID = c.ID
                                                           
                                                           declare @Orders table (ID bigint, CustomerID int, ShippingDate date, RV binary(8))
                                                           
                                                           insert into @Orders
                                                             select o.ID, o.CustomerID, o.ShippingDate, o.RV
                                                           from Orders o
                                                             inner join @ConsideredCustomers cc on cc.ID = o.CustomerID
                                                           where o.ReceiveDate > parse('1398/02/01' as date using 'fa-IR')
                                                           
                                                           select * 
                                                           from @Orders o
                                                           order by o.ShippingDate

                                                           select *
                                                           from OrderDetails od
                                                             inner join @Orders o on o.ID = od.OrderID

                                                           select * 
                                                           from Products
                                                           where ID = @ProductID")
                                                 .WithParameters(new { ProductID = 6005, ConsideredCustomers = consideredCustomers })
                                                 .To(context)
                                                 .Fill<Customer>(e => new { e.LastName, e.NationalIdentityNumber, e.FirstName, e.ID })
                                                 .Fill<Order>(e => new { e.ShippingDate, e.RV, e.ID })
                                                 .Fill<OrderDetail>(e => new { e.Amount, e.RV, e.Fee, e.ID, e.OrderID }) // OrderID is not null column, therefore it cann't be omitted in query, and must be loaded in model
                                                 .Fill<Product>(e => new { e.ID, e.Name, e.Manufacturer })
                                                 .Execute();
            }
        }
        [TestMethod]
        public void Select_Adhoc_With_Parameter_With_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();

            var consideredCustomers = UserDefinedTableTypeCreator.Create("BigIntIDs", () => new { ID = default(long) })
                                                                 .Add(new { ID = 15L })
                                                                 .Add(new { ID = 19L });

            using (EntityDatabaseConnection connection = GetConnection())
            {
                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Adhoc()
                                                 .Select(@"select * 
                                                           from Customers c
                                                             inner join @ConsideredCustomers cc on cc.ID = c.ID
                                                           
                                                           declare @Orders table (ID bigint, CustomerID int, ShippingDate date, RV binary(8))
                                                           
                                                           insert into @Orders
                                                             select o.ID, o.CustomerID, o.ShippingDate, o.RV
                                                           from Orders o
                                                             inner join @ConsideredCustomers cc on cc.ID = o.CustomerID
                                                           where o.ReceiveDate > parse('1398/02/01' as date using 'fa-IR')
                                                           
                                                           select * 
                                                           from @Orders o
                                                           order by o.ShippingDate

                                                           select *
                                                           from OrderDetails od
                                                             inner join @Orders o on o.ID = od.OrderID

                                                           select * 
                                                           from Products
                                                           where ID = @ProductID")
                                                 .WithParameters(new { ProductID = 6005, ConsideredCustomers = consideredCustomers })
                                                 .To(context)
                                                 .Fill<Customer>(e => new { e.LastName, e.NationalIdentityNumber, e.FirstName, e.ID })
                                                 .Fill<Order>(e => new { e.ShippingDate, e.RV, e.ID })
                                                 .Fill<OrderDetail>(e => new { e.Amount, e.RV, e.Fee, e.ID, e.OrderID }) // OrderID is not null column, therefore it cann't be omitted in query, and must be loaded in model
                                                 .Fill<Product>(e => new { e.ID, e.Name, e.Manufacturer })
                                                 .Execute();
            }
        }

        [TestMethod]
        public void Select_With_Transaction_With_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();


            using (EntityDatabaseConnection connection = GetConnection())
            {
                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Transaction(IsolationLevel.Snapshot)
                                                 .Select(@"select * from Customers
                                                           select * from Orders
                                                           select * from Products")
                                                 .To(context)
                                                 .Fill<Customer>(e => new { e.LastName, e.NationalIdentityNumber, e.FirstName, e.ID })
                                                 .Fill<Order>(e => new { e.RV, e.ID, e.CustomerID })
                                                 .Fill<Product>(e => new { e.ID, e.Name, e.Manufacturer })
                                                 .Execute();
            }
        }
        [TestMethod]
        public void Select_With_Transaction_Without_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();


            using (EntityDatabaseConnection connection = GetConnection())
            {
                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Transaction(IsolationLevel.Snapshot)
                                                 .Select(@"select * from Customers
                                                           select * from Orders
                                                           select * from Products")
                                                 .To(context)
                                                 .Fill<Customer>()
                                                 .Fill<Order>()
                                                 .Fill<Product>()
                                                 .Execute();
            }
        }


        [TestMethod]
        public void SelectAsync_Prepared_With_Transaction_With_Parameter_With_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();

            var consideredCustomers = UserDefinedTableTypeCreator.Create("BigIntIDs", () => new { ID = default(long) })
                                                                 .Add(new { ID = 15L })
                                                                 .Add(new { ID = 19L });

            EntityDatabaseConnection connection = GetConnection();

            var t = EntityDatabase<EnterpriseContext>.Connect(connection)
                                                     .Transaction(IsolationLevel.Snapshot)
                                                     .Select(@"select * 
                                                               from Customers c
                                                                 inner join @ConsideredCustomers cc on cc.ID = c.ID
                                                           
                                                               declare @Orders table (ID bigint, CustomerID int, ShippingDate date, RV binary(8))
                                                           
                                                               insert into @Orders
                                                                 select o.ID, o.CustomerID, o.ShippingDate, o.RV
                                                               from Orders o
                                                                 inner join @ConsideredCustomers cc on cc.ID = o.CustomerID
                                                               where o.ReceiveDate > parse('1398/02/01' as date using 'fa-IR')
                                                           
                                                               select * 
                                                               from @Orders o
                                                               order by o.ShippingDate

                                                               select *
                                                               from OrderDetails od
                                                                 inner join @Orders o on o.ID = od.OrderID

                                                               select * 
                                                               from Products
                                                               where ID = @ProductID")
                                                     .WithParameters(new { ProductID = 6005, ConsideredCustomers = consideredCustomers })
                                                     .To(context)
                                                     .Fill<Customer>(e => new { e.LastName, e.NationalIdentityNumber, e.FirstName, e.ID })
                                                     .Fill<Order>(e => new { e.ShippingDate, e.RV, e.ID })
                                                     .Fill<OrderDetail>(e => new { e.Amount, e.RV, e.Fee, e.ID, e.OrderID }) // OrderID is not null column, therefore it cann't be omitted in query, and must be loaded in model
                                                     .Fill<Product>(e => new { e.ID, e.Name, e.Manufacturer })
                                                     .ExecuteAsync();

            t.Wait();
        }
        [TestMethod]
        public void SelectAsync_Prepared_With_Transaction_Without_Parameter_Without_CallBack_BigData()
        {
            EnterpriseContext context = new EnterpriseContext();

            EntityDatabaseConnection connection = GetConnection();

            var t = EntityDatabase<EnterpriseContext>.Connect(connection)
                                                     .Transaction(IsolationLevel.Snapshot)
                                                     .Select(@"select * 
                                                               from Customers c
                                                               
                                                               select * 
                                                               from Customers c
                                                               
                                                               select *
                                                               from Customers c")
                                                     .To(context)
                                                     .Fill<Customer>()
                                                     .Merge<Customer>()
                                                     .Merge<Customer>()
                                                     .ExecuteAsync();

            t.Wait();
        }




        [TestMethod]
        public void Save_Data_WithEvent_WithCheckConcurrency()
        {
            void EntitySaving(EntityDatabaseContext<EnterpriseContext> sender, EntitySavingEventArgs<EnterpriseContext> e)
            {
                TestContext.WriteLine($"Saving {e.Entity.State}, {e.Entity}");
            }
            void Setter_EntitySaved(EntityDatabaseContext<EnterpriseContext> sender, EntitySavedEventArgs<EnterpriseContext> e)
            {
                TestContext.WriteLine($"Saved {e.State}, {e.Entity}");
            }

            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Context(context)
                                                 .Table<Customer>()
                                                    .Insert().WithCallBack(e => new { e.RV })
                                                    .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                    .Delete().WithCheckConcurrency(e => new { e.RV })
                                                 .Table<Order>()
                                                    .Insert().WithCallBack(e => new { e.RV })
                                                    .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                    .Delete().WithCheckConcurrency(e => new { e.RV })
                                                 .Event()
                                                    .Set(setter => setter.EntitySaving += EntitySaving)
                                                    .Set(setter => setter.EntitySaved += Setter_EntitySaved)
                                                 .Save(false);

                connection.CommitTransaction();
                connection.Close();
            }

            Data(Save);
        }

        [TestMethod]
        public void Save_Conflict_WithCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                (context, connection).WithCallBack().Save(false);
            }

            Conflict(Save);
        }
        [TestMethod]
        public void Save_Conflict_WithoutCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                (context, connection).WithoutCallBack().Save(false);
            }

            Conflict(Save);
        }
        [TestMethod]
        public void Save_Conflict_WithCheckConcurrency()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();

                (context, connection).WithCheckConcurrency().Save(false);
            }

            Conflict(Save);
        }




        private void Data(Action<EnterpriseContext> save)
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Customer amir = new Customer();
            amir.FirstName = "Amir";
            context.Customers.Add(amir);

            Customer sam = new Customer();
            context.Customers.Add(sam);


            Order order1 = new Order();
            order1.ReceiveDate = "1398/05/06".ToDateTime();
            order1.ShippingDate = "1398/06/06".ToDateTime();
            context.Orders.Add(order1);

            Order order2 = new Order();
            context.Orders.Add(order2);

            Order order3 = new Order();
            context.Orders.Add(order3);


            #region insert amir, sam, order2 and order3

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();


                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>().Insert().WithCallBack(e => new { e.RV })
                                                 .Write(amir, sam);

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Order>().Insert().WithCallBack(e => new { e.RV })
                                                 .Write(order2, order3);
                connection.Close();
            }

            #endregion insert amir, sam, order2 and order3

            amir.LastName = "Zare";

            context.Customers.Remove(sam);

            order2.CustomerID = ali.ID;

            context.Orders.Remove(order3);




            #region check

            ali.CheckAdded();
            amir.CheckModified();
            sam.CheckDeleted();

            order1.CheckAdded();
            order2.CheckModified();
            order3.CheckDeleted();

            context.Changes.CheckCount(6)
                           .CheckFound(ali)
                           .CheckFound(amir)
                           .CheckFound(sam)
                           .CheckFound(order1)
                           .CheckFound(order2)
                           .CheckFound(order3);

            #endregion check

            bool has_concurrency_conflict = false;

            try
            {
                context.Execute(save);
            }
            catch (EntityDatabaseConcurrencyException)
            {
                has_concurrency_conflict = true;
            }

            #region check

            if (has_concurrency_conflict)
            {
                ali.CheckAdded();
                amir.CheckModified();
                sam.CheckDeleted();

                order1.CheckAdded();
                order2.CheckModified();
                order3.CheckDeleted();

                context.Changes.CheckCount(6)
                               .CheckFound(ali)
                               .CheckFound(amir)
                               .CheckFound(sam)
                               .CheckFound(order1)
                               .CheckFound(order2)
                               .CheckFound(order3);
            }
            else
            {
                ali.CheckUnchanged();
                amir.CheckUnchanged();
                sam.CheckDetached();

                order1.CheckUnchanged();
                order2.CheckUnchanged();
                order3.CheckDetached();

                context.Changes.CheckCount(0);
            }

            #endregion check
        }
        private void Conflict(Action<EnterpriseContext> save)
        {
            EnterpriseContext context = new EnterpriseContext();

            #region step1: define some new data ...

            Order order = new Order();
            order.ReceiveDate = "1398/06/25".ToDateTime();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order;
            orderDetail.Amount = 100;
            context.OrderDetails.Add(orderDetail);

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            order.Customer = ali;

            Product laptop = new Product();
            laptop.Name = "Laptop Asus N552";
            context.Products.Add(laptop);

            orderDetail.Product = laptop;


            #region check

            laptop.CheckAdded();

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.CustomerID.Check(ali.ID);
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.OrderID.Check(order.ID);
            orderDetail.ProductID.Check(laptop.ID);

            context.Changes.CheckCount(4)
                           .CheckFound(laptop)
                           .CheckFound(ali)
                           .CheckFound(order)
                           .CheckFound(orderDetail);

            #endregion check

            // save changes
            context.Execute(save);

            #region check

            laptop.CheckUnchanged();

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckUnchanged();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckUnchanged();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);
            orderDetail.Product.Check(laptop);
            orderDetail.ProductID.Check(laptop.ID);

            context.Changes.CheckCount(0);

            #endregion check

            #endregion step1: define some new data ...


            // Test Snapshot ...
            #region step2: define some new data and edit previous data ...

            Order order1 = new Order();
            order1.Customer = ali;
            order1.ReceiveDate = "1398/07/07".ToDateTime();
            context.Orders.Add(order1);

            OrderDetail orderDetail1 = new OrderDetail();
            orderDetail1.Order = order1;
            orderDetail1.Amount = 500;
            context.OrderDetails.Add(orderDetail1);

            Customer amir = new Customer();
            amir.FirstName = "Amir";
            amir.LastName = "Zare";
            context.Customers.Add(amir);

            order.Customer = amir;

            Product tv = new Product();
            tv.Name = "Sony NX 40 inch";
            context.Products.Add(tv);

            orderDetail.Product = tv;
            orderDetail1.Product = laptop;

            #region check

            laptop.CheckUnchanged();

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order1);

            order.CheckModified();
            order.CustomerID.Check(amir.ID);
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            orderDetail.CheckModified();
            orderDetail.ProductID.Check(tv.ID);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.ProductID);

            tv.CheckAdded();

            amir.CheckAdded();
            amir.Orders.CheckCount(1).CheckItem(0, order);

            order1.CheckAdded();
            order1.CustomerID.Check(ali.ID);
            order1.OrderDetails.CheckCount(1).CheckItem(0, orderDetail1);

            orderDetail1.CheckAdded();
            orderDetail1.OrderID.Check(order1.ID);
            orderDetail1.ProductID.Check(laptop.ID);

            context.Changes.CheckCount(6)
                           .CheckFound(tv)
                           .CheckFound(amir)
                           .CheckFound(order)
                           .CheckFound(order1)
                           .CheckFound(orderDetail)
                           .CheckFound(orderDetail1);

            #endregion check

            bool has_concurrency_conflict = false;

            #region parallel delete

            EntityDatabaseConnection connection = GetConnection();

            connection.Open();

            Database.Connect(connection)
                    .Prepared()
                    .WithParameters(new { OrderID = order.ID })
                    .Query(@"set nocount on;

                             delete 
                             from OrderDetails
                             where OrderID = @OrderID

                             delete 
                             from Orders
                             where ID = @OrderID")
                    .Execute();

            connection.Close();

            #endregion parallel delete

            try
            {
                // save changes
                context.Execute(save);
            }
            catch (EntityDatabaseConcurrencyException)
            {
                has_concurrency_conflict = true;
            }

            #region check

            if (has_concurrency_conflict)
            {
                laptop.CheckUnchanged();

                ali.CheckUnchanged();
                ali.Orders.CheckCount(1).CheckItem(0, order1);

                order.CheckModified();
                order.CustomerID.Check(amir.ID);
                order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);
                order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

                orderDetail.CheckModified();
                orderDetail.ProductID.Check(tv.ID);
                orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.ProductID);

                tv.CheckAdded();

                amir.CheckAdded();
                amir.Orders.CheckCount(1).CheckItem(0, order);

                order1.CheckAdded();
                order1.CustomerID.Check(ali.ID);
                order1.OrderDetails.CheckCount(1).CheckItem(0, orderDetail1);

                orderDetail1.CheckAdded();
                orderDetail1.OrderID.Check(order1.ID);
                orderDetail1.ProductID.Check(laptop.ID);

                context.Changes.CheckCount(6)
                               .CheckFound(tv)
                               .CheckFound(amir)
                               .CheckFound(order)
                               .CheckFound(order1)
                               .CheckFound(orderDetail)
                               .CheckFound(orderDetail1);

            }
            else
            {
                throw new Exception("Expected Exception, Was Not Thrown");

                #region not reachable code

                tv.CheckUnchanged();
                laptop.CheckUnchanged();

                ali.CheckUnchanged();
                ali.Orders.CheckCount(1).CheckItem(0, order1);

                amir.CheckUnchanged();
                amir.Orders.CheckCount(1).CheckItem(0, order);

                order.CheckUnchanged();
                order.Customer.Check(amir);
                order.CustomerID.Check(amir.ID);
                order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

                order1.CheckUnchanged();
                order1.Customer.Check(ali);
                order1.CustomerID.Check(ali.ID);
                order1.OrderDetails.CheckCount(1).CheckItem(0, orderDetail1);

                orderDetail.CheckUnchanged();
                orderDetail.Order.Check(order);
                orderDetail.OrderID.Check(order.ID);
                orderDetail.Product.Check(tv);
                orderDetail.ProductID.Check(tv.ID);

                orderDetail1.CheckUnchanged();
                orderDetail1.Order.Check(order1);
                orderDetail1.OrderID.Check(order1.ID);
                orderDetail1.Product.Check(laptop);
                orderDetail1.ProductID.Check(laptop.ID);

                context.Changes.CheckCount(0);

                #endregion not reachable code
            }

            #endregion check

            #endregion step2: define some new data and edit previous data ...
        }
    }
}
