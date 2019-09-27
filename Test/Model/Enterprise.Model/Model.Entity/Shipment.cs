using Abi.Data;
using Abi.Types;

namespace Enterprise.Model
{
    [Table("Shipments")]
    public class Shipment : Entity<Shipment>
    {
        public Shipment()
        {
        }

        public long ID
        {
            get { return Get<long>(); }
            set { Set(value); }
        }
        public string PostalCode
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public string Telephone
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public string Cellphone
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public long? OrderID
        {
            get { return Get<long?>(); }
            set { Set(value); }
        }
        public Order Order
        {
            get { return Get<Order>(); }
            set { Set(value); }
        }
        public RowVersion RV
        {
            get { return Get<RowVersion>(); }
            set { Set(value); }
        }


        public override string ToString()
        {
            return $"Shipment [{ID}], For [Order: {(Order == null ? "Null" : $"{Order?.ID.ToString() ?? "Null"}")}]";
        }
    }
}
