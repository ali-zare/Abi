using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprise.Model;
using Abi.Data;

namespace Abi.Test.TrackChanges.OneToOne_Nullable
{
    [TestClass]
    public class ExceptionTest
    {
        public TestContext TestContext { get; set; }



        [TestMethod]
        public void Add_Duplicate_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Add(order);

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
        public void Add_Duplicate_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Add(order);

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
        public void Add_DuplicateKey_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = -5;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = -5;
                context.Orders.Add(order1);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_DuplicateKey_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 505;
                context.Orders.Add(order1);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void Add_ExistShipment_With_Ref_To_Added_NewOrder()
        {
            // In Real World, Database [Shipment] Record, Cannot Reference To New [Order] That Is Not Exists In Database
            // Therefore First [Shipment] Must be Added, That Cause State Be Changed [Unchanged]
            // Then Can Change [Shipment] Order/OrderID Property, That Cause State Be Changed [Modified]
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

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
        public void Add_ExistShipment_With_Ref_To_Added_NewOrderKey()
        {
            // In Real World, Database [Shipment] Record, Cannot Reference To New [Order] That Is Not Exists In Database
            // Therefore First [Shipment] Must be Added, That Cause State Be Changed [Unchanged]
            // Then Can Change [Shipment] Order/OrderID Property, That Cause State Be Changed [Modified]
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

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
        public void Add_ExistShipment_With_Ref_To_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

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
        public void Add_ExistShipment_With_Ref_To_NotAdded_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

                order.CheckUnchanged();
                order.Shipment.CheckItem(shipment);

                shipment.CheckUnchanged();
                shipment.Order.Check(order);
                shipment.OrderID.Check(order.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistShipment_With_Ref_To_NotAdded_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

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
        public void Add_ExistShipment_With_Ref_To_NotAdded_NewOrderKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

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
        public void Add_ExistShipment_With_Ref_To_NotAdded_NewOrderKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = -5;

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

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
        public void Add_ExistShipment_With_Ref_To_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

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
        public void Add_ExistShipment_With_Ref_To_Removed_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

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
        public void Add_ExistShipment_With_Ref_To_Removed_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

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
        public void Add_ExistShipment_With_Ref_To_Removed_NewOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

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
        public void Add_NewShipment_With_Ref_To_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

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
        public void Add_NewShipment_With_Ref_To_NotAdded_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;

                Shipment shipment = new Shipment();
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

                order.CheckUnchanged();
                order.Shipment.CheckItem(shipment);

                shipment.CheckAdded();
                shipment.Order.Check(order);
                shipment.OrderID.Check(order.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewShipment_With_Ref_To_NotAdded_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

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
        public void Add_NewShipment_With_Ref_To_NotAdded_NewOrderKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();

                Shipment shipment = new Shipment();
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

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
        public void Add_NewShipment_With_Ref_To_NotAdded_NewOrderKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = -5;

                Shipment shipment = new Shipment();
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

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
        public void Add_NewShipment_With_Ref_To_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

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
        public void Add_NewShipment_With_Ref_To_Removed_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

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
        public void Add_NewShipment_With_Ref_To_Removed_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

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
        public void Add_NewShipment_With_Ref_To_Removed_NewOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();
                shipment.OrderID = order.ID;
                context.Shipments.Add(shipment);

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
        public void Remove_NotAdded_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Remove(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetRemoveException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Remove_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Remove(order);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetRemoveException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void Remove_NewOrder_Related_To_NewShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                context.Orders.Remove(order);

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
        public void Remove_ExistOrder_Related_To_NewShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                context.Orders.Remove(order);

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
        public void Remove_ExistOrder_Related_To_ExistShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                context.Orders.Remove(order);

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
        public void ExistOrder_EntityUnique_Add_Added_ExistShipment_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID; // Same Key As order
                context.Shipments.Add(shipment);

                order.Shipment.Set(shipment);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Added_NewShipment_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.OrderID = order.ID; // Same Key As order
                context.Shipments.Add(shipment);

                order.Shipment.Set(shipment);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Removed_ExistShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                context.Shipments.Add(shipment);
                context.Shipments.Remove(shipment);

                order.Shipment.Set(shipment);

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
        public void ExistOrder_EntityUnique_Remove_NotAdded_ExistShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID;

                order.Shipment.Set(null);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Remove_NotAdded_NewShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.OrderID = order.ID;

                order.Shipment.Set(null);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }


        [TestMethod]
        public void NewOrder_EntityUnique_Add_Added_NewShipment_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.OrderID = order.ID; // Same Key As order
                context.Shipments.Add(shipment);

                order.Shipment.Set(shipment);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_NotAdded_ExistShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;

                order.Shipment.Set(shipment);

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
        public void NewOrder_EntityUnique_Add_NotAdded_ExistShipment_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID; // Same Key As order

                order.Shipment.Set(shipment);

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
        public void NewOrder_EntityUnique_Add_Removed_ExistShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                context.Shipments.Add(shipment);
                context.Shipments.Remove(shipment);

                order.Shipment.Set(shipment);

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
        public void NewOrder_EntityUnique_Remove_NotAdded_ExistShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.OrderID = order.ID;

                order.Shipment.Set(null);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Remove_NotAdded_NewShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Shipment shipment = new Shipment();
                shipment.OrderID = order.ID;

                order.Shipment.Set(null);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void NewShipment_EditForeign_With_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 506;

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_NotAdded_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 506;

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                order.CheckUnchanged();
                order.Shipment.CheckItem(null);

                order1.CheckUnchanged();
                order1.Shipment.CheckItem(shipment);

                shipment.CheckAdded();
                shipment.Order.Check(order1);
                shipment.OrderID.Check(order1.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_NotAdded_NewOrder_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order1 = new Order();

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_NotAdded_NewOrder_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = -5;

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_NotAdded_NewOrderKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order1 = new Order();

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_NotAdded_NewOrderKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = -5;

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewShipment_EditForeign_With_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 506;
                context.Orders.Add(order1);
                context.Orders.Remove(order1);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_Removed_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 506;
                context.Orders.Add(order1);
                context.Orders.Remove(order1);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_Removed_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order1 = new Order();
                context.Orders.Add(order1);
                context.Orders.Remove(order1);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_Removed_NewOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order1 = new Order();
                context.Orders.Add(order1);
                context.Orders.Remove(order1);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewShipment_EditForeign_To_UnknownOrder_FromOtherContext_Without_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = -10;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = -20;
                new EnterpriseContext().Orders.Add(order1);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewShipment_EditForeign_To_UnknownOrder_FromOtherContext_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = -5;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = -5;
                new EnterpriseContext().Orders.Add(order1);

                Shipment shipment = new Shipment();
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }


        [TestMethod]
        public void ExistShipment_EditForeign_With_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 506;

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_NotAdded_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 506;

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                order.CheckUnchanged();
                order.Shipment.CheckItem(null);

                order1.CheckUnchanged();
                order1.Shipment.CheckItem(shipment);

                shipment.CheckModified();
                shipment.Order.Check(order1);
                shipment.OrderID.Check(order1.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_NotAdded_NewOrder_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_NotAdded_NewOrder_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = -5;

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_NotAdded_NewOrderKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_NotAdded_NewOrderKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = -5;

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistShipment_EditForeign_With_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 506;
                context.Orders.Add(order1);
                context.Orders.Remove(order1);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_Removed_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 506;
                context.Orders.Add(order1);
                context.Orders.Remove(order1);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistShipment_EditForeign_With_Removed_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                context.Orders.Add(order1);
                context.Orders.Remove(order1);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_Removed_NewOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                context.Orders.Add(order1);
                context.Orders.Remove(order1);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.OrderID = order1.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistShipment_EditForeign_To_UnknownOrder_FromOtherContext_Without_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 506;
                new EnterpriseContext().Orders.Add(order1);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistShipment_EditForeign_To_UnknownOrder_FromOtherContext_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order1 = new Order();
                order1.ID = 505;
                new EnterpriseContext().Orders.Add(order1);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;
                shipment.Order = order;
                context.Shipments.Add(shipment);

                shipment.Order = order1;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void Removed_ExistOrder_EntityUnique_Add_NotAdded_ExistShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();
                shipment.ID = 53952148;

                order.Shipment.Set(shipment);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Removed_NewOrder_EntityUnique_Add_NotAdded_NewShipment()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                Shipment shipment = new Shipment();

                order.Shipment.Set(shipment);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

    }
}
