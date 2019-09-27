using Abi.Data;
using Abi.Types;

namespace Enterprise.Model
{
    [Table("Products")]
    public class Product : Entity<Product>
    {
        public Product()
        {
        }

        public short ID
        {
            get { return Get<short>(); }
            set { Set(value); }
        }
        public string Name
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public string Manufacturer
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public float? Length
        {
            get { return Get<float?>(); }
            set { Set(value); }
        }
        public float? Width
        {
            get { return Get<float?>(); }
            set { Set(value); }
        }
        public float? Height
        {
            get { return Get<float?>(); }
            set { Set(value); }
        }
        public float? Weight
        {
            get { return Get<float?>(); }
            set { Set(value); }
        }
        public string Color
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public short? Power
        {
            get { return Get<short?>(); }
            set { Set(value); }
        }
        public string EnergyEfficiency
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public string Material
        {
            get { return Get<string>(); }
            set { Set(value); }
        }
        public short? Quality
        {
            get { return Get<short?>(); }
            set { Set(value); }
        }
        public short? Kind
        {
            get { return Get<short?>(); }
            set { Set(value); }
        }
        public byte[] Image
        {
            get { return Get<byte[]>(); }
            set { Set(value); }
        }
        public decimal YearlyIncome
        {
            get { return Get<decimal>(); }
            set { Set(value); }
        }
        public RowVersion RV
        {
            get { return Get<RowVersion>(); }
            set { Set(value); }
        }
        [Ignore] public string Note { get; set; }


        public override string ToString()
        {
            return $"[Product ({ID}) {Name}]";
        }
    }
}
