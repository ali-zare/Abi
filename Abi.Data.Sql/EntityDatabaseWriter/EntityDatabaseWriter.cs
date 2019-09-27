using System.Data.SqlClient;

namespace Abi.Data.Sql
{
    public abstract class EntityDatabaseWriter
    {
        private protected EntityDatabaseWriter(EntityDatabaseConnection Connection) => this.Connection = Connection;

        private protected EntityDatabaseConnection Connection;

        internal abstract void Write(Entity Entity);
        internal abstract void Write(Entity Entity, SaveBatchCommand Batch, SqlDataReader Reader);

        internal abstract void Append(Entity Entity, SaveBatchCommand Batch);
        internal abstract void Append(Entity Entity, EntityCommand Command, ParameterIdentity Parameter);
    }
}
