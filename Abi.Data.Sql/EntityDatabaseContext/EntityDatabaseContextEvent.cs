using System;
using System.Collections.Generic;
using System.Text;

namespace Abi.Data.Sql
{
    public class EntityDatabaseContextEvent<TEntityContext> : EntityDatabaseContextAgent<TEntityContext>, IEntityDatabaseContext<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal EntityDatabaseContextEvent(EntityDatabaseContext<TEntityContext> Context) : base(Context) { }


        public EntityDatabaseContextBuilder<TEntityContext, TEntity> Table<TEntity>() where TEntity : Entity<TEntity>
        {
            return new EntityDatabaseContextBuilder<TEntityContext, TEntity>(context);
        }
        public EntityDatabaseContextEvent<TEntityContext> Set(Action<EntityDatabaseContextEventSetter<TEntityContext>> EventSetter)
        {
            EventSetter(new EntityDatabaseContextEventSetter<TEntityContext>(context));

            return this;
        }
    }








    public class EntityDatabaseContextEventSetter<TEntityContext> where TEntityContext : EntityContext<TEntityContext>
    {
        internal EntityDatabaseContextEventSetter(EntityDatabaseContext<TEntityContext> Context) => context = Context;

        private EntityDatabaseContext<TEntityContext> context;



        public event EntityDatabaseContextEntitySavingEventHandler<TEntityContext> EntitySaving
        {
            add => context.entity_saving += value;
            remove => context.entity_saving -= value;
        }



        public event EntityDatabaseContextEntitySavedEventHandler<TEntityContext> EntitySaved
        {
            add => context.entity_saved += value;
            remove => context.entity_saved -= value;
        }
    }








    public delegate void EntityDatabaseContextEntitySavingEventHandler<TEntityContext>(EntityDatabaseContext<TEntityContext> sender, EntitySavingEventArgs<TEntityContext> e) where TEntityContext : EntityContext<TEntityContext>;

    public delegate void EntityDatabaseContextEntitySavedEventHandler<TEntityContext>(EntityDatabaseContext<TEntityContext> sender, EntitySavedEventArgs<TEntityContext> e) where TEntityContext : EntityContext<TEntityContext>;

}
