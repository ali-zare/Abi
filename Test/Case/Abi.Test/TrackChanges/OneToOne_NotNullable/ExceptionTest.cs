using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enterprise.Model;
using Abi.Data;

namespace Abi.Test.TrackChanges.OneToOne_NotNullable
{
    [TestClass]
    public class ExceptionTest
    {
        public TestContext TestContext { get; set; }




        [TestMethod]
        public void Add_NewAddress_Without_Customer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Address address = new Address();
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_Without_Customer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Address address = new Address();
                address.ID = 741952;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_Added_NewCustomer()
        {
            // In Real World, Database [Address] Record, Cannot Reference To New [Customer] That Is Not Exists In Database
            // Therefore First [Address] Must be Added, That Cause State Be Changed [Unchanged]
            // Then Can Change [Address] Customer/CustomerID Property, That Cause State Be Changed [Modified]
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_Added_NewCustomerKey()
        {
            // In Real World, Database [Address] Record, Cannot Reference To New [Customer] That Is Not Exists In Database
            // Therefore First [Address] Must be Added, That Cause State Be Changed [Unchanged]
            // Then Can Change [Address] Customer/CustomerID Property, That Cause State Be Changed [Modified]
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.ID = 741952;
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_NotAdded_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_NotAdded_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.ID = 741952;
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

                ali.CheckUnchanged();
                ali.Address.CheckItem(address);

                address.CheckUnchanged();
                address.Customer.Check(ali);
                address.CustomerID.Check(ali.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_ExistAddress_With_Ref_To_NotAdded_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_NotAdded_NewCustomerKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.ID = 741952;
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_NotAdded_NewCustomerKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = -5;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.ID = 741952;
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_Removed_ExistCustomer()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_Removed_ExistCustomerKey()
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

                Address address = new Address();
                address.ID = 741952;
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_Removed_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Add_ExistAddress_With_Ref_To_Removed_NewCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Address address = new Address();
                address.ID = 741952;
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

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
        public void Add_NewAddress_With_Ref_To_NotAdded_ExistCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Add_NewAddress_With_Ref_To_NotAdded_ExistCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

                ali.CheckUnchanged();
                ali.Address.CheckItem(address);

                address.CheckAdded();
                address.Customer.Check(ali);
                address.CustomerID.Check(ali.ID);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Add_NewAddress_With_Ref_To_NotAdded_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Add_NewAddress_With_Ref_To_NotAdded_NewCustomerKey_ZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

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
        public void Add_NewAddress_With_Ref_To_NotAdded_NewCustomerKey_NonZeroKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = -5;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";

                Address address = new Address();
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

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
        public void Add_NewAddress_With_Ref_To_Removed_ExistCustomer()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Add_NewAddress_With_Ref_To_Removed_ExistCustomerKey()
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

                Address address = new Address();
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

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
        public void Add_NewAddress_With_Ref_To_Removed_NewCustomer()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Add_NewAddress_With_Ref_To_Removed_NewCustomerKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Address address = new Address();
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

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
        public void Remove_NewCustomer_Related_To_NewAddress()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

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
        public void Remove_ExistCustomer_Related_To_NewAddress()
        {
            try
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
        public void Remove_ExistCustomer_Related_To_ExistAddress()
        {
            try
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
        public void ExistCustomer_EntityUnique_Add_Added_ExistAddress_With_SameKey()
        {
            try
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
                context.Addresses.Add(address);

                ali.Address.Set(address);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Added_NewAddress_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.CustomerID = ali.ID; // Same Key As ali
                context.Addresses.Add(address);

                ali.Address.Set(address);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Add_Removed_ExistAddress()
        {
            try
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
                context.Addresses.Remove(address);

                ali.Address.Set(address);

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
        public void ExistCustomer_EntityUnique_Remove_NotAdded_ExistAddress()
        {
            try
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

                ali.Address.Set(null);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Remove_NotAdded_NewAddress()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.CustomerID = ali.ID;

                ali.Address.Set(null);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistCustomer_EntityUnique_Remove_Added_ExistAddress()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.ID = 741952;
                address.CustomerID = 15;
                context.Addresses.Add(address);

                ali.Address.Set(null);

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
        public void ExistCustomer_EntityUnique_Remove_Added_NewAddress()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.ID = 15;
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.CustomerID = 15;
                context.Addresses.Add(address);

                ali.Address.Set(null);

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
        public void NewCustomer_EntityUnique_Add_Added_NewAddress_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.CustomerID = ali.ID; // Same Key As ali
                context.Addresses.Add(address);

                ali.Address.Set(address);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Add_NotAdded_ExistAddress()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.ID = 741952;

                ali.Address.Set(address);

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
        public void NewCustomer_EntityUnique_Add_NotAdded_ExistAddress_With_SameKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.ID = 741952;
                address.CustomerID = ali.ID; // Same Key As ali

                ali.Address.Set(address);

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
        public void NewCustomer_EntityUnique_Add_Removed_ExistAddress()
        {
            try
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
                context.Addresses.Remove(address);

                ali.Address.Set(address);

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
        public void NewCustomer_EntityUnique_Remove_NotAdded_ExistAddress()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.ID = 741952;
                address.CustomerID = ali.ID;

                ali.Address.Set(null);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Remove_NotAdded_NewAddress()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.CustomerID = ali.ID;

                ali.Address.Set(null);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewCustomer_EntityUnique_Remove_Added_NewAddress()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.CustomerID = ali.ID;
                context.Addresses.Add(address);

                ali.Address.Set(null);

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
        public void NewAddress_EditForeign_With_NotAdded_ExistCustomer()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_NotAdded_ExistCustomerKey()
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

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_NotAdded_NewCustomer_ZeroKey()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_NotAdded_NewCustomer_NonZeroKey()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_NotAdded_NewCustomerKey_ZeroKey()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_NotAdded_NewCustomerKey_NonZeroKey()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewAddress_EditForeign_With_Removed_ExistCustomer()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_Removed_ExistCustomerKey()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_Removed_NewCustomer()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_With_Removed_NewCustomerKey()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewAddress_EditForeign_To_UnknownCustomer_FromOtherContext_Without_SameKey()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_To_UnknownCustomer_FromOtherContext_With_SameKey()
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

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void NewAddress_EditForeign_To_Null()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = null;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void NewAddress_EditForeign_To_NullKey()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);

                Address address = new Address();
                address.Customer = ali;
                context.Addresses.Add(address);

                address.CustomerID = 0;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }


        [TestMethod]
        public void ExistAddress_EditForeign_With_NotAdded_ExistCustomer()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_NotAdded_ExistCustomerKey()
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

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(NotExpectedResultException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_NotAdded_NewCustomer_ZeroKey()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_NotAdded_NewCustomer_NonZeroKey()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_NotAdded_NewCustomerKey_ZeroKey()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_NotAdded_NewCustomerKey_NonZeroKey()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistAddress_EditForeign_With_Removed_ExistCustomer()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_Removed_ExistCustomerKey()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistAddress_EditForeign_With_Removed_NewCustomer()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistAddress_EditForeign_With_Removed_NewCustomerKey()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.CustomerID = amir.ID;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistAddress_EditForeign_To_UnknownCustomer_FromOtherContext_Without_SameKey()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistAddress_EditForeign_To_UnknownCustomer_FromOtherContext_With_SameKey()
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

                Address address = new Address();
                address.ID = 741952;
                address.Customer = ali;
                context.Addresses.Add(address);

                address.Customer = amir;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityKeyManagerException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void ExistAddress_EditForeign_To_Null()
        {
            try
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

                address.Customer = null;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void ExistAddress_EditForeign_To_NullKey()
        {
            try
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

                address.CustomerID = 0;

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntitySetCanEditException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }








        [TestMethod]
        public void Removed_ExistCustomer_EntityUnique_Add_NotAdded_ExistAddress()
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

                Address address = new Address();
                address.ID = 741952;

                ali.Address.Set(address);

                throw new Exception("Expected Exception, Was Not Thrown");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.GetType() == typeof(EntityUniqueSetException), e.Message);

                TestContext.WriteLine(e.Message);
            }
        }
        [TestMethod]
        public void Removed_NewCustomer_EntityUnique_Add_NotAdded_NewAddress()
        {
            try
            {
                EnterpriseContext context = new EnterpriseContext();

                Customer ali = new Customer();
                ali.FirstName = "Ali";
                ali.LastName = "Zare";
                context.Customers.Add(ali);
                context.Customers.Remove(ali);

                Address address = new Address();

                ali.Address.Set(address);

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
