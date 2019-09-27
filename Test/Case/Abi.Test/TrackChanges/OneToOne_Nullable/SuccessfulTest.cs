using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprise.Model;

namespace Abi.Test.TrackChanges.OneToOne_Nullable
{
    [TestClass]
    public class SuccessfulTest
    {
        [TestMethod]
        public void Add_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            order.CheckAdded();

            order.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void Add_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            order.CheckUnchanged();

            order.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(0);
        }

        [TestMethod]
        public void Modify_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            order.CheckAdded();

            order.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void Modify_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            order.ReceiveDate = "1398/06/06".ToDateTime();

            order.CheckModified();

            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }








        [TestMethod]
        public void Add_NewShipment_With_Ref_To_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void Add_NewShipment_With_Ref_To_Added_NewOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = order.ID;
            context.Shipments.Add(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void Add_NewShipment_With_Ref_To_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void Add_NewShipment_With_Ref_To_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = order.ID;
            context.Shipments.Add(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }








        [TestMethod]
        public void Add_ExistShipment_With_Ref_To_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order;
            context.Shipments.Add(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckUnchanged();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Add_ExistShipment_With_Ref_To_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = order.ID;
            context.Shipments.Add(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckUnchanged();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

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
        public void Remove_NewShipment_With_Ref_To_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);
            context.Shipments.Remove(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(null);

            shipment.CheckDetached();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void Remove_NewShipment_With_Ref_To_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);
            context.Shipments.Remove(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            shipment.CheckDetached();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(0);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Remove_NewShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Shipment shipment = new Shipment();
            shipment.OrderID = 5; // this line for test UpdateRelated > previousForeign > Orphan
            context.Shipments.Add(shipment);
            context.Shipments.Remove(shipment);

            shipment.CheckDetached();

            context.Shipments.CheckCount(0);

            context.Changes.CheckCount(0);
        }

        [TestMethod]
        public void Remove_ExistShipment_With_Ref_To_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order;
            context.Shipments.Add(shipment);
            context.Shipments.Remove(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            shipment.CheckDeleted();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void Remove_ExistShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = 5; // this line for test UpdateRelated > previousForeign > Orphan
            context.Shipments.Add(shipment);
            context.Shipments.Remove(shipment);

            shipment.CheckDeleted();

            context.Shipments.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }








        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Added_ExistShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Added_ExistShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = 11;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Added_ExistShipment_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = order1.ID;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Added_ExistShipment_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order1;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Added_NewShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Added_NewShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = 11;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Added_NewShipment_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.OrderID = order1.ID;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_Added_NewShipment_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.Order = order1;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }

        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_ExistShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckUnchanged();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_ExistShipment_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = order.ID; // Same Key As order

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckUnchanged();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_ExistShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = 11;

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_ExistShipment_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = order1.ID;

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_ExistShipment_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order1;

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_NewShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_NewShipment_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = order.ID; // Same Key As order

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_NewShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = 11;

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_NewShipment_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.OrderID = order1.ID;

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Add_NotAdded_NewShipment_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.Order = order1;

            order.Shipment.Set(shipment);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }


        [TestMethod]
        public void NewOrder_EntityUnique_Add_Added_ExistShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_Added_ExistShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = 11;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_Added_ExistShipment_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = order1.ID;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_Added_ExistShipment_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order1;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_Added_NewShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_Added_NewShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = 11;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_Added_NewShipment_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.OrderID = order1.ID;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_Added_NewShipment_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.Order = order1;
            context.Shipments.Add(shipment);

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }

        [TestMethod]
        public void NewOrder_EntityUnique_Add_NotAdded_ExistShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = 11;

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_NotAdded_ExistShipment_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = order1.ID;

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_NotAdded_ExistShipment_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order1;

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_NotAdded_NewShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_NotAdded_NewShipment_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = order.ID;

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_NotAdded_NewShipment_With_Ref_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = 11;

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_NotAdded_NewShipment_With_Ref_To_Other_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.OrderID = order1.ID;

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewOrder_EntityUnique_Add_NotAdded_NewShipment_With_Ref_To_Other_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.Order = order1;

            order.Shipment.Set(shipment);

            order.CheckAdded();
            order.Shipment.CheckItem(shipment);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }


        [TestMethod]
        public void ExistOrder_EntityUnique_Remove_Added_ExistShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = 505;
            context.Shipments.Add(shipment);

            order.Shipment.Set(null);

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(null);
            shipment.OrderID.Check(null);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistOrder_EntityUnique_Remove_Added_NewShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = 505;
            context.Shipments.Add(shipment);

            order.Shipment.Set(null);

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(null);
            shipment.OrderID.Check(null);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }


        [TestMethod]
        public void NewOrder_EntityUnique_Remove_Added_NewShipment()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.OrderID = order.ID;
            context.Shipments.Add(shipment);

            order.Shipment.Set(null);

            order.CheckAdded();
            order.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(null);
            shipment.OrderID.Check(null);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }








        [TestMethod]
        public void NewShipment_EditForeign_With_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.Order = order1;

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order1);
            shipment.OrderID.Check(order1.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

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

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.Order = order1;

            order.CheckAdded();
            order.Shipment.CheckItem(null);

            order1.CheckAdded();
            order1.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order1);
            shipment.OrderID.Check(order1.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(3).CheckFound(order).CheckFound(order1).CheckFound(shipment);
        }
        [TestMethod]
        public void NewShipment_EditForeign_With_Added_NewOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.OrderID = order1.ID;

            order.CheckAdded();
            order.Shipment.CheckItem(null);

            order1.CheckAdded();
            order1.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order1);
            shipment.OrderID.Check(order1.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(3).CheckFound(order).CheckFound(order1).CheckFound(shipment);
        }


        [TestMethod]
        public void NewShipment_EditForeign_To_Null()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.Order = null;

            order.CheckAdded();
            order.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(null);
            shipment.OrderID.Check(null);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }
        [TestMethod]
        public void NewShipment_EditForeign_To_NullKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.OrderID = null;

            order.CheckAdded();
            order.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(null);
            shipment.OrderID.Check(null);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }

        [TestMethod]
        public void NewShipment_EditForeign_To_OrphanOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;

            Shipment shipment = new Shipment();
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.OrderID = order1.ID;

            order.CheckAdded();
            order.Shipment.CheckItem(null);

            shipment.CheckAdded();
            shipment.Order.Check(null);
            shipment.OrderID.Check(order1.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order).CheckFound(shipment);
        }


        [TestMethod]
        public void ExistShipment_EditForeign_With_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.Order = order1;

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            order1.CheckUnchanged();
            order1.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order1);
            shipment.OrderID.Check(order1.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_Added_ExistOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            order1.ID = 506;
            context.Orders.Add(order1);

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

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.Order = order1;

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            order1.CheckAdded();
            order1.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order1);
            shipment.OrderID.Check(order1.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistShipment_EditForeign_With_Added_NewOrderKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Order order1 = new Order();
            context.Orders.Add(order1);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.OrderID = order1.ID;

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            order1.CheckAdded();
            order1.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order1);
            shipment.OrderID.Check(order1.ID);

            order.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(2).CheckFound(order).CheckFound(order1);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(2).CheckFound(order1).CheckFound(shipment);
        }


        [TestMethod]
        public void ExistShipment_EditForeign_To_Null()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.Order = null;

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(null);
            shipment.OrderID.Check(null);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void ExistShipment_EditForeign_To_NullKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.Order = order;
            context.Shipments.Add(shipment);

            shipment.OrderID = null;

            order.CheckUnchanged();
            order.Shipment.CheckItem(null);

            shipment.CheckModified();
            shipment.Order.Check(null);
            shipment.OrderID.Check(null);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }

        [TestMethod]
        public void ExistShipment_EditForeign_To_OrphanOrder()
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

            shipment.CheckModified();
            shipment.Order.Check(null);
            shipment.OrderID.Check(order1.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }








        [TestMethod]
        public void Orphan1()
        {
            EnterpriseContext context = new EnterpriseContext();

            Shipment shipment = new Shipment();
            shipment.OrderID = 5;
            context.Shipments.Add(shipment);

            Order order = new Order();
            order.ID = 5;
            context.Orders.Add(order);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckAdded();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
        [TestMethod]
        public void Orphan2()
        {
            EnterpriseContext context = new EnterpriseContext();

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = 5;
            context.Shipments.Add(shipment);

            Order order = new Order();
            order.ID = 5;
            context.Orders.Add(order);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckUnchanged();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(0);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Orphan3()
        {
            EnterpriseContext context = new EnterpriseContext();

            Shipment shipment = new Shipment();
            shipment.ID = 53952148;
            shipment.OrderID = 4;
            context.Shipments.Add(shipment);
            shipment.OrderID = 5;

            Order order = new Order();
            order.ID = 5;
            context.Orders.Add(order);

            order.CheckUnchanged();
            order.Shipment.CheckItem(shipment);

            shipment.CheckModified();
            shipment.Order.Check(order);
            shipment.OrderID.Check(order.ID);

            order.GetChangedProperties().CheckCount(0);
            shipment.GetChangedProperties().CheckCount(1).CheckFound<Shipment>(s => s.OrderID);

            context.Orders.CheckCount(1).CheckFound(order);
            context.Shipments.CheckCount(1).CheckFound(shipment);

            context.Changes.CheckCount(1).CheckFound(shipment);
        }
    }
}
