using Abi.Data;
using Abi.Types;

namespace Enterprise.Model.NonEntity
{
    [Table("Customers")]
    public class Customer
    {
        public Customer()
        {
        }

        public int ID { get; set; }
        [Column(Name = "FName")]
        public string FirstName { get; set; }
        [Column(Name = "LName")]
        public string LastName { get; set; }
        public int? PartnerID { get; set; }
        [Column(Name = "NIN")]
        public string NationalIdentityNumber { get; set; }
        public RowVersion RV { get; set; }


        public override string ToString()
        {
            return $"Customer {ID}, {FirstName} {LastName}";
        }
    }
}
