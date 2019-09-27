using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprise.Model;
using Abi.Data;

namespace Abi.Test.TrackChanges.OneToMany_Nullable
{
    [TestClass]
    public class ExceptionTest
    {
        public TestContext TestContext { get; set; }



        [TestMethod]
        public void Add_Duplicate_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Add(ali);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 111, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_Duplicate_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Add(ali);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 111, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void Add_DuplicateKey_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = -5;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = -5;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_DuplicateKey_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 15;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_Added_NewCustomer()
        {
            // In Real World, Database [Order] Record, Cannot Reference To New [Customer] That Is Not Exists In Database
            // Therefore First [Order] Must be Added, That Cause State Be Changed [Unchanged]
            // Then Can Change [Order] Customer/CustomerID Property, That Cause State Be Changed [Modified]
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_Added_NewCustomerKey()
        {
            // In Real World, Database [Order] Record, Cannot Reference To New [Customer] That Is Not Exists In Database
            // Therefore First [Order] Must be Added, That Cause State Be Changed [Unchanged]
            // Then Can Change [Order] Customer/CustomerID Property, That Cause State Be Changed [Modified]
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_NotAdded_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_NotAdded_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                ali.CheckUnchanged();
                ali.Orders.CheckCount(1).CheckItem(0, order);

                order.CheckUnchanged();
                order.Customer.Check(ali);
                order.CustomerID.Check(ali.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_NotAdded_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_NotAdded_NewCustomerKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_NotAdded_NewCustomerKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = -5;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_Removed_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_Removed_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_Removed_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_Removed_NewCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }


        [TestMethod]
        public void Add_NewOrder_With_Ref_To_NotAdded_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_NotAdded_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                ali.CheckUnchanged();
                ali.Orders.CheckCount(1).CheckItem(0, order);

                order.CheckAdded();
                order.Customer.Check(ali);
                order.CustomerID.Check(ali.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_NotAdded_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_NotAdded_NewCustomerKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_NotAdded_NewCustomerKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = -5;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Order order = new Order();
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void Add_NewOrder_With_Ref_To_Removed_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_Removed_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_Removed_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_Removed_NewCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();
                order.CustomerID = ali.ID;
                context.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void Remove_NotAdded_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Remove(ali);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetRemoveException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Remove_NotAdded_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Remove(ali);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetRemoveException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void Remove_NewCustomer_Related_To_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                context.Customers.Remove(ali);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanRemoveException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 123, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Remove_ExistCustomer_Related_To_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                context.Customers.Remove(ali);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanRemoveException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 123, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Remove_ExistCustomer_Related_To_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                context.Customers.Remove(ali);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanRemoveException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 123, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_ExistOrder_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID; // Same Key As ali
                context.Orders.Add(order);

                ali.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionInsertException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_NewOrder_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.CustomerID = ali.ID; // Same Key As ali
                context.Orders.Add(order);

                ali.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionInsertException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                ali.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityRelationManagerCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 211, e.Message);
                Assert.IsTrue((e = e.Next()).GetType() == typeof(EntitySetAcceptEntityException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Remove_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID;

                ali.Orders.Remove(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionRemoveAtException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Remove_NotAdded_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.CustomerID = ali.ID;

                ali.Orders.Remove(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionRemoveAtException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }


        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Added_NewOrder_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.CustomerID = ali.ID; // Same Key As ali
                context.Orders.Add(order);

                ali.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionInsertException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;

                ali.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_ExistOrder_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID; // Same Key As ali

                ali.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                ali.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityRelationManagerCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 211, e.Message);
                Assert.IsTrue((e = e.Next()).GetType() == typeof(EntitySetAcceptEntityException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Remove_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.ID = 505;
                order.CustomerID = ali.ID;

                ali.Orders.Remove(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionRemoveAtException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Remove_NotAdded_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Order order = new Order();
                order.CustomerID = ali.ID;

                ali.Orders.Remove(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionRemoveAtException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void NewOrder_EditForeign_With_NotAdded_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 16;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_NotAdded_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 16;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                ali.CheckUnchanged();
                ali.Orders.CheckCount(0);

                amir.CheckUnchanged();
                amir.Orders.CheckCount(1).CheckItem(0, order);

                order.CheckAdded();
                order.Customer.Check(amir);
                order.CustomerID.Check(amir.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_NotAdded_NewCustomer_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_NotAdded_NewCustomer_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = -5;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_NotAdded_NewCustomerKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_NotAdded_NewCustomerKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = -5;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewOrder_EditForeign_With_Removed_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 16;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);
                context.Customers.Remove(amir);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_Removed_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 16;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);
                context.Customers.Remove(amir);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_Removed_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);
                context.Customers.Remove(amir);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_Removed_NewCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);
                context.Customers.Remove(amir);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewOrder_EditForeign_To_UnknownCustomer_FromOtherContext_Without_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = -10;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = -20;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                new EnterpriseContext().Customers.Add(amir);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EditForeign_To_UnknownCustomer_FromOtherContext_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = -5;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = -5;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                new EnterpriseContext().Customers.Add(amir);

                Order order = new Order();
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }


        [TestMethod]
        public void ExistOrder_EditForeign_With_NotAdded_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 16;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_NotAdded_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 16;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                ali.CheckUnchanged();
                ali.Orders.CheckCount(0);

                amir.CheckUnchanged();
                amir.Orders.CheckCount(1).CheckItem(0, order);

                order.CheckModified();
                order.Customer.Check(amir);
                order.CustomerID.Check(amir.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_NotAdded_NewCustomer_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_NotAdded_NewCustomer_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = -5;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_NotAdded_NewCustomerKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_NotAdded_NewCustomerKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = -5;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistOrder_EditForeign_With_Removed_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 16;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);
                context.Customers.Remove(amir);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_Removed_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 16;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);
                context.Customers.Remove(amir);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistOrder_EditForeign_With_Removed_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);
                context.Customers.Remove(amir);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_Removed_NewCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                context.Customers.Add(amir);
                context.Customers.Remove(amir);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistOrder_EditForeign_To_UnknownCustomer_FromOtherContext_Without_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 16;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                new EnterpriseContext().Customers.Add(amir);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EditForeign_To_UnknownCustomer_FromOtherContext_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Customer amir = new Customer();
                amir.ID = 15;
                amir.FirstName = "Amir";
                amir.LastName = "Zare";
                new EnterpriseContext().Customers.Add(amir);

                Order order = new Order();
                order.ID = 505;
                order.Customer = ali;
                context.Orders.Add(order);

                order.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void Removed_ExistCustomer_EntityCollection_Add_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();
                order.ID = 505;

                ali.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionInsertException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Removed_NewCustomer_EntityCollection_Add_NotAdded_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Order order = new Order();

                ali.Orders.Add(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionInsertException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

    }
}
