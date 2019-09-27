using System;

namespace Abi.Data
{
    public sealed class EntityContextConfiguration<TEntityContext> : EntityContextConfiguration where TEntityContext : EntityContext<TEntityContext>
    {
        static EntityContextConfiguration()
        {
            Default = new EntityContextConfiguration<TEntityContext>();
        }

        internal EntityContextConfiguration() : base(typeof(TEntityContext))
        {
            base.Entities = Entities = new ConfigurationEntity<TEntityContext>(this);
            base.EntityKeys = EntityKeys = new ConfigurationEntityKey<TEntityContext>(this);
            base.EntitySets = EntitySets = new ConfigurationEntitySet<TEntityContext>(this);
            base.EntityTrackings = EntityTrackings = new ConfigurationEntityTracking<TEntityContext>(this);
            base.EntityRelations = EntityRelations = new ConfigurationEntityRelation<TEntityContext>(this);
            base.EntityProperties = EntityProperties = new ConfigurationEntityProperty<TEntityContext>(this);
        }

        internal static EntityContextConfiguration<TEntityContext> Default { get; }


        public new ConfigurationEntity<TEntityContext> Entities { get; }
        public new ConfigurationEntityKey<TEntityContext> EntityKeys { get; }
        public new ConfigurationEntitySet<TEntityContext> EntitySets { get; }
        public new ConfigurationEntityTracking<TEntityContext> EntityTrackings { get; }
        public new ConfigurationEntityRelation<TEntityContext> EntityRelations { get; }
        public new ConfigurationEntityProperty<TEntityContext> EntityProperties { get; }



        public void Build()
        {
            if (CheckConfigured) throw new EntityContextConfigurationException("Already Configured");

            foreach (Type entityType in EntityRelations.Types())
                if (!EntitySets.Contains(entityType)) throw new EntityContextConfigurationException("EntitySet Not Found For EntityRelation");

            CheckConfigured = true;
        }



        public override string ToString()
        {
            return $"{typeof(TEntityContext).Name} EntityContextConfiguration, Is{(CheckConfigured ? " " : " Not ")}Configured";
        }
    }

    public abstract class EntityContextConfiguration
    {
        internal EntityContextConfiguration(Type EntityContextType)
        {
            this.EntityContextType = EntityContextType;
            CheckConfigured = false;
        }



        public static bool IsConfigured<TEntityContext>() where TEntityContext : EntityContext<TEntityContext>
        {
            return EntityContextConfiguration<TEntityContext>.Default.CheckConfigured;
        }
        public static EntityContextConfiguration<TEntityContext> Configure<TEntityContext>() where TEntityContext : EntityContext<TEntityContext>
        {
            EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration<TEntityContext>.Default;

            if (!configuration.CheckConfigured)
                return EntityContextConfiguration<TEntityContext>.Default;
            else
                throw new EntityContextConfigurationException($"{typeof(TEntityContext).Name} Already Configured");
        }
        public static EntityContextConfiguration<TEntityContext> GetConfiguration<TEntityContext>() where TEntityContext : EntityContext<TEntityContext>
        {
            EntityContextConfiguration<TEntityContext> configuration = EntityContextConfiguration<TEntityContext>.Default;

            if (configuration.CheckConfigured)
                return EntityContextConfiguration<TEntityContext>.Default;
            else
                throw new EntityContextConfigurationException($"{typeof(TEntityContext).Name} Is Not Configured");
        }



        internal bool CheckConfigured { get; private protected set; }
        public Type EntityContextType { get; }



        internal IConfigurationEntity Entities { get; private protected set; }
        internal IConfigurationEntityKey EntityKeys { get; private protected set; }
        internal IConfigurationEntitySet EntitySets { get; private protected set; }
        internal IConfigurationEntityTracking EntityTrackings { get; private protected set; }
        internal IConfigurationEntityRelation EntityRelations { get; private protected set; }
        internal IConfigurationEntityProperty EntityProperties { get; private protected set; }
    }
}

