using System;
using Abi.Data;
using Abi.Types;

namespace Enterprise.Model
{
    [Table("Orders")]
    public class Order : Entity<Order>
    {
        public Order()
        {
        }

        public long ID
        {
            get { return Get<long>(); }
            set { Set(value); }
        }
        public int? CustomerID
        {
            get { return Get<int?>(); }
            set { Set(value); }
        }
        public Customer Customer
        {
            get { return Get<Customer>(); }
            set { Set(value); }
        }
        public DateTime? ReceiveDate
        {
            get { return Get<DateTime?>(); }
            set { Set(value); }
        }
        public DateTime? ShippingDate
        {
            get { return Get<DateTime?>(); }
            set { Set(value); }
        }
        public RowVersion RV
        {
            get { return Get<RowVersion>(); }
            set { Set(value); }
        }


        public EntityUnique<Shipment> Shipment { get; set; }
        public EntityCollection<OrderDetail> OrderDetails { get; set; }


        public override string ToString()
        {
            return $"Order [{ID}], By [Customer: {(Customer == null ? "Null" : $"{Customer?.ID.ToString() ?? "Null"}, {Customer?.FirstName?.ToString() ?? "Null"} {Customer?.LastName?.ToString() ?? "Null"}")}] [CustomerID: {CustomerID?.ToString() ?? "Null"}]";
        }
    }
}
