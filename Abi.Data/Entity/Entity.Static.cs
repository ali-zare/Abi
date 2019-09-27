using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data
{
    public abstract partial class Entity<T>
    {
        static Entity()
        {
            Type = typeof(T);

            editing_entities = new HashSet<T>();
            snapshot_entitySet_of_entities = new Dictionary<T, EntitySet<T>>();
            snapshot_entityState_of_entities = new Dictionary<T, EntityState>();

            #region Create Related Properties Initializer

            int counter = 0;

            Type typeForeignEntity = Type;

            ParameterExpression argEntity = Expression.Parameter(typeForeignEntity, "entity");

            PropertyInfo[] entityRelatedProperties = Type.GetEntityAllRelatedProperties().ToArray();

            Expression[] expressions = new Expression[entityRelatedProperties.Length];

            foreach (PropertyInfo propRelated in entityRelatedProperties)
            {
                Type propType = propRelated.PropertyType;

                Type typeRelatedEntity = propType.GenericTypeArguments[0];

                MethodInfo propSetMthd = propRelated.GetSetMethod();

                if (propType.GetGenericTypeDefinition() == typeof(EntityCollection<>))
                {
                    Type typeEntityCollection = typeof(EntityCollection<,>).MakeGenericType(typeForeignEntity, typeRelatedEntity);

                    ConstructorInfo ctrEntityCollection = typeEntityCollection.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);

                    expressions[counter++] = Expression.Call(argEntity, propSetMthd, Expression.New(ctrEntityCollection));
                }
                else if (propType.GetGenericTypeDefinition() == typeof(EntityUnique<>))
                {
                    Type typeEntityUnique = typeof(EntityUnique<,>).MakeGenericType(typeForeignEntity, typeRelatedEntity);

                    ConstructorInfo ctrEntityUnique = typeEntityUnique.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);

                    expressions[counter++] = Expression.Call(argEntity, propSetMthd, Expression.New(ctrEntityUnique));
                }
                else
                    throw new CriticalException($"Entity<{Type.Name}> Initialize Related Properties Failed, {propType} Type Is Not Supported");
            }

            if (entityRelatedProperties.Length > 0)
                initializeRelatedProperties = Expression.Lambda<Action<T>>(Expression.Block(expressions), argEntity).Compile();

            #endregion Create Related Properties Initializer
        }

        private static Action<T> initializeRelatedProperties;
        private static HashSet<T> editing_entities;
        private static Dictionary<T, EntitySet<T>> snapshot_entitySet_of_entities;
        private static Dictionary<T, EntityState> snapshot_entityState_of_entities;

        public static Type Type { get; private set; }


        private static PropertyInfo[] acceptableProperties;
        public static PropertyInfo[] AcceptableProperties
        {
            get
            {
                if (acceptableProperties == null)
                    acceptableProperties = Type.GetEntityAcceptableProperties().ToArray();

                return acceptableProperties;
            }
        }


        private static PropertyInfo[] acceptableDataProperties;
        public static PropertyInfo[] AcceptableDataProperties
        {
            get
            {
                if (acceptableDataProperties == null)
                    acceptableDataProperties = Type.GetEntityAcceptableDataProperties().ToArray();

                return acceptableDataProperties;
            }
        }
    }
}
