using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PoolModule
{
    [Serializable]
    public class UnityObjectPool : IObjectPool<Object>, IDisposable
    {
        public Object ItemPrefab;
        public int InitialPoolSize = 0;
        public readonly int MaxPoolSize = 500;
        private int _curCount = 0;
        private readonly Func<Object> _objectFactory;
        private readonly Queue<Object> _objects;
        private Action<Object> _enterQueueHandle;
        private Action<Object> _dequeueHandle;

        private bool _disposed;

        public UnityObjectPool(Object itemPrefab, Func<Object> objectFactory,
            int initialPoolSize = 0, int maxPoolSize = 500, Action<Object> enterQueueHandle = null,
            Action<Object> dequeueHandle = null)
        {
            _disposed = false;
            ItemPrefab = itemPrefab;
            _objectFactory = objectFactory;
            MaxPoolSize = maxPoolSize;
            InitialPoolSize = initialPoolSize;
            _enterQueueHandle = enterQueueHandle;
            _dequeueHandle = dequeueHandle;
            _objects = new Queue<Object>(MaxPoolSize);
            _curCount = 0;
            for (int i = 0; i < InitialPoolSize; i++)
            {
                var obj = CreateObject();
                if (obj != null)
                    _objects.Enqueue(obj);
            }
        }

        public Object Get()
        {
            Object item = _objects.Count == 0 ? CreateObject() : _objects.Dequeue();
            if (item != null)
                DequeueHandle(item);
            if (item is GameObject obj)
                obj.SetActive(true);
            _dequeueHandle?.Invoke(item);
            DequeueHandle(item);
            return item;
        }

        public void Recycle(Object item)
        {
            if (item == null)
                return;
            if (!_objects.Contains(item))
            {
                if (item is GameObject obj)
                    obj.SetActive(false);
                _enterQueueHandle?.Invoke(item);
                EnqueueHandle(item);
                _objects.Enqueue(item);
            }
        }

        protected Object CreateObject()
        {
            if (_curCount >= MaxPoolSize) return default;
            var newObject = _objectFactory != null
                ? _objectFactory()
                : Object.Instantiate(ItemPrefab);
            if (newObject is GameObject obj)
                obj.SetActive(false);
            _curCount++;
            _enterQueueHandle?.Invoke(newObject);
            EnqueueHandle(newObject);
            return newObject;
        }

        public void Cleanup(Func<Object, bool> shouldCleanup)
        {
            int count = _objects.Count;
            for (int i = 0; i < count; i++)
            {
                var obj = _objects.Dequeue();
                if (shouldCleanup(obj))
                {
                    Object.Destroy(obj);
                    _curCount--;
                }
                else
                    _objects.Enqueue(obj);
            }
        }

        public virtual void EnqueueHandle(Object item)
        {
        }

        public virtual void DequeueHandle(Object item)
        {
        }

        public void Dispose()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                Cleanup(item => true);
                _objects.Clear();
            }

            _disposed = true;
        }

        ~UnityObjectPool()
        {
            Dispose(false);
        }
    }
}