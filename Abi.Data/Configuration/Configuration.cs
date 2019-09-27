namespace Abi.Data
{
    public abstract class Configuration<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal Configuration(EntityContextConfiguration<TEntityContext> EntityContextConfiguration)
        {
            this.EntityContextConfiguration = EntityContextConfiguration;
        }

        internal EntityContextConfiguration<TEntityContext> EntityContextConfiguration { get; }
    }
}
