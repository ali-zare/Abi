using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Abi.Data.Sql;
using static Abi.Test.EnterpriseDatabase;

namespace Abi.Test.SqlDatabase.NonEntities
{
    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public void Query_Execute_Prepared_WithParameters_Insert_Customer_Update_Order()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                Database.Connect(connection).Prepared()
                                            .WithParameters(new { OrderID = 509, FName = "New-FirstName", LName = "New-LastName", NIN = "7777777777"})
                                            .Query(@"insert into Customers(FName, LName, NIN) 
                                                                   values (@FName, @LName, @NIN);
                                                    
                                                     update Orders
                                                        set CustomerID = scope_identity()
                                                     where ID = @OrderID;
                                                    ")
                                            .Execute();

                connection.CommitTransaction();
                connection.Close();
            }
        }

        [TestMethod]
        public void Query_Execute_Adhoc_WithParameters_Insert_Customer_Update_Order()
        {
            using (EntityDatabaseConnection connection = GetConnection())
            {
                connection.Open();
                connection.BeginTransaction(IsolationLevel.Snapshot);

                Database.Connect(connection).Adhoc()
                                            .WithParameters(new { OrderID = 509, FName = "New-FirstName", LName = "New-LastName", NIN = "7777777777" })
                                            .Query(@"insert into Customers(FName, LName, NIN) 
                                                                   values (@FName, @LName, @NIN);
                                                    
                                                     update Orders
                                                        set CustomerID = scope_identity()
                                                     where ID = @OrderID;
                                                    ")
                                            .Execute();

                connection.CommitTransaction();
                connection.Close();
            }
        }
    }
}
