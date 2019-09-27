using Abi.Data;

namespace Enterprise.Model
{
    public partial class EnterpriseContext
    {
        static EnterpriseContext()
        {
            EntityContextConfiguration<EnterpriseContext> config = EntityContextConfiguration.Configure<EnterpriseContext>();

            config.Entities.Add<Product>();
            config.Entities.Add<Address>();
            config.Entities.Add<Shipment>();
            config.Entities.Add<Customer>();
            config.Entities.Add<Order>();
            config.Entities.Add<OrderDetail>();

            config.EntityKeys.Add<Product>(e => e.ID);
            config.EntityKeys.Add<Address>(e => e.ID);
            config.EntityKeys.Add<Shipment>(e => e.ID);
            config.EntityKeys.Add<Customer>(e => e.ID);
            config.EntityKeys.Add<Order>(e => e.ID);
            config.EntityKeys.Add<OrderDetail>(e => e.ID);

            config.EntityTrackings.Except<Product>(e => e.RV);
            config.EntityTrackings.Except<Address>(e => e.RV);
            config.EntityTrackings.Except<Shipment>(e => e.RV);
            config.EntityTrackings.Except<Customer>(e => e.RV);
            config.EntityTrackings.Except<Order>(e => e.RV);
            config.EntityTrackings.Except<OrderDetail>(e => e.RV);

            config.EntityTrackings.Except<Product>(e => e.YearlyIncome);

            config.EntityRelations.OneToOne.Add<Customer, Address>(e => e.CustomerID, e => e.Customer, e => e.Address);
            config.EntityRelations.OneToMany.Add<Customer, Customer>(e => e.FirstPartnerID, e => e.FirstPartner, e => e.FirstPartners);
            config.EntityRelations.OneToMany.Add<Customer, Customer>(e => e.SecondPartnerID, e => e.SecondPartner, e => e.SecondPartners);
            config.EntityRelations.OneToMany.Add<Customer, Order>(e => e.CustomerID, e => e.Customer, e => e.Orders);
            config.EntityRelations.OneToOne.Add<Order, Shipment>(e => e.OrderID, e => e.Order, e => e.Shipment);
            config.EntityRelations.OneToMany.Add<Order, OrderDetail>(e => e.OrderID, e => e.Order, e => e.OrderDetails);
            config.EntityRelations.OneToMany.Add<Product, OrderDetail>(e => e.ProductID, e => e.Product);

            config.EntitySets.Add(ctx => ctx.Products);
            config.EntitySets.Add(ctx => ctx.Addresses);
            config.EntitySets.Add(ctx => ctx.Shipments);
            config.EntitySets.Add(ctx => ctx.Customers);
            config.EntitySets.Add(ctx => ctx.Orders);
            config.EntitySets.Add(ctx => ctx.OrderDetails);

            config.EntityProperties.Add<Customer>(e => e.FirstName, "FName");
            config.EntityProperties.Add<Customer>(e => e.LastName, "LName");
            config.EntityProperties.Add<Customer>(e => e.NationalIdentityNumber, "NIN");

            config.Build();
        }
    }
}
