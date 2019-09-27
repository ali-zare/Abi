using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Enterprise.Model;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.Entities
{
    [TestClass]
    public class UpdateTest
    {
        [TestMethod]
        public void Update_Customer_Without_CallBack()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Update(ali)
                                                    .Update(amir);

                connection.Close();
            }

            Data(Save);
        }
        [TestMethod]
        public void Update_Customer_Without_CallBack_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Update(ali, amir);

                connection.Close();
            }

            Data(Save);
        }

        [TestMethod]
        public void Update_Customer_With_CallBack()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency()
                                                       .Write(ali)
                                                       .Write(amir);

                connection.Close();
            }

            Data(Save);
        }
        [TestMethod]
        public void Update_Customer_With_CallBack_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Update().WithCallBack(e => new { e.RV }).WithoutCheckConcurrency()
                                                       .Write(ali, amir);

                connection.Close();
            }

            Data(Save);
        }

        [TestMethod]
        public void Update_Customer_With_CallBack_With_Check_Concurrency()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                       .Write(ali)
                                                       .Write(amir);

                connection.Close();
            }

            Data(Save);
        }
        [TestMethod]
        public void Update_Customer_With_CallBack_With_Check_Concurrency_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Update().WithCallBack(e => new { e.RV }).WithCheckConcurrency(e => new { e.RV })
                                                       .Write(ali, amir);

                connection.Close();
            }

            Data(Save);
        }

        [TestMethod]
        public void Update_Product_Without_CallBack()
        {
            EnterpriseContext context = new EnterpriseContext();

            Product laptop = new Product();
            laptop.ID = 6005;
            context.Products.Add(laptop);

            laptop.Image = new byte[] { 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f };

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Product>()
                                                 .Update(laptop);

                connection.Close();
            }
        }




        private void Data(Action<EnterpriseContext, Customer[]> save)
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

            Order order1 = new Order();
            order1.Customer = ali;
            context.Orders.Add(order1);

            Order order2 = new Order();
            context.Orders.Add(order2);


            #region insert ali, amir, order2

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();


                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>().Insert().WithCallBack(e => new { e.RV })
                                                 .Write(ali, amir);

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Order>().Insert().WithCallBack(e => new { e.RV })
                                                 .Write(order2);
                connection.Close();
            }

            #endregion insert ali, amir, order2 


            ali.FirstPartnerID = amir.ID;
            ali.NationalIdentityNumber = "0123456789";
            ali.FirstName = "@li";

            amir.NationalIdentityNumber = "9874563210";
            amir.FirstPartnerID = ali.ID;
            amir.LastName = "Z@rE";


            order2.Customer = ali;



            #region check

            ali.CheckModified();
            ali.Orders.CheckCount(2).CheckItem(0, order1).CheckItem(1, order2);

            amir.CheckModified();
            amir.Orders.CheckCount(0);

            order1.CheckAdded();
            order1.Customer.Check(ali);
            order1.CustomerID.Check(ali.ID);

            order2.CheckModified();
            order2.Customer.Check(ali);
            order2.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(3).CheckFound<Customer>(c => c.FirstName)
                                                    .CheckFound<Customer>(c => c.FirstPartnerID)
                                                    .CheckFound<Customer>(c => c.NationalIdentityNumber);
            amir.GetChangedProperties().CheckCount(3).CheckFound<Customer>(c => c.LastName)
                                       .CheckFound<Customer>(c => c.FirstPartnerID)
                                       .CheckFound<Customer>(c => c.NationalIdentityNumber);
            order1.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            #endregion check

            bool has_concurrency_conflict = false;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                try
                {
                    ali.TakeSnapshot();
                    amir.TakeSnapshot();

                    order1.TakeSnapshot();
                    order2.TakeSnapshot();

                    context.Execute(save, ali, amir);

                    ali.RemoveSnapshot();
                    amir.RemoveSnapshot();

                    order1.RemoveSnapshot();
                    order2.RemoveSnapshot();
                }
                catch (EntityDatabaseConcurrencyException)
                {
                    ali.RestoreSnapshot();
                    amir.RestoreSnapshot();

                    order1.RestoreSnapshot();
                    order2.RestoreSnapshot();

                    has_concurrency_conflict = true;
                }

                connection.Close();
            }

            #region check

            if (has_concurrency_conflict)
            {
                ali.CheckModified();
                ali.Orders.CheckCount(2).CheckItem(0, order1).CheckItem(1, order2);

                amir.CheckModified();
                amir.Orders.CheckCount(0);

                order1.CheckAdded();
                order1.Customer.Check(ali);
                order1.CustomerID.Check(ali.ID);

                order2.CheckModified();
                order2.Customer.Check(ali);
                order2.CustomerID.Check(ali.ID);

                ali.GetChangedProperties().CheckCount(3).CheckFound<Customer>(c => c.FirstName)
                                                        .CheckFound<Customer>(c => c.FirstPartnerID)
                                                        .CheckFound<Customer>(c => c.NationalIdentityNumber);
                amir.GetChangedProperties().CheckCount(3).CheckFound<Customer>(c => c.LastName)
                                           .CheckFound<Customer>(c => c.FirstPartnerID)
                                           .CheckFound<Customer>(c => c.NationalIdentityNumber);
                order1.GetChangedProperties().CheckCount(0);
                order2.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);
            }
            else
            {
                ali.CheckUnchanged();
                ali.Orders.CheckCount(2).CheckItem(0, order1).CheckItem(1, order2);

                amir.CheckUnchanged();
                amir.Orders.CheckCount(0);

                order1.CheckAdded();
                order1.Customer.Check(ali);
                order1.CustomerID.Check(ali.ID);

                order2.CheckModified();
                order2.Customer.Check(ali);
                order2.CustomerID.Check(ali.ID);

                ali.GetChangedProperties().CheckCount(0);
                order1.GetChangedProperties().CheckCount(0);
                order2.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);
            }

            #endregion check
        }
    }
}
