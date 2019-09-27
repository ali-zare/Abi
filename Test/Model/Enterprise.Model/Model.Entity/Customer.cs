using Abi.Data;
using Abi.Types;

namespace Enterprise.Model
{
    [Table("Customers")]
    public class Customer : Entity<Customer>
    {
        public Customer()
        {
        }

        public int ID
        {
            get { return Get<int>(); }
            set { Set(value); }
        }
        public string FirstName
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public string LastName
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public int? FirstPartnerID
        {
            get { return Get<int?>(); }
            set { Set(value); }
        }
        public Customer FirstPartner
        {
            get { return Get<Customer>(); }
            set { Set(value); }
        }
        public int? SecondPartnerID
        {
            get { return Get<int?>(); }
            set { Set(value); }
        }
        public Customer SecondPartner
        {
            get { return Get<Customer>(); }
            set { Set(value); }
        }
        public string NationalIdentityNumber
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public RowVersion RV
        {
            get { return Get<RowVersion>(); }
            set { Set(value); }
        }


        public EntityUnique<Address> Address { get; set; }
        public EntityCollection<Order> Orders { get; set; }
        public EntityCollection<Customer> FirstPartners { get; set; }
        public EntityCollection<Customer> SecondPartners { get; set; }


        public override string ToString()
        {
            return $"Customer {ID}, {FirstName} {LastName}";
        }
    }
}
