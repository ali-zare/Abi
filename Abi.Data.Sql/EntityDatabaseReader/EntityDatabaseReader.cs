using System.Threading;
using System.Data.SqlClient;

namespace Abi.Data.Sql
{
    internal abstract class EntityDatabaseReader
    {
        private protected EntityDatabaseReader() { }

        internal abstract void Fill(SqlDataReader Reader, EntitySet EntitySet);
        internal abstract void Merge(SqlDataReader Reader, EntitySet EntitySet);
        internal abstract void Refresh(SqlDataReader Reader, EntitySet EntitySet);


        internal abstract void FillAsync(SqlDataReader Reader, EntitySet EntitySet, CancellationToken CancellationToken);
        internal abstract void MergeAsync(SqlDataReader Reader, EntitySet EntitySet, CancellationToken CancellationToken);
        internal abstract void RefreshAsync(SqlDataReader Reader, EntitySet EntitySet, CancellationToken CancellationToken);
    }
}