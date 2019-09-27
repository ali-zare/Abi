using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Enterprise.Model;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.Entities
{
    [TestClass]
    public class DeleteTest
    {
        [TestMethod]
        public void Delete_Customer()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer sam = entities[0];
                Customer mahan = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Delete(sam)
                                                    .Delete(mahan);

                connection.Close();
            }

            Data(Save);
        }
        [TestMethod]
        public void Delete_Customer_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer sam = entities[0];
                Customer mahan = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Delete(sam, mahan);

                connection.Close();
            }

            Data(Save);
        }

        [TestMethod]
        public void Delete_Customer_With_Check_Concurrency()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer sam = entities[0];
                Customer mahan = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Delete().WithCheckConcurrency(e => new { e.RV })
                                                       .Write(sam)
                                                       .Write(mahan);

                connection.Close();
            }

            Data(Save);
        }
        [TestMethod]
        public void Delete_Customer_With_Check_Concurrency_MultiRow()
        {
            void Save(EnterpriseContext context, params Customer[] entities)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();

                Customer sam = entities[0];
                Customer mahan = entities[1];

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                    .Delete().WithCheckConcurrency(e => new { e.RV })
                                                       .Write(sam, mahan);

                connection.Close();
            }

            Data(Save);
        }




        private void Data(Action<EnterpriseContext, Customer[]> save)
        {
            EnterpriseContext context = new EnterpriseContext();

            Customer sam = new Customer();
            sam.FirstName = "Sam";
            sam.LastName = "Zare";
            context.Customers.Add(sam);

            Customer mahan = new Customer();
            mahan.FirstName = "Mahan";
            mahan.LastName = "Esabat Tabari";
            context.Customers.Add(mahan);

            #region insert sam and mahan

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();


                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>().Insert().WithCallBack(e => new { e.RV })
                                                 .Write(sam, mahan);

                connection.Close();
            }

            #endregion insert sam and mahan

            context.Customers.Remove(sam);
            context.Customers.Remove(mahan);



            #region check

            sam.CheckDeleted();
            mahan.CheckDeleted();

            sam.GetChangedProperties().CheckCount(0);
            mahan.GetChangedProperties().CheckCount(0);

            #endregion check

            bool has_concurrency_conflict = false;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                try
                {
                    sam.TakeSnapshot();
                    mahan.TakeSnapshot();

                    context.Execute(save, sam, mahan);

                    sam.RemoveSnapshot();
                    mahan.RemoveSnapshot();
                }
                catch (EntityDatabaseConcurrencyException)
                {
                    sam.RestoreSnapshot();
                    mahan.RestoreSnapshot();

                    has_concurrency_conflict = true;
                }

                connection.Close();
            }

            #region check

            if (has_concurrency_conflict)
            {
                sam.CheckDeleted();
                mahan.CheckDeleted();
            }
            else
            {
                sam.CheckDetached();
                mahan.CheckDetached();

                sam.GetChangedProperties().CheckCount(0);
                mahan.GetChangedProperties().CheckCount(0);
            }

            #endregion check
        }
    }
}
