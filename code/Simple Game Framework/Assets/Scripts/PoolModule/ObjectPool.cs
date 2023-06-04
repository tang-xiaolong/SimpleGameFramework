using System;
using System.Collections;
using System.Collections.Generic;

namespace PoolModule
{
    public class ObjectPool<T> : IObjectPool<T>, IDisposable where T : new()
    {
        private Queue<T> _objects;
        private Func<T> _objectFactory;
        private int InitialPoolSize = 0;
        private readonly int MaxPoolSize = 200;
        private int _curCount = 0;
        private bool _disposed;

        public ObjectPool(Func<T> objectFactory, int initialPoolSize = 0, int maxPoolSize = 200)
        {
            _disposed = false;
            _objectFactory = objectFactory;
            _objects = new Queue<T>();
            MaxPoolSize = maxPoolSize;
            InitialPoolSize = initialPoolSize;
            _curCount = 0;
            for (int i = 0; i < InitialPoolSize; i++)
            {
                var obj = CreateObject();
                if (obj != null)
                    _objects.Enqueue(obj);
            }
        }

        public T Get()
        {
            T item = _objects.Count == 0 ? CreateObject() : _objects.Dequeue();
            if (item != null)
                DequeueHandle(item);
            return item;
        }

        public void Recycle(T item)
        {
            if (item == null)
                return;
            if (!_objects.Contains(item))
            {
                EnqueueHandle(item);
                _objects.Enqueue(item);
            }
        }

        private T CreateObject()
        {
            if (_curCount >= MaxPoolSize) return default;
            var newObject = _objectFactory != null ? _objectFactory() : new T();
            _curCount++;
            EnqueueHandle(newObject);
            return newObject;
        }

        public void Cleanup(Func<T, bool> shouldCleanupFunc)
        {
            int count = _objects.Count;
            for (int i = 0; i < count; i++)
            {
                T item = _objects.Dequeue();
                if (!shouldCleanupFunc(item))
                    _objects.Enqueue(item);
                else
                    _curCount--;
            }
        }

        public void EnqueueHandle(T item)
        {
            if(item is IPoolObjectItem iPoolItem)
                iPoolItem.OnRecycleHandle();
            if(item is IList list)
                list.Clear();
            else if(item is IDictionary dictionary)
                dictionary.Clear();
        }

        public void DequeueHandle(T item)
        {
            if(item is IPoolObjectItem iPoolItem)
                iPoolItem.OnGetHandle();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (_objects != null)
                {
                    _objects.Clear();
                    _objects = null;
                }

                _objectFactory = null;
            }

            _curCount = 0;
            _disposed = true;
        }

        ~ObjectPool()
        {
            Dispose(false);
        }
    }
}