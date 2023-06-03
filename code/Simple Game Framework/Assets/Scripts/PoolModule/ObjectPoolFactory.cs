using System;
using System.Collections;
using System.Collections.Generic;

namespace PoolModule
{
    public class ObjectPoolFactory : IDisposable
    {
        private readonly Dictionary<System.Type, object> _pools = new Dictionary<Type, object>();
        private const int DefaultPoolSize = 2;
        private const int DefaultPoolMaxSize = 500;
        private bool _disposed;
        private static ObjectPoolFactory _instance;

        public static ObjectPoolFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ObjectPoolFactory();
                }

                return _instance;
            }
        }

        public ObjectPoolFactory()
        {
            _disposed = false;
        }

        public ObjectPool<T> GetPool<T>(Func<T> objectGenerator = null, int poolSize = DefaultPoolSize) where T : new()
        {
            var type = typeof(T);
            if (!_pools.TryGetValue(type, out var pool))
            {
                pool = new ObjectPool<T>(objectGenerator, poolSize, DefaultPoolMaxSize);
                _pools.Add(type, pool);
            }

            return pool as ObjectPool<T>;
        }

        //使用列表时一定要注意清空列表，防止残留前面的数据导致Bug
        public T GetItem<T>() where T : new()
        {
            return GetPool<T>().Get();
        }

        public void RecycleItem<T>(T item) where T : new()
        {
            GetPool<T>().Recycle(item);
            if(item is IList list)
                list.Clear();
            else if(item is IDictionary dictionary)
                dictionary.Clear();
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
                foreach (var pool in _pools)
                {
                    (pool.Value as IDisposable)?.Dispose();
                }

                _pools.Clear();
            }

            _disposed = true;
        }

        ~ObjectPoolFactory()
        {
            Dispose(false);
        }
    }
}