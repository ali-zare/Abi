using System;
using System.Linq;
using System.Reflection;

namespace Abi.Data.Sql
{
    internal sealed class CommandParameterModifiedProperties<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal CommandParameterModifiedProperties(TEntity Entity)
        {
            entity = Entity;
        }

        private TEntity entity;

        internal ParameterDefinition[] Definitions
        {
            get
            {
                return entity.GetChangedProperties().GetParameterDefinition<TEntityContext>().ToArray();
            }
        }
    }

    internal sealed class CommandParameterModify<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        internal CommandParameterModify(TEntity Entity)
        {
            entity = Entity;
        }

        private TEntity entity;

        internal CommandParameterModifiedProperties<TEntityContext, TEntity> ModifiedProperties
        {
            get
            {
                return new CommandParameterModifiedProperties<TEntityContext, TEntity>(entity);
            }
        }
    }








    internal sealed class CommandParameterAllDerivativeProperties<TEntityContext, TEntity, TEntityDerivative> : CommandParameter<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static CommandParameterAllDerivativeProperties()
        {
            Default = new CommandParameterAllDerivativeProperties<TEntityContext, TEntity, TEntityDerivative>();
        }
        private CommandParameterAllDerivativeProperties()
        {
        }

        private static ParameterDefinition[] all_derivative_properties_params;

        internal ParameterDefinition[] Definitions
        {
            get
            {
                if (all_derivative_properties_params == null)
                {
                    Type typeEntity = typeof(TEntity);
                    Type typeEntityDerivative = typeof(TEntityDerivative);

                    PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);
                    PropertyInfo[] entityAcceptableDataProperties = Entity<TEntity>.AcceptableDataProperties;

                    PropertyInfo[] entityDerivativeProperties = typeEntityDerivative.GetProperties();
                    PropertyInfo[] entityDerivativeAcceptableDataProperties = entityDerivativeProperties.Map(entityAcceptableDataProperties).ToArray();

                    if (entityDerivativeProperties.Length != entityDerivativeAcceptableDataProperties.Length)
                        foreach (PropertyInfo property in entityDerivativeProperties)
                            if (!entityDerivativeAcceptableDataProperties.Any(p => p.Name == property.Name))
                                throw new EntityDatabaseException($"Derive {typeof(TEntity).Name}.{property.Name} Property Failed, Derived Property Must Be Acceptable Data Property");

                    all_derivative_properties_params = entityDerivativeAcceptableDataProperties.GetParameterDefinition<TEntityContext>().ToArray();
                }

                return all_derivative_properties_params;
            }
        }

        internal static CommandParameterAllDerivativeProperties<TEntityContext, TEntity, TEntityDerivative> Default { get; }
    }

    internal sealed class CommandParameterDerivativeProperties<TEntityContext, TEntity, TEntityDerivative> : CommandParameter<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static CommandParameterDerivativeProperties()
        {
            Default = new CommandParameterDerivativeProperties<TEntityContext, TEntity, TEntityDerivative>();
        }
        private CommandParameterDerivativeProperties()
        {
        }

        private static ParameterDefinition[] derivative_properties_params;

        internal ParameterDefinition[] Definitions
        {
            get
            {
                if (derivative_properties_params == null)
                {
                    Type typeEntity = typeof(TEntity);
                    Type typeEntityDerivative = typeof(TEntityDerivative);

                    PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);
                    PropertyInfo[] entityAcceptableDataProperties = Entity<TEntity>.AcceptableDataProperties.Where(p => p != key).ToArray();

                    PropertyInfo[] entityDerivativeProperties = typeEntityDerivative.GetProperties();
                    PropertyInfo[] entityDerivativeAcceptableDataProperties = entityDerivativeProperties.Map(entityAcceptableDataProperties).ToArray();

                    if (entityDerivativeProperties.Length != entityDerivativeAcceptableDataProperties.Length)
                        foreach (PropertyInfo property in entityDerivativeProperties)
                            if (!entityDerivativeAcceptableDataProperties.Any(p => p.Name == property.Name))
                                throw new EntityDatabaseException($"Derive {typeof(TEntity).Name}.{property.Name} Property Failed, Derived Property Must Be Acceptable Data Property, Except Key Property");

                    derivative_properties_params = entityDerivativeAcceptableDataProperties.GetParameterDefinition<TEntityContext>().ToArray();
                }

                return derivative_properties_params;
            }
        }

        internal static CommandParameterDerivativeProperties<TEntityContext, TEntity, TEntityDerivative> Default { get; }
    }

    internal sealed class CommandParameterDerive<TEntityContext, TEntity, TEntityDerivative> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static CommandParameterDerive()
        {
            Default = new CommandParameterDerive<TEntityContext, TEntity, TEntityDerivative>();
        }
        private CommandParameterDerive()
        {
        }

        internal CommandParameterDerivativeProperties<TEntityContext, TEntity, TEntityDerivative> DerivativeProperties
        {
            get
            {
                return CommandParameterDerivativeProperties<TEntityContext, TEntity, TEntityDerivative>.Default;
            }
        }
        internal CommandParameterAllDerivativeProperties<TEntityContext, TEntity, TEntityDerivative> AllDerivativeProperties
        {
            get
            {
                return CommandParameterAllDerivativeProperties<TEntityContext, TEntity, TEntityDerivative>.Default;
            }
        }

        internal static CommandParameterDerive<TEntityContext, TEntity, TEntityDerivative> Default { get; }
    }








    internal sealed class CommandParameterKeyProperty<TEntityContext, TEntity> : CommandParameter<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static CommandParameterKeyProperty()
        {
            Default = new CommandParameterKeyProperty<TEntityContext, TEntity>();
        }
        private CommandParameterKeyProperty()
        {
        }

        private static ParameterDefinition? key_property_param;

        internal ParameterDefinition Definition
        {
            get
            {
                if (key_property_param == null)
                {
                    Type typeEntity = typeof(TEntity);

                    PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);

                    key_property_param = key.GetParameterDefinition<TEntityContext>();
                }

                return key_property_param.Value;
            }
        }

        internal static CommandParameterKeyProperty<TEntityContext, TEntity> Default { get; }
    }








    internal sealed class CommandParameterTrackableProperties<TEntityContext, TEntity> : CommandParameter<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static CommandParameterTrackableProperties()
        {
            Default = new CommandParameterTrackableProperties<TEntityContext, TEntity>();
        }
        private CommandParameterTrackableProperties()
        {
        }

        private static ParameterDefinition[] trackable_properties_params;

        internal ParameterDefinition[] Definitions
        {
            get
            {
                if (trackable_properties_params == null)
                {
                    Type typeEntity = typeof(TEntity);

                    PropertyInfo key = configuration.EntityKeys.GetPropEntityKey(typeEntity);

                    PropertyInfo[] trackableProperties = Entity<TEntity>.AcceptableDataProperties
                                                                                .Except(configuration.EntityTrackings.GetExceptedProperties(typeEntity))
                                                                                .Where(p => p != key).ToArray();

                    trackable_properties_params = trackableProperties.GetParameterDefinition<TEntityContext>().ToArray();
                }

                return trackable_properties_params;
            }
        }

        internal static CommandParameterTrackableProperties<TEntityContext, TEntity> Default { get; }
    }








    internal sealed class CommandParameterAcceptableProperties<TEntityContext, TEntity> : CommandParameter<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static CommandParameterAcceptableProperties()
        {
            Default = new CommandParameterAcceptableProperties<TEntityContext, TEntity>();
        }
        private CommandParameterAcceptableProperties()
        {
        }

        private static ParameterDefinition[] acceptable_properties_params;

        internal ParameterDefinition[] Definitions
        {
            get
            {
                if (acceptable_properties_params == null)
                {
                    PropertyInfo[] acceptableProperties = Entity<TEntity>.AcceptableDataProperties;

                    acceptable_properties_params = acceptableProperties.GetParameterDefinition<TEntityContext>().ToArray();
                }

                return acceptable_properties_params;
            }
        }

        internal static CommandParameterAcceptableProperties<TEntityContext, TEntity> Default { get; }
    }








    internal abstract class CommandParameter<TEntityContext, TEntity> where TEntityContext : EntityContext<TEntityContext> where TEntity : Entity<TEntity>
    {
        static CommandParameter()
        {
            configuration = EntityContextConfiguration.GetConfiguration<TEntityContext>();

            KeyProperty = CommandParameterKeyProperty<TEntityContext, TEntity>.Default;
            TrackableProperties = CommandParameterTrackableProperties<TEntityContext, TEntity>.Default;
            AcceptableProperties = CommandParameterAcceptableProperties<TEntityContext, TEntity>.Default;
        }
        private protected CommandParameter()
        {
        }

        private protected static EntityContextConfiguration<TEntityContext> configuration;

        internal static CommandParameterKeyProperty<TEntityContext, TEntity> KeyProperty { get; }
        internal static CommandParameterTrackableProperties<TEntityContext, TEntity> TrackableProperties { get; }
        internal static CommandParameterAcceptableProperties<TEntityContext, TEntity> AcceptableProperties { get; }

        internal static CommandParameterModify<TEntityContext, TEntity> Modified(TEntity Entity)
        {
            return new CommandParameterModify<TEntityContext, TEntity>(Entity);
        }
        internal static CommandParameterDerive<TEntityContext, TEntity, TEntityDerivative> Derive<TEntityDerivative>()
        {
            return CommandParameterDerive<TEntityContext, TEntity, TEntityDerivative>.Default;
        }
    }








    internal sealed class CommandParameterVariableProperties<T>
    {
        static CommandParameterVariableProperties()
        {
            Default = new CommandParameterVariableProperties<T>();
        }
        private CommandParameterVariableProperties()
        {
        }

        private static ParameterDefinition[] variable_properties_params;

        internal ParameterDefinition[] Definitions
        {
            get
            {
                if (variable_properties_params == null)
                {
                    PropertyInfo[] allProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    PropertyInfo[] variableProperties = allProperties.Where(Property => Property.HasAcceptableType() || Property.IsUserDefinedTableType())
                                                                     .Where(Property => !Property.IsIgnored())
                                                                     .ToArray();

                    variable_properties_params = variableProperties.GetParameterDefinition().ToArray();

                    if (allProperties.Length != variableProperties.Length)
                        foreach (PropertyInfo property in allProperties)
                            if (!variableProperties.Any(p => p.Name == property.Name))
                                throw new EntityDatabaseException($"Variable {typeof(T).Name}.{property.Name} Property Failed, Variable Property Must Be Acceptable Data Property Or UserDefinedTableType Property");
                }

                return variable_properties_params;
            }
        }

        internal static CommandParameterVariableProperties<T> Default { get; }
    }








    internal sealed class CommandParameterAcceptableProperties<T>
    {
        static CommandParameterAcceptableProperties()
        {
            Default = new CommandParameterAcceptableProperties<T>();
        }
        private CommandParameterAcceptableProperties()
        {
        }

        private static ParameterDefinition[] acceptable_properties_params;

        internal ParameterDefinition[] Definitions
        {
            get
            {
                if (acceptable_properties_params == null)
                {
                    PropertyInfo[] allProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    PropertyInfo[] acceptableProperties = allProperties.Where(Property => Property.HasAcceptableType())
                                                                       .Where(Property => !Property.IsIgnored())
                                                                       .ToArray();

                    acceptable_properties_params = acceptableProperties.GetParameterDefinition().ToArray();

                    if (allProperties.Length != acceptableProperties.Length)
                        foreach (PropertyInfo property in allProperties)
                            if (!acceptableProperties.Any(p => p.Name == property.Name))
                                throw new EntityDatabaseException($"Acceptable {typeof(T).Name}.{property.Name} Property Failed, Variable Property Must Be Acceptable Data Property");
                }

                return acceptable_properties_params;
            }
        }

        internal static CommandParameterAcceptableProperties<T> Default { get; }
    }








    internal abstract class CommandParameter<T>
    {
        static CommandParameter()
        {
            VariableProperties = CommandParameterVariableProperties<T>.Default;
            AcceptableProperties = CommandParameterAcceptableProperties<T>.Default;
        }

        internal static CommandParameterVariableProperties<T> VariableProperties { get; }
        internal static CommandParameterAcceptableProperties<T> AcceptableProperties { get; }
    }
}
