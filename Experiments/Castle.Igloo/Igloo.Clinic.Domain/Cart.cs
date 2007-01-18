using System;
using System.Collections;

namespace Igloo.Clinic.Domain
{
    public class Cart : ICart
    {
        private IList _items = new ArrayList();

        #region IList Members

        public int Add(object value)
        {
            return _items.Add(value);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(object value)
        {
            return _items.Contains(value);
        }

        public int IndexOf(object value)
        {
            return _items.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            _items.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return _items.IsFixedSize; }
        }

        public bool IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        public void Remove(object value)
        {
            _items.Remove(value);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public object this[int index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            _items.CopyTo(array, index);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsSynchronized
        {
            get { return _items.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return _items.SyncRoot; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator(); ;
        }

        #endregion
    }
}
