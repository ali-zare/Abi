namespace Abi.Data.Sql
{
    public class EntityDatabaseContextAgent<TEntityContext> : IEntityDatabaseContext<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal EntityDatabaseContextAgent(EntityDatabaseContext<TEntityContext> Context) => context = Context;

        private protected EntityDatabaseContext<TEntityContext> context;


        public void Save(bool Batch = true, bool Snapshot = true)
        {
            context.Save(Batch, Snapshot);
        }
    }
}
