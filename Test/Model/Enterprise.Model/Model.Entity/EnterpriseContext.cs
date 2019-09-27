using Abi.Data;

namespace Enterprise.Model
{
    public partial class EnterpriseContext : EntityContext<EnterpriseContext>
    {
        public EnterpriseContext()
        {
        }

        public EntitySet<Product> Products { get; set; }
        public EntitySet<Address> Addresses { get; set; }
        public EntitySet<Shipment> Shipments { get; set; }
        public EntitySet<Customer> Customers { get; set; }
        public EntitySet<Order> Orders { get; set; }
        public EntitySet<OrderDetail> OrderDetails { get; set; }
    }
}
