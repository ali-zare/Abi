using System.Collections;
using System.Collections.Generic;

namespace Abi.Data.Sql
{
    internal class OneWayCollection<T> : IEnumerable<T>
    {
        internal OneWayCollection()
        {
            items = new List<T>();
        }

        private List<T> items;

        internal int Count => items.Count;
        internal void Add(T item) => items.Add(item);

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in items)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable
    }
}
