using Abi.Data;
using Abi.Types;

namespace Enterprise.Model
{
    [Table("OrderDetails")]
    public class OrderDetail : Entity<OrderDetail>
    {
        public OrderDetail()
        {
        }

        public long ID
        {
            get { return Get<long>(); }
            set { Set(value); }
        }
        public long OrderID
        {
            get { return Get<long>(); }
            set { Set(value); }
        }
        public Order Order
        {
            get { return Get<Order>(); }
            set { Set(value); }
        }
        public short? ProductID
        {
            get { return Get<short?>(); }
            set { Set(value); }
        }
        public Product Product
        {
            get { return Get<Product>(); }
            set { Set(value); }
        }
        public double? Amount
        {
            get { return Get<double?>(); }
            set { Set(value); }
        }
        public long? Fee
        {
            get { return Get<long?>(); }
            set { Set(value); }
        }
        public RowVersion RV
        {
            get { return Get<RowVersion>(); }
            set { Set(value); }
        }


        public override string ToString()
        {
            return $"[OrderDetail ({ID}) Of [[{OrderID}] [{Order}]] {Product} : {Amount?.ToString() ?? "Null"} * {Fee?.ToString() ?? "Null"}]";
        }
    }
}
