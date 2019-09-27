using System.Reflection;

namespace Abi.Data
{
    internal sealed class EditedProperty<P> : EditedProperty
    {
        private EditedProperty() { }
        internal EditedProperty(PropertyInfo Property, P Current, P New)
        {
            this.Property = Property;
            this.Current = Current;
            this.New = New;
        }

        internal P Current { get; }
        internal P New { get; }
    }

    internal abstract class EditedProperty
    {
        private protected EditedProperty() { }

        internal PropertyInfo Property { get; private protected set; }
    }
}
