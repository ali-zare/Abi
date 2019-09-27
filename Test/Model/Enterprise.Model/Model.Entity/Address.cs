using Abi.Data;
using Abi.Types;

namespace Enterprise.Model
{
    [Table("Addresses")]
    public class Address : Entity<Address>
    {
        public Address()
        {
        }

        public int ID
        {
            get { return Get<int>(); }
            set { Set(value); }
        }
        public string Line1
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public string Line2
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public int CustomerID
        {
            get { return Get<int>(); }
            set { Set(value); }
        }
        public Customer Customer
        {
            get { return Get<Customer>(); }
            set { Set(value); }
        }
        public RowVersion RV
        {
            get { return Get<RowVersion>(); }
            set { Set(value); }
        }


        public override string ToString()
        {
            return $"Address [{ID}], For [Customer: {(Customer == null ? "Null" : $"{Customer?.ID.ToString() ?? "Null"}, {Customer?.FirstName?.ToString() ?? "Null"} {Customer?.LastName?.ToString() ?? "Null"}")}] [CustomerID: {CustomerID.ToString() ?? "Null"}]";
        }
    }
}
