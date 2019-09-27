using System;

namespace Abi.Data.Sql
{
    public class EntitySavingEventArgs<TEntityContext> : SaveEventArgs<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        public EntitySavingEventArgs(EntityContext<TEntityContext> context, EntityDatabaseConnection connection, Entity entity) : base(context, connection)
        {
            Entity = entity;
        }

        public Entity Entity { get; }
    }








    public class EntitySavedEventArgs<TEntityContext> : SaveEventArgs<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        public EntitySavedEventArgs(EntityContext<TEntityContext> context, EntityDatabaseConnection connection, Entity entity, EntityState state) : base(context, connection)
        {
            State = state;
            Entity = entity;
        }

        public Entity Entity { get; }
        public EntityState State { get; }
    }








    public class SaveEventArgs<TEntityContext> : EventArgs where TEntityContext : EntityContext<TEntityContext>
    {
        public SaveEventArgs(EntityContext<TEntityContext> context, EntityDatabaseConnection connection)
        {
            Context = context;
            Connection = connection;
        }

        public EntityDatabaseConnection Connection { get; }
        public EntityContext<TEntityContext> Context { get; }
    }
}
