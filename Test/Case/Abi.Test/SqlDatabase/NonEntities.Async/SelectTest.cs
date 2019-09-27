using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abi.Data.Sql;
using NonEntity = Enterprise.Model.NonEntity;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.NonEntities.Async
{
    [TestClass]
    public class SelectddTest
    {
        [TestMethod]
        public void Select_ToArrayAsync_Prepared_WithParameters_NonEntity_Customers()
        {
            NonEntity.Customer[] customers;

            EntityDatabaseConnection connection = GetConnection();

            connection.Open();

            var t = Database.Connect(connection).Prepared()
                                                .WithParameters(new { CustomerID = 16 })
                                                .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                          from Customers c
                                                          where c.ID = @CustomerID

                                                          waitfor delay '00:00:01'")
                                                .ToArrayAsync<NonEntity.Customer>()
                                                .ContinueWith(task =>
                                                {
                                                    customers = task.Result;
                                                    connection.Close();
                                                });


            // do some work here, then wait for select command to be returned ...

            t.Wait();
        }

        [TestMethod]
        public void Select_FirstAsync_Prepared_WithParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            EntityDatabaseConnection connection = GetConnection();

            connection.Open();

            var t = Database.Connect(connection).Prepared()
                                                .WithParameters(new { CustomerID = 16 })
                                                .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                          from Customers c
                                                          where c.ID = @CustomerID

                                                          waitfor delay '00:00:01'")
                                                .FirstAsync<NonEntity.Customer>()
                                                .ContinueWith(task =>
                                                {
                                                    customer = task.Result;
                                                    connection.Close();
                                                });


            // do some work here, then wait for select command to be returned ...

            t.Wait();
        }

        [TestMethod]
        public void Select_FirstOrDefaultAsync_Prepared_WithParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            EntityDatabaseConnection connection = GetConnection();

            connection.Open();

            var t = Database.Connect(connection).Prepared()
                                                .WithParameters(new { CustomerID = 16 })
                                                .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                          from Customers c
                                                          where c.ID = @CustomerID

                                                          waitfor delay '00:00:01'")
                                                .FirstOrDefaultAsync<NonEntity.Customer>()
                                                .ContinueWith(task =>
                                                {
                                                    customer = task.Result;
                                                    connection.Close();
                                                });


            // do some work here, then wait for select command to be returned ...

            t.Wait();
        }

        [TestMethod]
        public void Select_SingleAsync_Prepared_WithParameters_NonEntity_CustomerFirstPartnerID()
        {
            int? customerFirstPartnerID;

            EntityDatabaseConnection connection = GetConnection();

            connection.Open();

            var t = Database.Connect(connection).Prepared()
                                                .WithParameters(new { CustomerID = 16 })
                                                .Select(@"select c.FirstPartnerID
                                                          from Customers c
                                                          where c.ID = @CustomerID

                                                          waitfor delay '00:00:01'")
                                                .SingleAsync<int?>()
                                                .ContinueWith(task =>
                                                {
                                                    customerFirstPartnerID = task.Result;
                                                    connection.Close();
                                                });

            // do some work here, then wait for select command to be returned ...

            t.Wait();
        }

        [TestMethod]
        public void Select_SingleOrDefaultAsync_Prepared_WithParameters_NonEntity_CustomerFirstPartnerID()
        {
            int? customerFirstPartnerID;

            EntityDatabaseConnection connection = GetConnection();

            connection.Open();

            var t = Database.Connect(connection).Prepared()
                                                .WithParameters(new { CustomerID = 16 })
                                                .Select(@"select c.FirstPartnerID
                                                          from Customers c
                                                          where c.ID = @CustomerID

                                                          waitfor delay '00:00:01'")
                                                .SingleOrDefaultAsync<int?>()
                                                .ContinueWith(task =>
                                                {
                                                    customerFirstPartnerID = task.Result;
                                                    connection.Close();
                                                });

            // do some work here, then wait for select command to be returned ...

            t.Wait();
        }
    }
}
