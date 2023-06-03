using System;
using System.Collections.Generic;
using UnityEngine;

namespace PoolModule
{
    [Serializable]
    public class AutoRecycleConfigItem
    {
        public string itemName;
        public int recycleTime;
    }

    [Serializable]
    public class AutoRecycleConfig
    {
        public AutoRecycleConfigItem[] conf;
    }
    
    public class AutoRecycleConf : IDisposable
    {
        private Dictionary<string, float> _recycleTimeDic = new Dictionary<string, float>();
        private bool _hasInit = false;
        private bool _disposed;

        public float GetRecycleTime(string name)
        {
            if (!_hasInit)
            {
                _recycleTimeDic.Clear();
                TextAsset conf = Resources.Load<TextAsset>("AutoRecycle");
                if (conf != null)
                {
                    AutoRecycleConfig autoRecycleConfig = JsonUtility.FromJson<AutoRecycleConfig>(conf.text);
                    if (autoRecycleConfig != null)
                    {
                        foreach (var confItem in autoRecycleConfig.conf)    
                        {
                            if(!_recycleTimeDic.ContainsKey(confItem.itemName))
                                _recycleTimeDic.Add(confItem.itemName, confItem.recycleTime);
                        }
                    }
                }
                _hasInit = true;
                _disposed = false;
            }
            if(_recycleTimeDic.TryGetValue(name, out var time))
            {
                return time;
            }
            else
            {
                return 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _recycleTimeDic.Clear();
                }

                _disposed = true;
                _hasInit = false;
            }
        }
        ~AutoRecycleConf()
        {
            Dispose(false);
        }
    }
}