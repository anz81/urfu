using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Urfu.Its.VersionedDocs.Core
{
    public class VersionedDocumentItemCollection<T> : IList<T> where T : IVersionedDocumentBlockItemDescriptor
    {
        public IVersionedDocumentBlockEnumerableItem Parent { get; }
        private readonly List<T> _items = new List<T>();

        public VersionedDocumentItemCollection(IVersionedDocumentBlockEnumerableItem parent)
        {
            Parent = parent;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void OnItemAdded(T item, int index)
        {

        }

        protected virtual void OnItemRemoved(T item)
        {

        }

        public void Add(T item)
        {
            _items.Add(item);
            OnItemAdded(item, _items.Count - 1);
        }

        public void Clear()
        {
            var removed = _items.ToList();
            _items.Clear();
            foreach (var item in removed)
                OnItemRemoved(item);
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var removed = _items.Remove(item);
            if (removed)
                OnItemRemoved(item);
            return removed;
        }

        public int Count => _items.Count;
        public bool IsReadOnly => false;
        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
            OnItemAdded(item, index);
        }

        public void RemoveAt(int index)
        {
            var item = _items[index];
            _items.RemoveAt(index);
            OnItemRemoved(item);
        }

        public T this[int index]
        {
            get => _items[index];
            set
            {
                var oldItem = _items[index];
                _items[index] = value;
                OnItemRemoved(oldItem);
                OnItemAdded(value, index);
            }
        }
    }
}