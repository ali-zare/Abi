using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;
using NonEntity = Enterprise.Model.NonEntity;

namespace Abi.Test.SqlDatabase.NonEntities
{
    [TestClass]
    public class SelectTest
    {
        [TestMethod]
        public void Select_ToArray_Prepared_WithParameters_NonEntity_Customers()
        {
            NonEntity.Customer[] customers;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customers = Database.Connect(connection).Prepared()
                                                        .WithParameters(new { CustomerID = 16 })
                                                        .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                  from Customers c
                                                                  where c.ID = @CustomerID")
                                                        .ToArray<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_ToArray_Prepared_WithParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customers = Database.Connect(connection).Prepared()
                                                            .WithParameters(new { CustomerID = 16 })
                                                            .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                      from Customers c
                                                                      where c.ID = @CustomerID")
                                                            .ToArray(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }

        [TestMethod]
        public void Select_ToArray_Prepared_WithoutParameters_NonEntity_Customers()
        {
            NonEntity.Customer[] customers;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customers = Database.Connect(connection).Prepared()
                                                        .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                  from Customers c")
                                                        .ToArray<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_ToArray_Prepared_WithoutParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customers = Database.Connect(connection).Prepared()
                                                            .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                      from Customers c")
                                                            .ToArray(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }


        [TestMethod]
        public void Select_ToArray_Adhoc_WithParameters_NonEntity_Customers()
        {
            NonEntity.Customer[] customers;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customers = Database.Connect(connection).Adhoc()
                                                        .WithParameters(new { CustomerID = 16 })
                                                        .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                  from Customers c
                                                                  where c.ID = @CustomerID")
                                                        .ToArray<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_ToArray_Adhoc_WithParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customers = Database.Connect(connection).Adhoc()
                                                            .WithParameters(new { CustomerID = 16 })
                                                            .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                      from Customers c
                                                                      where c.ID = @CustomerID")
                                                            .ToArray(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }

        [TestMethod]
        public void Select_ToArray_Adhoc_WithoutParameters_NonEntity_Customers()
        {
            NonEntity.Customer[] customers;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customers = Database.Connect(connection).Adhoc()
                                                        .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                  from Customers c")
                                                        .ToArray<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_ToArray_Adhoc_WithoutParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customers = Database.Connect(connection).Adhoc()
                                                            .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                      from Customers c")
                                                            .ToArray(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }




        [TestMethod]
        public void Select_First_Prepared_WithParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customer = Database.Connect(connection).Prepared()
                                                       .WithParameters(new { CustomerID = 16 })
                                                       .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                 from Customers c
                                                                 where c.ID = @CustomerID")
                                                       .First<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_First_Prepared_WithParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customer = Database.Connect(connection).Prepared()
                                                           .WithParameters(new { CustomerID = 16 })
                                                           .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                     from Customers c
                                                                     where c.ID = @CustomerID")
                                                           .First(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }

        [TestMethod]
        public void Select_First_Prepared_WithoutParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customer = Database.Connect(connection).Prepared()
                                                       .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                 from Customers c")
                                                       .First<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_First_Prepared_WithoutParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customer = Database.Connect(connection).Prepared()
                                                           .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                     from Customers c")
                                                           .First(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }


        [TestMethod]
        public void Select_First_Adhoc_WithParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customer = Database.Connect(connection).Adhoc()
                                                       .WithParameters(new { CustomerID = 16 })
                                                       .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                 from Customers c
                                                                 where c.ID = @CustomerID")
                                                       .First<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_First_Adhoc_WithParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customer = Database.Connect(connection).Adhoc()
                                                           .WithParameters(new { CustomerID = 16 })
                                                           .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                     from Customers c
                                                                     where c.ID = @CustomerID")
                                                           .First(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }

        [TestMethod]
        public void Select_First_Adhoc_WithoutParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customer = Database.Connect(connection).Adhoc()
                                                       .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                 from Customers c")
                                                       .First<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_First_Adhoc_WithoutParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customer = Database.Connect(connection).Adhoc()
                                                           .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                     from Customers c")
                                                           .First(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }




        [TestMethod]
        public void Select_FirstOrDefault_Prepared_WithParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customer = Database.Connect(connection).Prepared()
                                                       .WithParameters(new { CustomerID = 16 })
                                                       .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                 from Customers c
                                                                 where c.ID = @CustomerID")
                                                       .FirstOrDefault<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_FirstOrDefault_Prepared_WithParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customer = Database.Connect(connection).Prepared()
                                                           .WithParameters(new { CustomerID = 16 })
                                                           .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                     from Customers c
                                                                     where c.ID = @CustomerID")
                                                           .FirstOrDefault(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }

        [TestMethod]
        public void Select_FirstOrDefault_Prepared_WithoutParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customer = Database.Connect(connection).Prepared()
                                                       .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                 from Customers c")
                                                       .FirstOrDefault<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_FirstOrDefault_Prepared_WithoutParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customer = Database.Connect(connection).Prepared()
                                                           .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                     from Customers c")
                                                           .FirstOrDefault(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }


        [TestMethod]
        public void Select_FirstOrDefault_Adhoc_WithParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customer = Database.Connect(connection).Adhoc()
                                                       .WithParameters(new { CustomerID = 16 })
                                                       .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                 from Customers c
                                                                 where c.ID = @CustomerID")
                                                       .FirstOrDefault<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_FirstOrDefault_Adhoc_WithParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customer = Database.Connect(connection).Adhoc()
                                                           .WithParameters(new { CustomerID = 16 })
                                                           .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                     from Customers c
                                                                     where c.ID = @CustomerID")
                                                           .FirstOrDefault(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }

        [TestMethod]
        public void Select_FirstOrDefault_Adhoc_WithoutParameters_NonEntity_Customers()
        {
            NonEntity.Customer customer;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customer = Database.Connect(connection).Adhoc()
                                                       .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                 from Customers c")
                                                       .FirstOrDefault<NonEntity.Customer>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_FirstOrDefault_Adhoc_WithoutParameters_NonEntity_Anonymous_Customers()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                var customer = Database.Connect(connection).Adhoc()
                                                           .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                                     from Customers c")
                                                           .FirstOrDefault(() => new { ID = default(int), RV = default(Types.RowVersion), FName = default(string), LName = default(string), NIN = default(string) });

                connection.Close();
            }
        }




        [TestMethod]
        public void Select_Single_Prepared_WithParameters_NonEntity_CustomerFirstPartnerID()
        {
            int? customerFirstPartnerID;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customerFirstPartnerID = Database.Connect(connection).Prepared()
                                                                .WithParameters(new { CustomerID = 16 })
                                                                .Select(@"select c.FirstPartnerID
                                                                          from Customers c
                                                                          where c.ID = @CustomerID")
                                                                .Single<int?>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Single_Prepared_WithParameters_NonEntity_CustomerNIN()
        {
            string customerNIN;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customerNIN = Database.Connect(connection).Prepared()
                                                          .WithParameters(new { CustomerID = 16 })
                                                          .Select(@"select c.NIN
                                                                    from Customers c
                                                                    where c.ID = @CustomerID")
                                                          .Single<string>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Single_Adhoc_WithParameters_NonEntity_OrderShippingDate()
        {
            DateTime orderShippingDate;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                orderShippingDate = Database.Connect(connection).Prepared()
                                                                .WithParameters(new { OrderID = 505L })
                                                                .Select(@"select o.ShippingDate
                                                                          from Orders o
                                                                          where o.ID = @OrderID")
                                                                .Single<DateTime>();

                orderShippingDate = Database.Connect(connection).Prepared()
                                                                .WithParameters(new { OrderID = 505L })
                                                                .Select(@"select o.ShippingDate
                                                                          from Orders o
                                                                          where o.ID = @OrderID")
                                                                .Single<DateTime>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Single_Adhoc_WithParameters_NonEntity_OrderRV()
        {
            Types.RowVersion orderRV;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                orderRV = Database.Connect(connection).Prepared()
                                                      .WithParameters(new { OrderID = 505L })
                                                      .Select(@"select o.RV
                                                                from Orders o
                                                                where o.ID = @OrderID")
                                                      .Single<Types.RowVersion>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Single_Adhoc_WithParameters_NonEntity_ProductImage()
        {
            byte[] productImage;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                productImage = Database.Connect(connection).Prepared()
                                                           .WithParameters(new { ProductID = 6005 })
                                                           .Select(@"select p.Image
                                                                     from Products p
                                                                     where p.ID = @ProductID")
                                                           .Single<byte[]>();

                connection.Close();
            }
        }

        [TestMethod]
        public void Select_SingleOrDefault_Prepared_WithParameters_NonEntity_CustomerFirstPartnerID()
        {
            int? customerFirstPartnerID;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customerFirstPartnerID = Database.Connect(connection).Prepared()
                                                                .WithParameters(new { CustomerID = 16 })
                                                                .Select(@"select c.FirstPartnerID
                                                                          from Customers c
                                                                          where c.ID = @CustomerID")
                                                                .SingleOrDefault<int?>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_SingleOrDefault_Prepared_WithParameters_NonEntity_CustomerNIN()
        {
            string customerNIN;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                customerNIN = Database.Connect(connection).Prepared()
                                                          .WithParameters(new { CustomerID = 16 })
                                                          .Select(@"select c.NIN
                                                                    from Customers c
                                                                    where c.ID = @CustomerID")
                                                          .SingleOrDefault<string>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_SingleOrDefault_Adhoc_WithParameters_NonEntity_OrderShippingDate()
        {
            DateTime orderShippingDate;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                orderShippingDate = Database.Connect(connection).Prepared()
                                                                .WithParameters(new { OrderID = 505L })
                                                                .Select(@"select o.ShippingDate
                                                                          from Orders o
                                                                          where o.ID = @OrderID")
                                                                .SingleOrDefault<DateTime>();

                orderShippingDate = Database.Connect(connection).Prepared()
                                                                .WithParameters(new { OrderID = 505L })
                                                                .Select(@"select o.ShippingDate
                                                                          from Orders o
                                                                          where o.ID = @OrderID")
                                                                .SingleOrDefault<DateTime>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_SingleOrDefault_Adhoc_WithParameters_NonEntity_OrderRV()
        {
            Types.RowVersion orderRV;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                orderRV = Database.Connect(connection).Prepared()
                                                      .WithParameters(new { OrderID = 505L })
                                                      .Select(@"select o.RV
                                                                from Orders o
                                                                where o.ID = @OrderID")
                                                      .SingleOrDefault<Types.RowVersion>();

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_SingleOrDefault_Adhoc_WithParameters_NonEntity_ProductImage()
        {
            byte[] productImage;

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                productImage = Database.Connect(connection).Prepared()
                                                           .WithParameters(new { ProductID = 6005 })
                                                           .Select(@"select p.Image
                                                                     from Products p
                                                                     where p.ID = @ProductID")
                                                           .SingleOrDefault<byte[]>();

                connection.Close();
            }
        }
    }
}
