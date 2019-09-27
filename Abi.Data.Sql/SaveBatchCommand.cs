using System;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    internal class SaveBatchCommand
    {
        static SaveBatchCommand()
        {
            get_dependency_funcs = new Dictionary<Type, Func<Entity, PropertyInfo, Entity>>();
        }
        internal SaveBatchCommand()
        {
            key_parameter_identities = new Dictionary<Entity, string>();
            commands = new Dictionary<Entity, EntityCommand>();

            Exceptions = new OneWayCollection<Entity>();
        }


        private static Dictionary<Type, Func<Entity, PropertyInfo, Entity>> get_dependency_funcs;

        private int parameter_number;
        private Dictionary<Entity, string> key_parameter_identities;
        private Dictionary<Entity, EntityCommand> commands;



        internal bool HasException => Exceptions.Count > 0;
        internal OneWayCollection<Entity> Exceptions { get; }



        internal ParameterIdentity GetParameter(Entity Entity, PropertyInfo Property)
        {
            if (!get_dependency_funcs.TryGetValue(Entity.EntityType, out Func<Entity, PropertyInfo, Entity> get_dependency))
                get_dependency_funcs.Add(Entity.EntityType, get_dependency = generate_get_dependency_func(Entity.EntityType));

            Entity ForeignEntity = get_dependency(Entity, Property);

            if (ForeignEntity != null && ForeignEntity.IsAdded())
                return new ParameterIdentity(GetKeyParameter(ForeignEntity), true);

            return new ParameterIdentity();
        }
        private string GetParameter()
        {
            return $"@P{parameter_number++}";
        }
        private string GetKeyParameter(Entity Entity)
        {
            if (!key_parameter_identities.TryGetValue(Entity, out string key_parameter_identity))
                key_parameter_identities.Add(Entity, key_parameter_identity = CreateKeyParameter(Entity));

            return key_parameter_identity;
        }
        private string CreateKeyParameter(Entity Entity)
        {
            EntityCommandInsert command = (EntityCommandInsert)commands[Entity];

            string param = GetParameter();

            command.Change(new ParameterIdentity(param, true));

            return param;
        }

        internal void Add(EntityCommand Command)
        {
            commands.Add(Command.Entity, Command);
        }

        internal string GetCommandText()
        {
            StringBuilder builder = new StringBuilder();

            foreach (EntityCommand command in commands.Values)
            {
                command.Append(builder);

                builder.AppendLine()
                       .AppendLine();
            }

            return builder.ToString();
        }



        private static Func<Entity, PropertyInfo, Entity> generate_get_dependency_func(Type type)
        {
            Type typeEntity = type;
            Type typeEntitySet = typeof(EntitySet<>).MakeGenericType(typeEntity);

            MethodInfo propEntitySetGetMthd = typeEntity.GetProperty("EntitySet", typeEntitySet).GetGetMethod(true);
            MethodInfo mthdGetDependency = typeEntitySet.GetMethod("GetDependency", BindingFlags.NonPublic | BindingFlags.Instance);

            ParameterExpression argEntity = Expression.Parameter(typeof(Entity), "Entity");
            ParameterExpression argProperty = Expression.Parameter(typeof(PropertyInfo), "Property");

            return Expression.Lambda<Func<Entity, PropertyInfo, Entity>>(Expression.Call(Expression.Call(Expression.TypeAs(argEntity, typeEntity), propEntitySetGetMthd), mthdGetDependency, Expression.TypeAs(argEntity, typeEntity), argProperty), argEntity, argProperty).Compile();
        }

    }
}
