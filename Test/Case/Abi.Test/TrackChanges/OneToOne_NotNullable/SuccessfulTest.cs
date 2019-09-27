using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprise.Model;

namespace Abi.Test.TrackChanges.OneToOne_NotNullable
{
    [TestClass]
    public class SuccessfulTest
    {
        [TestMethod]
        public void Add_NewAddress_With_Ref_To_Added_NewCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.Customer = ali;
            context.Addresses.Add(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void Add_NewAddress_With_Ref_To_Added_NewCustomerKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = ali.ID;
            context.Addresses.Add(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void Add_NewAddress_With_Ref_To_Added_ExistCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.Customer = ali;
            context.Addresses.Add(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void Add_NewAddress_With_Ref_To_Added_ExistCustomerKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = ali.ID;
            context.Addresses.Add(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }








        [TestMethod]
        public void Add_ExistAddress_With_Ref_To_Added_ExistCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.Customer = ali;
            context.Addresses.Add(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckUnchanged();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Add_ExistAddress_With_Ref_To_Added_ExistCustomerKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = ali.ID;
            context.Addresses.Add(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckUnchanged();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

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
        public void Remove_NewAddress_With_Ref_To_NewCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.Customer = ali;
            context.Addresses.Add(address);
            context.Addresses.Remove(address);

            ali.CheckAdded();
            ali.Address.CheckItem(null);

            address.CheckDetached();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(ali);
        }
        [TestMethod]
        public void Remove_NewAddress_With_Ref_To_ExistCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.Customer = ali;
            context.Addresses.Add(address);
            context.Addresses.Remove(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(null);

            address.CheckDetached();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(0);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Remove_NewAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Address address = new Address();
            address.CustomerID = 5; // this line for test UpdateRelated > previousForeign > Orphan
            context.Addresses.Add(address);
            context.Addresses.Remove(address);

            address.CheckDetached();

            context.Addresses.CheckCount(0);

            context.Changes.CheckCount(0);
        }

        [TestMethod]
        public void Remove_ExistAddress_With_Ref_To_ExistCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.Customer = ali;
            context.Addresses.Add(address);
            context.Addresses.Remove(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(null);

            address.CheckDeleted();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void Remove_ExistAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = 5; // this line for test UpdateRelated > previousForeign > Orphan
            context.Addresses.Add(address);
            context.Addresses.Remove(address);

            address.CheckDeleted();

            context.Addresses.CheckCount(0);

            context.Changes.CheckCount(1).CheckFound(address);
        }








        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Added_ExistAddress()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = 11;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Added_ExistAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = 11;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Added_ExistAddress_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = amir.ID;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Added_ExistAddress_With_Ref_To_Other_Added_ExistCustomer()
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

            Address address = new Address();
            address.ID = 741952;
            address.Customer = amir;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Added_NewAddress()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = 11;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Added_NewAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = 11;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Added_NewAddress_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.CustomerID = amir.ID;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Added_NewAddress_With_Ref_To_Other_Added_ExistCustomer()
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

            Address address = new Address();
            address.Customer = amir;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }

        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_ExistAddress()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckUnchanged();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_ExistAddress_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = ali.ID; // Same Key As ali

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckUnchanged();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_ExistAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = 11;

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_ExistAddress_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = amir.ID;

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_ExistAddress_With_Ref_To_Other_Added_ExistCustomer()
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

            Address address = new Address();
            address.ID = 741952;
            address.Customer = amir;

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_NewAddress()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_NewAddress_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = ali.ID; // Same Key As ali

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_NewAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.ID = 15;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = 11;

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_NewAddress_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.CustomerID = amir.ID;

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_NotAdded_NewAddress_With_Ref_To_Other_Added_ExistCustomer()
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

            Address address = new Address();
            address.Customer = amir;

            ali.Address.Set(address);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }


        [TestMethod]
        public void NewCustomer_EntityUnique_Add_Added_ExistAddress()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = 11;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_Added_ExistAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = 11;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_Added_ExistAddress_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = amir.ID;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_Added_ExistAddress_With_Ref_To_Other_Added_ExistCustomer()
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

            Address address = new Address();
            address.ID = 741952;
            address.Customer = amir;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_Added_NewAddress()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = 11;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_Added_NewAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = 11;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_Added_NewAddress_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.CustomerID = amir.ID;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_Added_NewAddress_With_Ref_To_Other_Added_ExistCustomer()
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

            Address address = new Address();
            address.Customer = amir;
            context.Addresses.Add(address);

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }

        [TestMethod]
        public void NewCustomer_EntityUnique_Add_NotAdded_ExistAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = 11;

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_NotAdded_ExistAddress_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = amir.ID;

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_NotAdded_ExistAddress_With_Ref_To_Other_Added_ExistCustomer()
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

            Address address = new Address();
            address.ID = 741952;
            address.Customer = amir;

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_NotAdded_NewAddress()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_NotAdded_NewAddress_With_SameKey()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = ali.ID;

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_NotAdded_NewAddress_With_Ref_To_OrphanCustomer()
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer ali = new Customer();
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            Address address = new Address();
            address.CustomerID = 11;

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_NotAdded_NewAddress_With_Ref_To_Other_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.CustomerID = amir.ID;

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_NotAdded_NewAddress_With_Ref_To_Other_Added_ExistCustomer()
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

            Address address = new Address();
            address.Customer = amir;

            ali.Address.Set(address);

            ali.CheckAdded();
            ali.Address.CheckItem(address);

            amir.CheckUnchanged();
            amir.Address.CheckItem(null);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }








