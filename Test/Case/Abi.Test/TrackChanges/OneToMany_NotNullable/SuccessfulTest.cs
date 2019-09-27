using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprise.Model;

namespace Abi.Test.TrackChanges.OneToMany_NotNullable
{
    [TestClass]
    public partial class SuccessfulTest
    {
        [TestMethod]
        public void Add_NewOrderDetail_With_Ref_To_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void Add_NewOrderDetail_With_Ref_To_Added_NewOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = order.ID;
            context.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void Add_NewOrderDetail_With_Ref_To_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void Add_NewOrderDetail_With_Ref_To_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = order.ID;
            context.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }


        [TestMethod]
        public void Add_ExistOrderDetail_With_Ref_To_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckUnchanged();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Add_ExistOrderDetail_With_Ref_To_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = order.ID;
            context.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckUnchanged();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(0);
        }








        [TestMethod]
        public void Remove_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);
            context.Orders.Remove(order);

            order.CheckDetached();

            context.Orders.CheckCount(0);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Remove_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);
            context.Orders.Remove(order);

            order.CheckDeleted();

            context.Orders.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(order);
        }

        [TestMethod]
        public void Remove_NewOrderDetail_With_Ref_To_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);
            context.OrderDetails.Remove(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(0);

            orderDetail.CheckDetached();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void Remove_NewOrderDetail_With_Ref_To_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);
            context.OrderDetails.Remove(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(0);

            orderDetail.CheckDetached();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(0);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Remove_NewOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = 5; // this line for test UpdateRelated > previousForeign > Orphan
            context.OrderDetails.Add(orderDetail);
            context.OrderDetails.Remove(orderDetail);

            orderDetail.CheckDetached();

            context.OrderDetails.CheckCount(0);

            context.Changes.CheckCount(0);
        }

        [TestMethod]
        public void Remove_ExistOrderDetail_With_Ref_To_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);
            context.OrderDetails.Remove(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(0);

            orderDetail.CheckDeleted();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void Remove_ExistOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = 5; // this line for test UpdateRelated > previousForeign > Orphan
            context.OrderDetails.Add(orderDetail);
            context.OrderDetails.Remove(orderDetail);

            orderDetail.CheckDeleted();

            context.OrderDetails.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }








        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Added_ExistOrderDetail()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = 606;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Added_ExistOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = 404;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Added_ExistOrderDetail_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = order2.ID;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Added_ExistOrderDetail_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.Order = order2;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Added_NewOrderDetail()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = 606;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Added_NewOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = 404;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Added_NewOrderDetail_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = order2.ID;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_Added_NewOrderDetail_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order2;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }


        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_ExistOrderDetail()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckUnchanged();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_ExistOrderDetail_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = order.ID; // Same Key As order

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckUnchanged();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_ExistOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = 404;

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_ExistOrderDetail_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = order2.ID;

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_ExistOrderDetail_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.Order = order2;

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }

        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_NewOrderDetail()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_NewOrderDetail_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = order.ID; // Same Key As order

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_NewOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = 404;

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_NewOrderDetail_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = order2.ID;

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrder_EntityCollection_Add_NotAdded_NewOrderDetail_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order2;

            order.OrderDetails.Add(orderDetail);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }


        [TestMethod]
        public void NewOrder_EntityCollection_Add_Added_ExistOrderDetail()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = 606;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_Added_ExistOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = 404;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_Added_ExistOrderDetail_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = order2.ID;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_Added_ExistOrderDetail_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.Order = order2;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_Added_NewOrderDetail()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = 606;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_Added_NewOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = 404;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_Added_NewOrderDetail_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = order2.ID;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_Added_NewOrderDetail_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order2;
            context.OrderDetails.Add(orderDetail);

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }

        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_ExistOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = 404;

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_ExistOrderDetail_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = order2.ID;

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_ExistOrderDetail_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.Order = order2;

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_NewOrderDetail()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_NewOrderDetail_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = order.ID;

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_NewOrderDetail_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = 404;

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_NewOrderDetail_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = order2.ID;

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrder_EntityCollection_Add_NotAdded_NewOrderDetail_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order2;

            order.OrderDetails.Add(orderDetail);

            order.CheckAdded();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(0);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }








        [TestMethod]
        public void NewOrderDetail_EditForeign_With_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            orderDetail.Order = order2;

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(0);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order2);
            orderDetail.OrderID.Check(order2.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

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

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            orderDetail.Order = order2;

            order.CheckAdded();
            order.OrderDetails.CheckCount(0);

            order2.CheckAdded();
            order2.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order2);
            orderDetail.OrderID.Check(order2.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(3).CheckFound(order).CheckFound(order2).CheckFound(orderDetail);
        }
        [TestMethod]
        public void NewOrderDetail_EditForeign_With_Added_NewOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            orderDetail.OrderID = order2.ID;

            order.CheckAdded();
            order.OrderDetails.CheckCount(0);

            order2.CheckAdded();
            order2.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order2);
            orderDetail.OrderID.Check(order2.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(3).CheckFound(order).CheckFound(order2).CheckFound(orderDetail);
        }


        [TestMethod]
        public void NewOrderDetail_EditForeign_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            orderDetail.OrderID = order2.ID;

            order.CheckAdded();
            order.OrderDetails.CheckCount(0);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(null);
            orderDetail.OrderID.Check(order2.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(orderDetail);
        }


        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            orderDetail.Order = order2;

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(0);

            order2.CheckUnchanged();
            order2.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order2);
            orderDetail.OrderID.Check(order2.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            order2.ID = 506;
            context.Orders.Add(order2);

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

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            orderDetail.Order = order2;

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(0);

            order2.CheckAdded();
            order2.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order2);
            orderDetail.OrderID.Check(order2.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order2).CheckFound(orderDetail);
        }
        [TestMethod]
        public void ExistOrderDetail_EditForeign_With_Added_NewOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order2 = new Order();
            context.Orders.Add(order2);

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.Order = order;
            context.OrderDetails.Add(orderDetail);

            orderDetail.OrderID = order2.ID;

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(0);

            order2.CheckAdded();
            order2.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order2);
            orderDetail.OrderID.Check(order2.ID);

            order.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order2);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(2).CheckFound(order2).CheckFound(orderDetail);
        }


        [TestMethod]
        public void ExistOrderDetail_EditForeign_To_OrphanOrder()
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

            orderDetail.CheckModified();
            orderDetail.Order.Check(null);
            orderDetail.OrderID.Check(order2.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }








        [TestMethod]
        public void Orphan1()
        {
            EnterpriseContext context = new EnterpriseContext();

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderID = 505;
            context.OrderDetails.Add(orderDetail);

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckAdded();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }
        [TestMethod]
        public void Orphan2()
        {
            EnterpriseContext context = new EnterpriseContext();

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = 505;
            context.OrderDetails.Add(orderDetail);

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckUnchanged();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Orphan3()
        {
            EnterpriseContext context = new EnterpriseContext();

            OrderDetail orderDetail = new OrderDetail();
            orderDetail.ID = 987654321;
            orderDetail.OrderID = 504;
            context.OrderDetails.Add(orderDetail);
            orderDetail.OrderID = 505;

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            order.CheckUnchanged();
            order.OrderDetails.CheckCount(1).CheckItem(0, orderDetail);

            orderDetail.CheckModified();
            orderDetail.Order.Check(order);
            orderDetail.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            orderDetail.GetChangedProperties().CheckCount(1).CheckFound<OrderDetail>(od => od.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.OrderDetails.CheckCount(1).CheckFound(orderDetail);

            context.Changes.CheckCount(1).CheckFound(orderDetail);
        }

    }
}
