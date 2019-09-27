using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace Abi.Data
{
    internal sealed class EntityProxy<TEntity, TData> : EntityProxy<TEntity> where TEntity : Entity<TEntity> where TData : struct
    {
        static EntityProxy()
        {
            original_data_of_entities = new Dictionary<TEntity, TData>();
            snapshot_data_of_entities = new Dictionary<TEntity, TData>();
        }
        internal EntityProxy()
        {
        }

        private static Dictionary<TEntity, TData> original_data_of_entities;
        private static Dictionary<TEntity, TData> snapshot_data_of_entities;



        public TData item;

        internal override P Get<P>(PropertyInfo Property)
        {
            return Extension.Get<TData, P>(ref item, GetFieldInfo(Property));
        }
        internal override void Set<P>(PropertyInfo Property, P Value)
        {
            Extension.Set(ref item, GetFieldInfo(Property), Value);
        }


        internal override void TransformBegin(TEntity Entity)
        {
            original_data_of_entities.Add(Entity, item);
        }
        internal override void TransformEnd(TEntity Entity)
        {
            original_data_of_entities.Remove(Entity);
        }
        internal override void TransformCancel(TEntity Entity)
        {
            EntityProxy<TEntity, TData> data = (EntityProxy<TEntity, TData>)Entity.Data;

            EntityProxy<TEntity, TData> not_committed_data = (EntityProxy<TEntity, TData>)Create();

            not_committed_data.item = data.item;

            data.item = original_data_of_entities[Entity];

            original_data_of_entities.Remove(Entity);

            Entity.EntitySet.Cancel(Entity, not_committed_data);
        }

        internal override void TakeSnapshot(TEntity Entity)
        {
            snapshot_data_of_entities.Add(Entity, ((EntityProxy<TEntity, TData>)Entity.Data).item);
        }
        internal override void RestoreSnapshot(TEntity Entity)
        {
            EntityProxy<TEntity, TData> data = (EntityProxy<TEntity, TData>)Entity.Data;

            EntityProxy<TEntity, TData> not_committed_data = (EntityProxy<TEntity, TData>)Create();

            not_committed_data.item = data.item;

            data.item = snapshot_data_of_entities[Entity];

            snapshot_data_of_entities.Remove(Entity);

            Entity.EntitySet.Cancel(Entity, not_committed_data);
        }
        internal override void RemoveSnapshot(TEntity Entity)
        {
            snapshot_data_of_entities.Remove(Entity);
        }
    }


    internal abstract class EntityProxy<TEntity> : EntityProxy where TEntity : Entity<TEntity>
    {
        static EntityProxy()
        {
            // Step 1 :

            Type typeEntity = Entity<TEntity>.Type;

            TypeBuilder tb = module.DefineType($"Abi.Data.EntityData.{typeEntity.Name}Data", TypeAttributes.Public, typeof(ValueType));

            PropertyInfo[] acceptableProperties = Entity<TEntity>.AcceptableProperties;

            foreach (PropertyInfo prop in acceptableProperties)
                tb.DefineField(prop.Name, prop.PropertyType, FieldAttributes.Public);

            Type typeEntityData = DataType = tb.CreateTypeInfo();

            foreach (PropertyInfo prop in acceptableProperties)
                propsEntityData.Add(prop, typeEntityData.GetField(prop.Name));

            // Step 2 :

            Type entityProxy = typeof(EntityProxy<,>).MakeGenericType(typeEntity, typeEntityData);
            ConstructorInfo ctr = entityProxy.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);

            funcProxyCreator = Expression.Lambda<Func<EntityProxy<TEntity>>>(Expression.New(ctr)).Compile();

        }
        internal EntityProxy()
        {
        }

        private static Func<EntityProxy<TEntity>> funcProxyCreator;

        internal static EntityProxy<TEntity> Create()
        {
            return funcProxyCreator();
        }

        internal static Type DataType { get; }


        internal abstract P Get<P>(PropertyInfo Property);
        internal abstract void Set<P>(PropertyInfo Property, P Value);


        internal abstract void TransformBegin(TEntity Entity);
        internal abstract void TransformEnd(TEntity Entity);
        internal abstract void TransformCancel(TEntity Entity);

        internal abstract void TakeSnapshot(TEntity Entity);
        internal abstract void RestoreSnapshot(TEntity Entity);
        internal abstract void RemoveSnapshot(TEntity Entity);
    }

    internal abstract class EntityProxy
    {
        static EntityProxy()
        {
            AssemblyName name = new AssemblyName("Abi.Data.EntityData");
            assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            module = assembly.DefineDynamicModule(name.Name);

            propsEntityData = new Dictionary<PropertyInfo, FieldInfo>();
        }

        protected static AssemblyBuilder assembly;
        protected static ModuleBuilder module;
        protected static Dictionary<PropertyInfo, FieldInfo> propsEntityData;

        protected static FieldInfo GetFieldInfo(PropertyInfo Property)
        {
            try
            {
                return propsEntityData[Property];
            }
            catch
            {
                throw new CriticalException($"EntityProxy Can Not Found Data Field For {Property.DeclaringType.Name}.{Property.Name}");
            }
        }
    }
}