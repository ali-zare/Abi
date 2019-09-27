namespace Abi.Data
{
    internal sealed class EntitySet64<TEntity> : EntitySet<TEntity> where TEntity : Entity<TEntity>
    {
        private EntitySet64() : base()
        {
        }
        internal EntitySet64(EntityContext Context) : base(Context)
        {
            EntityKeyManager = new EntityKeyManager64<TEntity>(Key);
        }
    }
}
