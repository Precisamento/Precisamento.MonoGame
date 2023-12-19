using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.YarnSpinner.Utils
{
    /// <summary>
    /// Simple object pooling implementation
    /// </summary>
    /// <remarks>
    /// This implementation is a bit overengineered since it was taken from another project of mine:
    /// Soren.Collections
    /// </remarks>
    public class Pool<T>
    {
        private readonly int _maxSize = int.MaxValue;
        private readonly List<T> _pool;
        private readonly Func<T> _createItem;
        private readonly Action<T>? _resetItem;
        private readonly bool _shouldLock;
        private readonly object? _key;

        public Pool(Func<T> createItem, Action<T>? resetItem)
            : this(createItem, resetItem, 0, int.MaxValue, false)
        {
        }

        public Pool(Func<T> createItem, Action<T>? resetItem, int capacity)
            : this(createItem, resetItem, capacity, int.MaxValue, false)
        {
            _pool = new List<T>(capacity);
            _createItem = createItem;
            _resetItem = resetItem;
        }

        public Pool(Func<T> createItem, Action<T>? resetItem, int capacity, int maxSize)
            : this(createItem, resetItem, capacity, maxSize, false)
        {
        }

        public Pool(Func<T> createItem, Action<T>? resetItem, bool shouldLock)
            : this(createItem, resetItem, 0, int.MaxValue, shouldLock)
        {
        }

        public Pool(Func<T> createItem, Action<T>? resetItem, int capacity, bool shouldLock)
            : this(createItem, resetItem, capacity, int.MaxValue, shouldLock)
        {
        }

        public Pool(Func<T> createItem, Action<T>? resetItem, int capacity, int maxSize, bool shouldLock)
        {
            _pool = new List<T>(capacity);
            _maxSize = maxSize;
            _createItem = createItem;
            _resetItem = resetItem;

            if (shouldLock)
            {
                _shouldLock = shouldLock;
                _key = new object();
            }
        }

        public T Get()
        {
            if (_shouldLock)
            {
                lock (_key)
                {
                    return GetImpl();
                }
            }
            else
            {
                return GetImpl();
            }
        }

        private T GetImpl()
        {
            if (_pool.Count == 0)
                return _createItem();

            var item = _pool[^1];
            _pool.RemoveAt(_pool.Count - 1);
            return item;
        }

        public void Release(T item)
        {
            if (_shouldLock)
            {
                lock (_key)
                {
                    ReleaseImpl(item);
                }
            }
            else
            {
                ReleaseImpl(item);
            }
        }

        private void ReleaseImpl(T item)
        {
            _resetItem?.Invoke(item);
            if (_pool.Count < _maxSize)
            {
                _pool.Add(item);
            }
        }
    }
}
