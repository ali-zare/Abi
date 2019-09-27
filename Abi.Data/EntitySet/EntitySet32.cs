namespace Abi.Data
{
    internal sealed class EntitySet32<TEntity> : EntitySet<TEntity> where TEntity : Entity<TEntity>
    {
        private EntitySet32() : base()
        {
        }
        internal EntitySet32(EntityContext Context) : base(Context)
        {
            EntityKeyManager = new EntityKeyManager32<TEntity>(Key);
        }
    }
}
