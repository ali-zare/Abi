using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Enterprise.Model;
using Abi.Data.Sql;
using Abi.Data;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.Entities
{
    [TestClass]
    public class InsertTest
    {
        [TestMethod]
        public void Insert_Customer_Without_CallBack()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];
                Customer sam = entities[2];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Insert(ali)
                                                    .Insert(amir)
                                                    .Insert(sam);

                connection.Close();
            }

            Data(Save);
        }
        [TestMethod]
        public void Insert_Customer_Without_CallBack_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];
                Customer sam = entities[2];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Insert(ali, amir, sam);

                connection.Close();
            }

            Data(Save);
        }

        [TestMethod]
        public void Insert_Customer_With_CallBack()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];
                Customer sam = entities[2];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Insert().WithCallBack(e => new { e.RV })
                                                       .Write(ali)
                                                       .Write(amir)
                                                       .Write(sam);

                connection.Close();
            }

            Data(Save);
        }
        [TestMethod]
        public void Insert_Customer_With_CallBack_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer ali = entities[0];
                Customer amir = entities[1];
                Customer sam = entities[2];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Insert().WithCallBack(e => new { e.RV })
                                                       .Write(ali, amir, sam);

                connection.Close();
            }

            Data(Save);
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

            Customer sam = new Customer();
            sam.FirstName = "Sam";
            sam.LastName = "Zare";
            context.Customers.Add(sam);

            Order order1 = new Order();
            order1.Customer = ali;
            context.Orders.Add(order1);

            Order order2 = new Order();
            order2.ID = 505;
            context.Orders.Add(order2);

            order2.Customer = ali;



            #region check

            ali.CheckAdded();
            ali.Orders.CheckCount(2).CheckItem(0, order1).CheckItem(1, order2);

            amir.CheckAdded();
            amir.Orders.CheckCount(0);

            sam.CheckAdded();
            sam.Orders.CheckCount(0);

            order1.CheckAdded();
            order1.Customer.Check(ali);
            order1.CustomerID.Check(ali.ID);

            order2.CheckModified();
            order2.Customer.Check(ali);
            order2.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            sam.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            #endregion

            context.Execute(save, ali, amir, sam);

            #region check

            ali.CheckUnchanged();
            ali.Orders.CheckCount(2).CheckItem(0, order1).CheckItem(1, order2);

            amir.CheckUnchanged();
            amir.Orders.CheckCount(0);

            sam.CheckUnchanged();
            sam.Orders.CheckCount(0);

            order1.CheckAdded();
            order1.Customer.Check(ali);
            order1.CustomerID.Check(ali.ID);

            order2.CheckModified();
            order2.Customer.Check(ali);
            order2.CustomerID.Check(ali.ID);

            ali.GetChangedProperties().CheckCount(0);
            amir.GetChangedProperties().CheckCount(0);
            sam.GetChangedProperties().CheckCount(0);
            order1.GetChangedProperties().CheckCount(0);
            order2.GetChangedProperties().CheckCount(1).CheckFound<Order>(o => o.CustomerID);

            #endregion check
        }
    }
}
