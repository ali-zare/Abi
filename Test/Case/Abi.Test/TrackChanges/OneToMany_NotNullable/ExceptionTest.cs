using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprise.Model;
using Abi.Data;

namespace Abi.Test.TrackChanges.OneToMany_NotNullable
{
    [TestClass]
    public partial class ExceptionTest
    {
        public TestContext TestContext { get; set; }




        [TestMethod]
        public void Add_NewOrderDetail_Without_Order()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                OrderDetail orderDetail = new OrderDetail();
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_Without_Order()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                context.OrderDetails.Add(orderDetail);

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
                order.ID = -505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = -505;
                context.Orders.Add(order2);

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

                Order order2 = new Order();
                order2.ID = 505;
                context.Orders.Add(order2);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void Add_ExistOrderDetail_With_Ref_To_Added_NewOrder()
        {
            // In Real World, Database [OrderDetail] Record, Cannot Reference To New [Order] That Is Not Exists In Database
            // Therefore First [OrderDetail] Must be Added, That Cause State Be Changed [Unchanged]
            // Then Can Change [OrderDetail] Order/OrderID Property, That Cause State Be Changed [Modified]
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_With_Ref_To_Added_NewOrderKey()
        {
            // In Real World, Database [OrderDetail] Record, Cannot Reference To New [Order] That Is Not Exists In Database
            // Therefore First [OrderDetail] Must be Added, That Cause State Be Changed [Unchanged]
            // Then Can Change [OrderDetail] Order/OrderID Property, That Cause State Be Changed [Modified]
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_With_Ref_To_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_With_Ref_To_NotAdded_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

                order.CheckUnchanged();
                order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

                orderDetail.CheckUnchanged();
                orderDetail.Order.Check(order);
                orderDetail.OrderID.Check(order.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistOrderDetail_With_Ref_To_NotAdded_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_With_Ref_To_NotAdded_NewOrderKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_With_Ref_To_NotAdded_NewOrderKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = -606;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_With_Ref_To_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_With_Ref_To_Removed_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_With_Ref_To_Removed_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_ExistOrderDetail_With_Ref_To_Removed_NewOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_NewOrderDetail_With_Ref_To_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_NewOrderDetail_With_Ref_To_NotAdded_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

                order.CheckUnchanged();
                order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

                orderDetail.CheckAdded();
                orderDetail.Order.Check(order);
                orderDetail.OrderID.Check(order.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewOrderDetail_With_Ref_To_NotAdded_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_NewOrderDetail_With_Ref_To_NotAdded_NewOrderKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_NewOrderDetail_With_Ref_To_NotAdded_NewOrderKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = -606;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_NewOrderDetail_With_Ref_To_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_NewOrderDetail_With_Ref_To_Removed_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_NewOrderDetail_With_Ref_To_Removed_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Add_NewOrderDetail_With_Ref_To_Removed_NewOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.ID;
                context.OrderDetails.Add(orderDetail);

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
        public void Remove_NewOrder_Related_To_NewOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Remove_ExistOrder_Related_To_NewOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void Remove_ExistOrder_Related_To_ExistOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

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
        public void ExistOrder_EntityCollection_Add_Added_ExistOrderDetail_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID; // Same Key As order
                context.OrderDetails.Add(orderDetail);

                order.OrderDetails.Add(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionInsertException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Added_NewOrderDetail_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.ID; // Same Key As order
                context.OrderDetails.Add(orderDetail);

                order.OrderDetails.Add(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionInsertException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Removed_ExistOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = 606;
                context.OrderDetails.Add(orderDetail);
                context.OrderDetails.Remove(orderDetail);

                order.OrderDetails.Add(orderDetail);

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
        public void ExistOrder_EntityCollection_Remove_NotAdded_ExistOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID;

                order.OrderDetails.Remove(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionRemoveAtException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Remove_NotAdded_NewOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.ID;

                order.OrderDetails.Remove(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionRemoveAtException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Remove_Added_ExistOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                order.OrderDetails.Remove(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityRelationManagerCanRemoveException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 221, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Remove_Added_NewOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                order.OrderDetails.Remove(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityRelationManagerCanRemoveException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 221, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }


        [TestMethod]
        public void NewOrder_EntityCollection_Add_Added_NewOrderDetail_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.ID; // Same Key As order
                context.OrderDetails.Add(orderDetail);

                order.OrderDetails.Add(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionInsertException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_ExistOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;

                order.OrderDetails.Add(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanAddException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 113, e.Message);

                TestContext.WriteLine(e.Message);
            };
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_ExistOrderDetail_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID; // Same Key As order

                order.OrderDetails.Add(orderDetail);

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
        public void NewOrder_EntityCollection_Add_Removed_ExistOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = 606;
                context.OrderDetails.Add(orderDetail);
                context.OrderDetails.Remove(orderDetail);

                order.OrderDetails.Add(orderDetail);

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
        public void NewOrder_EntityCollection_Remove_NotAdded_ExistOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.OrderID = order.ID;

                order.OrderDetails.Remove(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionRemoveAtException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Remove_NotAdded_NewOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.ID;

                order.OrderDetails.Remove(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionRemoveAtException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Remove_Added_NewOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                order.OrderDetails.Remove(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityRelationManagerCanRemoveException), e.Message);
                Assert.IsTrue((byte)e.Data["Code"] == 221, e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void NewOrderDetail_EditForeign_With_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 506;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_NotAdded_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 506;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                order.CheckUnchanged();
                order.OrderDetails.CheckCount(0);

                order2.CheckUnchanged();
                order2.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

                orderDetail.CheckAdded();
                orderDetail.Order.Check(order2);
                orderDetail.OrderID.Check(order2.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_NotAdded_NewOrder_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order2 = new Order();

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_NotAdded_NewOrder_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = -606;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_NotAdded_NewOrderKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order2 = new Order();

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_NotAdded_NewOrderKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = -606;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewOrderDetail_EditForeign_With_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 506;
                context.Orders.Add(order2);
                context.Orders.Remove(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_Removed_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 506;
                context.Orders.Add(order2);
                context.Orders.Remove(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_Removed_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order2 = new Order();
                context.Orders.Add(order2);
                context.Orders.Remove(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_Removed_NewOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                Order order2 = new Order();
                context.Orders.Add(order2);
                context.Orders.Remove(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewOrderDetail_EditForeign_To_UnknownOrder_FromOtherContext_Without_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = -10;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = -20;
                new EnterpriseContext().Orders.Add(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_To_UnknownOrder_FromOtherContext_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = -606;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = -606;
                new EnterpriseContext().Orders.Add(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewOrderDetail_EditForeign_To_Null()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = null;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_To_NullKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = 0;
                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }


        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_NotAdded_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 506;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_NotAdded_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 506;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                order.CheckUnchanged();
                order.OrderDetails.CheckCount(0);

                order2.CheckUnchanged();
                order2.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

                orderDetail.CheckModified();
                orderDetail.Order.Check(order2);
                orderDetail.OrderID.Check(order2.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_NotAdded_NewOrder_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_NotAdded_NewOrder_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = -606;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_NotAdded_NewOrderKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_NotAdded_NewOrderKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = -606;

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_Removed_ExistOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 506;
                context.Orders.Add(order2);
                context.Orders.Remove(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_Removed_ExistOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 506;
                context.Orders.Add(order2);
                context.Orders.Remove(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_Removed_NewOrder()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                context.Orders.Add(order2);
                context.Orders.Remove(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_Removed_NewOrderKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                context.Orders.Add(order2);
                context.Orders.Remove(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = order2.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistOrderDetail_EditForeign_To_UnknownOrder_FromOtherContext_Without_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 506;
                new EnterpriseContext().Orders.Add(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_To_UnknownOrder_FromOtherContext_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                Order order2 = new Order();
                order2.ID = 505;
                new EnterpriseContext().Orders.Add(order2);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = order2;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistOrderDetail_EditForeign_To_Null()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.Order = null;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_To_NullKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;
                orderDetail.Order = order;
                context.OrderDetails.Add(orderDetail);

                orderDetail.OrderID = 0;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void Removed_ExistOrder_EntityCollection_Add_NotAdded_ExistOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                order.ID = 505;
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.ID = 987654321;

                order.OrderDetails.Add(orderDetail);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityCollectionInsertException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Removed_NewOrder_EntityCollection_Add_NotAdded_NewOrderDetail()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Order order = new Order();
                context.Orders.Add(order);
                context.Orders.Remove(order);

                OrderDetail orderDetail = new OrderDetail();

                order.OrderDetails.Add(orderDetail);

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
