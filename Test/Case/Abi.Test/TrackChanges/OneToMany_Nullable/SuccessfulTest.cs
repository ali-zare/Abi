using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprise.Model;

namespace Abi.Test.TrackChanges.OneToMany_Nullable
{
    [TestClass]
    public class SuccessfulTest
    {
        [TestMethod]
        public void Add_NewCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.CheckAdded();

            ali.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);

            context.Changes.CheckCount(1).CheckFound(ali);
        }
        [TestMethod]
        public void Add_ExistCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.CheckUnchanged();

            ali.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);

            context.Changes.CheckCount(0);
        }

        [TestMethod]
        public void Modify_NewCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.FirstName = "ali";
            ali.LastName = "zare";

            ali.CheckAdded();

            ali.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);

            context.Changes.CheckCount(1).CheckFound(ali);
        }
        [TestMethod]
        public void Modify_ExistCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.FirstName = "ali";
            ali.LastName = "zare";

            ali.CheckModified();

            ali.GetChangedProperties().CheckCount(2).CheckFound<Customer>(c => c.FirstName).CheckFound<Customer>(c => c.LastName);

            context.Customers.CheckCount(1).CheckFound(ali);

            context.Changes.CheckCount(1).CheckFound(ali);
        }








        [TestMethod]
        public void Add_NewOrder_With_Ref_To_Added_NewCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.Customer = ali;
            context.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_Added_NewCustomerKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = ali.ID;
            context.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_Added_ExistCustomer()
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

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void Add_NewOrder_With_Ref_To_Added_ExistCustomerKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = ali.ID;
            context.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }








        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_Added_ExistCustomer()
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

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckUnchanged();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Add_ExistOrder_With_Ref_To_Added_ExistCustomerKey()
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
            context.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckUnchanged();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(0);
        }








        [TestMethod]
        public void Remove_NewCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);
            context.Customers.Remove(ali);

            ali.CheckDetached();

            context.Customers.CheckCount(0);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Remove_ExistCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);
            context.Customers.Remove(ali);

            ali.CheckDeleted();

            context.Customers.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(ali);
        }

        [TestMethod]
        public void Remove_NewOrder_With_Ref_To_NewCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.Customer = ali;
            context.Orders.Add(order);
            context.Orders.Remove(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(0);

            order.CheckDetached();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(ali);
        }
        [TestMethod]
        public void Remove_NewOrder_With_Ref_To_ExistCustomer()
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
            context.Orders.Remove(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            order.CheckDetached();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(0);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Remove_NewOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.CustomerID = 5; // this line for test UpdateRelated > previousForeign > Orphan
            context.Orders.Add(order);
            context.Orders.Remove(order);

            order.CheckDetached();

            context.Orders.CheckCount(0);

            context.Changes.CheckCount(0);
        }

        [TestMethod]
        public void Remove_ExistOrder_With_Ref_To_ExistCustomer()
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
            context.Orders.Remove(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            order.CheckDeleted();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void Remove_ExistOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = 5; // this line for test UpdateRelated > previousForeign > Orphan
            context.Orders.Add(order);
            context.Orders.Remove(order);

            order.CheckDeleted();

            context.Orders.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(order);
        }








        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_ExistOrder()
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

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_ExistOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = 11;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_ExistOrder_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = amir.ID;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_ExistOrder_With_Ref_To_Other_Added_ExistCustomer()
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

            Order order = new Order();
            order.ID = 505;
            order.Customer = amir;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_NewOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = 11;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_NewOrder_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Order order = new Order();
            order.CustomerID = amir.ID;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_Added_NewOrder_With_Ref_To_Other_Added_ExistCustomer()
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

            Order order = new Order();
            order.Customer = amir;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }

        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.ID = 505;

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckUnchanged();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_ExistOrder_With_SameKey()
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

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckUnchanged();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_ExistOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = 11;

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_ExistOrder_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = amir.ID;

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_ExistOrder_With_Ref_To_Other_Added_ExistCustomer()
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

            Order order = new Order();
            order.ID = 505;
            order.Customer = amir;

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_NewOrder_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = ali.ID; // Same Key As ali

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_NewOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = 11;

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_NewOrder_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Order order = new Order();
            order.CustomerID = amir.ID;

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Add_NotAdded_NewOrder_With_Ref_To_Other_Added_ExistCustomer()
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

            Order order = new Order();
            order.Customer = amir;

            ali.Orders.Add(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }


        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.ID = 505;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Added_ExistOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = 11;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Added_ExistOrder_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = amir.ID;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Added_ExistOrder_With_Ref_To_Other_Added_ExistCustomer()
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

            Order order = new Order();
            order.ID = 505;
            order.Customer = amir;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Added_NewOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = 11;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Added_NewOrder_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Order order = new Order();
            order.CustomerID = amir.ID;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_Added_NewOrder_With_Ref_To_Other_Added_ExistCustomer()
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

            Order order = new Order();
            order.Customer = amir;
            context.Orders.Add(order);

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }

        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_ExistOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = 11;

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_ExistOrder_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = amir.ID;

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_ExistOrder_With_Ref_To_Other_Added_ExistCustomer()
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

            Order order = new Order();
            order.ID = 505;
            order.Customer = amir;

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_NewOrder_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = ali.ID;

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_NewOrder_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = 11;

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_NewOrder_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Order order = new Order();
            order.CustomerID = amir.ID;

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewCustomer_EntityCollection_Add_NotAdded_NewOrder_With_Ref_To_Other_Added_ExistCustomer()
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

            Order order = new Order();
            order.Customer = amir;

            ali.Orders.Add(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }


        [TestMethod]
        public void ExistCustomer_EntityCollection_Remove_Added_ExistOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = 15;
            context.Orders.Add(order);

            ali.Orders.Remove(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(null);
            order.CustomerID.Check(null);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistCustomer_EntityCollection_Remove_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = 15;
            context.Orders.Add(order);

            ali.Orders.Remove(order);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(null);
            order.CustomerID.Check(null);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }


        [TestMethod]
        public void NewCustomer_EntityCollection_Remove_Added_NewOrder()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.CustomerID = ali.ID;
            context.Orders.Add(order);

            ali.Orders.Remove(order);

            ali.CheckAdded();
            ali.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(null);
            order.CustomerID.Check(null);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }








        [TestMethod]
        public void NewOrder_EditForeign_With_Added_ExistCustomer()
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

            Order order = new Order();
            order.Customer = ali;
            context.Orders.Add(order);

            order.Customer = amir;

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(amir);
            order.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_Added_ExistCustomerKey()
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

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_Added_NewCustomer()
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

            Order order = new Order();
            order.Customer = ali;
            context.Orders.Add(order);

            order.Customer = amir;

            ali.CheckAdded();
            ali.Orders.CheckCount(0);

            amir.CheckAdded();
            amir.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(amir);
            order.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(3).CheckFound(ali).CheckFound(amir).CheckFound(order);
        }
        [TestMethod]
        public void NewOrder_EditForeign_With_Added_NewCustomerKey()
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

            Order order = new Order();
            order.Customer = ali;
            context.Orders.Add(order);

            order.CustomerID = amir.ID;

            ali.CheckAdded();
            ali.Orders.CheckCount(0);

            amir.CheckAdded();
            amir.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(amir);
            order.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(3).CheckFound(ali).CheckFound(amir).CheckFound(order);
        }


        [TestMethod]
        public void NewOrder_EditForeign_To_Null()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.Customer = ali;
            context.Orders.Add(order);

            order.Customer = null;

            ali.CheckAdded();
            ali.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(null);
            order.CustomerID.Check(null);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }
        [TestMethod]
        public void NewOrder_EditForeign_To_NullKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Order order = new Order();
            order.Customer = ali;
            context.Orders.Add(order);

            order.CustomerID = null;

            ali.CheckAdded();
            ali.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(null);
            order.CustomerID.Check(null);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }

        [TestMethod]
        public void NewOrder_EditForeign_To_OrphanCustomer()
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

            Order order = new Order();
            order.Customer = ali;
            context.Orders.Add(order);

            order.CustomerID = amir.ID;

            ali.CheckAdded();
            ali.Orders.CheckCount(0);

            order.CheckAdded();
            order.Customer.Check(null);
            order.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(order);
        }


        [TestMethod]
        public void ExistOrder_EditForeign_With_Added_ExistCustomer()
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

            Order order = new Order();
            order.ID = 505;
            order.Customer = ali;
            context.Orders.Add(order);

            order.Customer = amir;

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(amir);
            order.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_Added_ExistCustomerKey()
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

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_Added_NewCustomer()
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

            Order order = new Order();
            order.ID = 505;
            order.Customer = ali;
            context.Orders.Add(order);

            order.Customer = amir;

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            amir.CheckAdded();
            amir.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(amir);
            order.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(amir).CheckFound(order);
        }
        [TestMethod]
        public void ExistOrder_EditForeign_With_Added_NewCustomerKey()
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

            Order order = new Order();
            order.ID = 505;
            order.Customer = ali;
            context.Orders.Add(order);

            order.CustomerID = amir.ID;

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            amir.CheckAdded();
            amir.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(amir);
            order.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(2).CheckFound(amir).CheckFound(order);
        }


        [TestMethod]
        public void ExistOrder_EditForeign_To_Null()
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

            order.Customer = null;

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(null);
            order.CustomerID.Check(null);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void ExistOrder_EditForeign_To_NullKey()
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

            order.CustomerID = null;

            ali.CheckUnchanged();
            ali.Orders.CheckCount(0);

            order.CheckModified();
            order.Customer.Check(null);
            order.CustomerID.Check(null);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }

        [TestMethod]
        public void ExistOrder_EditForeign_To_OrphanCustomer()
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

            order.CheckModified();
            order.Customer.Check(null);
            order.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }








        [TestMethod]
        public void Orphan1()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.CustomerID = 5;
            context.Orders.Add(order);

            Customer ali = new Customer();
            ali.ID = 5;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckAdded();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
        [TestMethod]
        public void Orphan2()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = 5;
            context.Orders.Add(order);

            Customer ali = new Customer();
            ali.ID = 5;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckUnchanged();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Orphan3()
        {
            EnterpriseContext context = new EnterpriseContext();

            Order order = new Order();
            order.ID = 505;
            order.CustomerID = 4;
            context.Orders.Add(order);
            order.CustomerID = 5;

            Customer ali = new Customer();
            ali.ID = 5;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.CheckUnchanged();
            ali.Orders.CheckCount(1).CheckItem(0, order);

            order.CheckModified();
            order.Customer.Check(ali);
            order.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            order.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Orders.CheckCount(1).CheckFound(order);

            context.Changes.CheckCount(1).CheckFound(order);
        }
    }
}
