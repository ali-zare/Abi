using System.Reflection;

namespace Abi.Data
{
    internal struct EditedEntity<TEntity, TProperty> where TEntity : Entity<TEntity>
    {
        internal EditedEntity(EditedProperty<TProperty> Value, TrakMode TrakMode = TrakMode.Track)
        {
            this.Value = Value;
            this.TrakMode = TrakMode;

            EntityRelationForeignManager = null;
        }

        internal EditedProperty<TProperty> Value { get; }
        internal TrakMode TrakMode { get; }

        internal IEntityRelationForeignManager<TEntity> EntityRelationForeignManager { get; set; }

        internal PropertyInfo Property => Value.Property;
    }
}
