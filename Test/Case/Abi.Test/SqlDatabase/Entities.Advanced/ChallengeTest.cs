using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Data;
using System;
using Enterprise.Model;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.Entities.Advanced
{
    [TestClass]
    public class ChallengeTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Save_WithCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                (context, connection).WithCallBack().Save(false);

                connection.CommitTransaction();
                connection.Close();
            }

            Challenge(Save);
        }
        [TestMethod]
        public void Save_WithoutCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                (context, connection).WithoutCallBack().Save(false);

                connection.CommitTransaction();
                connection.Close();
            }

            Challenge(Save);
        }
        [TestMethod]
        public void Save_WithCheckConcurrency()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                (context, connection).WithCheckConcurrency().Save(false);

                connection.CommitTransaction();
                connection.Close();
            }

            Challenge(Save);
        }


        [TestMethod]
        public void SaveBatch_WithCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                (context, connection).WithCallBack().Save();

                connection.CommitTransaction();
                connection.Close();
            }

            Challenge(Save);
        }
        [TestMethod]
        public void SaveBatch_WithoutCallBack()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                (context, connection).WithoutCallBack().Save();

                connection.CommitTransaction();
                connection.Close();
            }

            Challenge(Save);
        }
        [TestMethod]
        public void SaveBatch_WithCheckConcurrency()
        {
            void Save(EnterpriseContext context)
            {
                EntityDatabaseConnection connection = GetConnection();
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                (context, connection).WithCheckConcurrency().Save();

                connection.CommitTransaction();
                connection.Close();
            }

            Challenge(Save);
        }




        [TestMethod]
        public void Many_Save_WithCallBack()
        {
            Challenge(Save_WithCallBack);
        }
        [TestMethod]
        public void Many_Save_WithoutCallBack()
        {
            Challenge(Save_WithoutCallBack);
        }
        [TestMethod]
        public void Many_Save_WithCheckConcurrency()
        {
            Challenge(Save_WithCheckConcurrency);
        }


        [TestMethod]
        public void Many_SaveBatch_WithCallBack()
        {
            Challenge(SaveBatch_WithCallBack);
        }
        [TestMethod]
        public void Many_SaveBatch_WithoutCallBack()
        {
            Challenge(SaveBatch_WithoutCallBack);
        }
        [TestMethod]
        public void Many_SaveBatch_WithCheckConcurrency()
        {
            Challenge(SaveBatch_WithCheckConcurrency);
        }




        private void Challenge(Action<EnterpriseContext> save)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Product[] products = new Product[100];
            Customer[] customers = new Customer[10];
            Order[] orders = new Order[customers.Length * 10];
            OrderDetail[] orderDetails = new OrderDetail[orders.Length * 10];

            sw.Stop();
            TestContext.WriteLine($"Entity Creation Time : {sw.ElapsedMilliseconds}");
            sw.Restart();

            for (int i = 0; i < products.Length; i++)
                products[i] = new Product { Name = $"Product-{i:D6}" };

            for (int i = 0; i < customers.Length; i++)
                customers[i] = new Customer { FirstName = $"Customer-{i:D6}", LastName = $"Customer-{i:D6}", NationalIdentityNumber = $"9999{i:D6}" };

            for (int i = 0; i < orders.Length; i++)
                orders[i] = new Order { Customer = customers[i % customers.Length] };

            for (int i = 0; i < orderDetails.Length; i++)
                orderDetails[i] = new OrderDetail { Order = orders[i % orders.Length], Product = products[i % products.Length] };

            sw.Stop();
            TestContext.WriteLine($"Entity Initialize Time : {sw.ElapsedMilliseconds}");
            sw.Restart();

            EnterpriseContext context = new EnterpriseContext();

            sw.Stop();
            TestContext.WriteLine($"Context Creation Time : {sw.ElapsedMilliseconds}");
            sw.Restart();

            for (int i = 0; i < products.Length; i++)
                context.Products.Add(products[i]);

            for (int i = 0; i < customers.Length; i++)
                context.Customers.Add(customers[i]);

            for (int i = 0; i < orders.Length; i++)
                context.Orders.Add(orders[i]);

            for (int i = 0; i < orderDetails.Length; i++)
                context.OrderDetails.Add(orderDetails[i]);

            sw.Stop();
            TestContext.WriteLine($"Add To Context Time : {sw.ElapsedMilliseconds}");
            sw.Restart();

            context.Execute(save);

            sw.Stop();
            TestContext.WriteLine($"Insert To Database Time : {sw.ElapsedMilliseconds}");
            sw.Restart();

            for (int i = 0; i < customers.Length; i++)
                customers[i].LastName = $"Modified-{customers[i].LastName}";

            for (int i = 0; i < orders.Length; i++)
                orders[i].Customer = customers[(i + customers.Length - 1) % (customers.Length - 1)];

            for (int i = 0; i < orderDetails.Length; i++)
                orderDetails[i].Product = products[(i + products.Length - 1) % (products.Length - 1)];

            sw.Stop();
            TestContext.WriteLine($"Modify Entity Time : {sw.ElapsedMilliseconds}");
            sw.Restart();

            context.Execute(save);

            sw.Stop();
            TestContext.WriteLine($"Update Database Time : {sw.ElapsedMilliseconds}");
            sw.Restart();

            for (int i = 0; i < orderDetails.Length; i++)
                context.OrderDetails.Remove(orderDetails[i]);

            for (int i = 0; i < orders.Length; i++)
                context.Orders.Remove(orders[i]);

            for (int i = 0; i < customers.Length; i++)
                context.Customers.Remove(customers[i]);

            for (int i = 0; i < products.Length; i++)
                context.Products.Remove(products[i]);

            sw.Stop();
            TestContext.WriteLine($"Remove Entity Time : {sw.ElapsedMilliseconds}");
            sw.Restart();

            context.Execute(save);

            sw.Stop();
            TestContext.WriteLine($"Delete From Database Time : {sw.ElapsedMilliseconds}");
        }
        private void Challenge(Action challenge)
        {
            for (int i = 0; i < count; i++)
            {
                TestContext.WriteLine($"Test {i + 1,2}");
                challenge();
            }
        }

        private const int count = 5;
    }
}
