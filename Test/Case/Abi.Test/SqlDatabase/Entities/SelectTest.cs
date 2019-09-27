using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Enterprise.Model;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.Entities
{
    [TestClass]
    public class SelectTest
    {
        [TestMethod]
        public void Select_Prepared_WithParameters_WithCallBack_Fill_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Prepared()
                                                 .WithParameters(new { CustomerID = 16 })
                                                 .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                           from Customers c
                                                           where c.ID = @CustomerID")
                                                 .WithCallBack(e => new { e.ID, e.FirstName, e.LastName, e.NationalIdentityNumber, e.RV })
                                                 .Fill(context.Customers);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Prepared_WithParameters_WithoutCallBack_Fill_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Prepared()
                                                 .WithParameters(new { CustomerID = 16 })
                                                 .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                           from Customers c
                                                           where c.ID = @CustomerID")
                                                 .Fill(context.Customers);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Prepared_WithoutParameters_WithCallBack_Fill_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Prepared()
                                                 .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                           from Customers c")
                                                 .WithCallBack(e => new { e.ID, e.FirstName, e.LastName, e.NationalIdentityNumber, e.RV })
                                                 .Fill(context.Customers);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Prepared_WithoutParameters_WithoutCallBack_Fill_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Prepared()
                                                 .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                           from Customers c")
                                                 .Fill(context.Customers);

                connection.Close();
            }
        }

        [TestMethod]
        public void Select_Adhoc_WithParameters_WithCallBack_Fill_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Adhoc()
                                                 .WithParameters(new { CustomerID = 16 })
                                                 .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                           from Customers c
                                                           where ID = @CustomerID")
                                                 .WithCallBack(e => new { e.ID, e.FirstName, e.LastName, e.NationalIdentityNumber, e.RV })
                                                 .Fill(context.Customers);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Adhoc_WithParameters_WithoutCallBack_Fill_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Adhoc()
                                                 .WithParameters(new { CustomerID = 16 })
                                                 .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                           from Customers c
                                                           where ID = @CustomerID")
                                                 .Fill(context.Customers);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Adhoc_WithoutParameters_WithCallBack_Fill_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Adhoc()
                                                 .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                           from Customers c")
                                                 .WithCallBack(e => new { e.ID, e.FirstName, e.LastName, e.NationalIdentityNumber, e.RV })
                                                 .Fill(context.Customers);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_Adhoc_WithoutParameters_WithoutCallBack_Fill_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Adhoc()
                                                 .Select(@"select 'Extra' as ExtraColumn, c.*, 'Extra' as ExtraColumn 
                                                           from Customers c")
                                                 .Fill(context.Customers);

                connection.Close();
            }
        }


        [TestMethod]
        public void Select_WithoutCallBack_Fill_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select 'Extra' as ExtraColumn, *, 'Extra' as ExtraColumn from Customers")
                                                 .Fill(context.Customers);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_WithCallBack_Fill_Customers_FiveTime()
        {
            EnterpriseContext context1 = new EnterpriseContext();
            EnterpriseContext context2 = new EnterpriseContext();
            EnterpriseContext context3 = new EnterpriseContext();
            EnterpriseContext context4 = new EnterpriseContext();
            EnterpriseContext context5 = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select * from Customers")
                                                 .WithCallBack(e => new { e.ID, e.FirstName, e.LastName, e.NationalIdentityNumber, e.RV }) // Same As context4
                                                 .Fill(context1.Customers);

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select * from Customers")
                                                 .WithCallBack(e => new { e.ID, e.FirstName, e.LastName, e.RV }) // Same As context5
                                                 .Fill(context2.Customers);

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select * from Customers")
                                                 .WithCallBack(e => new { e.ID, e.RV, e.FirstName, e.LastName, e.NationalIdentityNumber })
                                                 .Fill(context3.Customers);

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select * from Customers")
                                                 .WithCallBack(e => new { e.ID, e.FirstName, e.LastName, e.NationalIdentityNumber, e.RV }) // Same As context1
                                                 .Fill(context4.Customers);

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select * from Customers")
                                                 .WithCallBack(e => new { e.ID, e.FirstName, e.LastName, e.RV }) // Same As context2
                                                 .Fill(context5.Customers);

                connection.Close();
            }
        }


        [TestMethod]
        public void Select_WithCallBack_Merge__With_Key_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select 'Extra' as ExtraColumn, *, 'Extra' as ExtraColumn from Customers")
                                                 .WithCallBack(e => new { e.ID, e.RV, e.LastName, e.FirstName })
                                                 .Merge(context.Customers);

                connection.Close();
            }

            context.Customers[0].NationalIdentityNumber = "9999999999";

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select 'Extra' as ExtraColumn, *, 'Extra' as ExtraColumn from Customers")
                                                 .WithCallBack(e => new { e.ID, e.NationalIdentityNumber })
                                                 .Merge(context.Customers);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_WithCallBack_Merge_Without_Key_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select 'Extra' as ExtraColumn, *, 'Extra' as ExtraColumn from Customers")
                                                 .WithCallBack(e => new { e.RV, e.LastName, e.FirstName })
                                                 .Merge(context.Customers);

                connection.Close();
            }
        }


        [TestMethod]
        public void Select_WithCallBack_Refresh__With_Key_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select 'Extra' as ExtraColumn, *, 'Extra' as ExtraColumn from Customers")
                                                 .WithCallBack(e => new { e.ID, e.RV, e.LastName, e.FirstName })
                                                 .Refresh(context.Customers);

                connection.Close();
            }

            context.Customers[0].NationalIdentityNumber = "9999999999";

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select 'Extra' as ExtraColumn, *, 'Extra' as ExtraColumn from Customers")
                                                 .WithCallBack(e => new { e.ID, e.NationalIdentityNumber })
                                                 .Refresh(context.Customers);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_WithCallBack_Refresh_Without_Key_Customers()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Customer>()
                                                 .Select("select 'Extra' as ExtraColumn, *, 'Extra' as ExtraColumn from Customers")
                                                 .WithCallBack(e => new { e.RV, e.LastName, e.FirstName })
                                                 .Refresh(context.Customers);

                connection.Close();
            }
        }




        [TestMethod]
        public void Select_WithCallBack_Orders()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Order>()
                                                 .Select("select * from Orders")
                                                 .WithCallBack(e => new { e.ID, e.CustomerID, e.ReceiveDate, e.ShippingDate, e.RV })
                                                 .Fill(context.Orders);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_WithCallBack_OrderDetails()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<OrderDetail>()
                                                 .Select("select * from OrderDetails")
                                                 .WithCallBack(e => new { e.ID, e.RV, e.Amount, e.Fee, e.OrderID }) // OrderID is not null column, therefore it cann't be omitted in query, and must be loaded in model
                                                 .Fill(context.OrderDetails);

                connection.Close();
            }
        }
        [TestMethod]
        public void Select_WithCallBack_Products()
        {
            EnterpriseContext context = new EnterpriseContext();

            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();

                EntityDatabase<EnterpriseContext>.Connect(connection)
                                                 .Table<Product>()
                                                 .Select("select * from Products")
                                                 .WithCallBack(e => new { e.ID, e.Name, e.Manufacturer, e.Length, e.Width, e.Height, e.Weight, e.Color, e.Power, e.EnergyEfficiency, e.Material, e.Quality, e.Kind, e.Image, e.RV })
                                                 .Fill(context.Products);

                connection.Close();
            }
        }
    }
}