        [TestMethod]
        public void NewAddress_EditForeign_With_Added_ExistCustomer()
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

            Address address = new Address();
            address.Customer = ali;
            context.Addresses.Add(address);

            address.Customer = amir;

            ali.CheckUnchanged();
            ali.Address.CheckItem(null);

            amir.CheckUnchanged();
            amir.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(amir);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.Customer = ali;
            context.Addresses.Add(address);

            address.CustomerID = amir.ID;

            ali.CheckUnchanged();
            ali.Address.CheckItem(null);

            amir.CheckUnchanged();
            amir.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(amir);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_Added_NewCustomer()
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

            Address address = new Address();
            address.Customer = ali;
            context.Addresses.Add(address);

            address.Customer = amir;

            ali.CheckAdded();
            ali.Address.CheckItem(null);

            amir.CheckAdded();
            amir.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(amir);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(3).CheckFound(ali).CheckFound(amir).CheckFound(address);
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_Added_NewCustomerKey()
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

            Address address = new Address();
            address.Customer = ali;
            context.Addresses.Add(address);

            address.CustomerID = amir.ID;

            ali.CheckAdded();
            ali.Address.CheckItem(null);

            amir.CheckAdded();
            amir.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(amir);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(3).CheckFound(ali).CheckFound(amir).CheckFound(address);
        }


        [TestMethod]
        public void NewAddress_EditForeign_To_OrphanCustomer()
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

            Address address = new Address();
            address.Customer = ali;
            context.Addresses.Add(address);

            address.CustomerID = amir.ID;

            ali.CheckAdded();
            ali.Address.CheckItem(null);

            address.CheckAdded();
            address.Customer.Check(null);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(ali).CheckFound(address);
        }


        [TestMethod]
        public void ExistAddress_EditForeign_With_Added_ExistCustomer()
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

            Address address = new Address();
            address.ID = 741952;
            address.Customer = ali;
            context.Addresses.Add(address);

            address.Customer = amir;

            ali.CheckUnchanged();
            ali.Address.CheckItem(null);

            amir.CheckUnchanged();
            amir.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(amir);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_Added_ExistCustomerKey()
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

            Address address = new Address();
            address.ID = 741952;
            address.Customer = ali;
            context.Addresses.Add(address);

            address.CustomerID = amir.ID;

            ali.CheckUnchanged();
            ali.Address.CheckItem(null);

            amir.CheckUnchanged();
            amir.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(amir);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_Added_NewCustomer()
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

            Address address = new Address();
            address.ID = 741952;
            address.Customer = ali;
            context.Addresses.Add(address);

            address.Customer = amir;

            ali.CheckUnchanged();
            ali.Address.CheckItem(null);

            amir.CheckAdded();
            amir.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(amir);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(amir).CheckFound(address);
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_Added_NewCustomerKey()
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

            Address address = new Address();
            address.ID = 741952;
            address.Customer = ali;
            context.Addresses.Add(address);

            address.CustomerID = amir.ID;

            ali.CheckUnchanged();
            ali.Address.CheckItem(null);

            amir.CheckAdded();
            amir.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(amir);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(2).CheckFound(ali).CheckFound(amir);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(2).CheckFound(amir).CheckFound(address);
        }


        [TestMethod]
        public void ExistAddress_EditForeign_To_OrphanCustomer()
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

            Address address = new Address();
            address.ID = 741952;
            address.Customer = ali;
            context.Addresses.Add(address);

            address.CustomerID = amir.ID;

            ali.CheckUnchanged();
            ali.Address.CheckItem(null);

            address.CheckModified();
            address.Customer.Check(null);
            address.CustomerID.Check(amir.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }








        [TestMethod]
        public void Orphan1()
        {
            EnterpriseContext context = new EnterpriseContext();

            Address address = new Address();
            address.CustomerID = 5;
            context.Addresses.Add(address);

            Customer ali = new Customer();
            ali.ID = 5;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckAdded();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }
        [TestMethod]
        public void Orphan2()
        {
            EnterpriseContext context = new EnterpriseContext();

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = 5;
            context.Addresses.Add(address);

            Customer ali = new Customer();
            ali.ID = 5;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckUnchanged();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(0);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(0);
        }
        [TestMethod]
        public void Orphan3()
        {
            EnterpriseContext context = new EnterpriseContext();

            Address address = new Address();
            address.ID = 741952;
            address.CustomerID = 4;
            context.Addresses.Add(address);
            address.CustomerID = 5;

            Customer ali = new Customer();
            ali.ID = 5;
            ali.FirstName = "Ali";
            ali.LastName = "Zare";
            context.Customers.Add(ali);

            ali.CheckUnchanged();
            ali.Address.CheckItem(address);

            address.CheckModified();
            address.Customer.Check(ali);
            address.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            address.GetChangedProperties().CheckCount(1).CheckFound<Address>(a => a.CustomerID);

            context.Customers.CheckCount(1).CheckFound(ali);
            context.Addresses.CheckCount(1).CheckFound(address);

            context.Changes.CheckCount(1).CheckFound(address);
        }

    }
}
