namespace Abi.Data
{
    internal sealed class EntitySet16<TEntity> : EntitySet<TEntity> where TEntity : Entity<TEntity>
    {
        private EntitySet16() : base()
        {
        }
        internal EntitySet16(EntityContext Context) : base(Context)
        {
            EntityKeyManager = new EntityKeyManager16<TEntity>(Key);
        }
    }
}
