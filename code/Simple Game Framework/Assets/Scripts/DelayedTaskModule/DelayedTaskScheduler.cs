using System;
using System.Collections.Generic;
using LDataStruct;
using PoolModule;
using UnityEngine;

namespace DelayedTaskModule
{
    /// <summary>
    /// 延时任务调度器
    /// </summary>
    [DefaultExecutionOrder(1)]
    public class DelayedTaskScheduler : MonoBehaviour, IDisposable
    {
        private Dictionary<string, DelayedTaskData> _taskDic = new Dictionary<string, DelayedTaskData>();
        private Dictionary<long, DelayedTaskList> _delayedTaskDict = new Dictionary<long, DelayedTaskList>();
        private Heap<DelayedTaskList> _delayedTaskQueue = new Heap<DelayedTaskList>(10, HeapType.MinHeap);
        private bool _disposed = false;
        [SerializeField] private long CurrentTime;
        public static DelayedTaskScheduler Instance { get; private set; }

        #region 时间事件管理

        /// <summary>
        /// 增加一个时间事件对象
        /// </summary>
        /// <param name="time">毫秒数</param>
        /// <param name="action"></param>
        public string AddDelayedTask(long time, Action action, Action earlyRemoveCallback = null)
        {
            if (time < CurrentTime)
            {
                Debug.LogError($"The time is pass. Time is {time} CurrentTime is {CurrentTime}");
                return null;
            }

            if (!_delayedTaskDict.TryGetValue(time, out var delayedTaskList))
            {
                delayedTaskList = ObjectPoolFactory.Instance.GetItem<DelayedTaskList>();
                delayedTaskList.Time = time;
                delayedTaskList.DelayedTaskDataList = ObjectPoolFactory.Instance.GetItem<List<DelayedTaskData>>();
                delayedTaskList.DelayedTaskDataList.Clear();
                _delayedTaskQueue.Insert(delayedTaskList);
                _delayedTaskDict.Add(time, delayedTaskList);
            }

            string token = Guid.NewGuid().ToString();
            var delayedTaskData = ObjectPoolFactory.Instance.GetItem<DelayedTaskData>();
            delayedTaskData.Time = time;
            delayedTaskData.Action = action;
            delayedTaskData.Token = token;
            delayedTaskData.EarlyRemoveCallback = earlyRemoveCallback;
            delayedTaskList.DelayedTaskDataList.Add(delayedTaskData);
            _taskDic.Add(token, delayedTaskData);
            return token;
        }

        /// <summary>
        /// 移除一个时间事件对象
        /// </summary>
        /// <param name="delayedTaskData"></param>
        /// <exception cref="Exception"></exception>
        public bool RemoveDelayedTask(string token)
        {
            _taskDic.TryGetValue(token, out var delayedTaskData);
            if (delayedTaskData == null)
                return false;
            _taskDic.Remove(token);
            if (_delayedTaskDict.TryGetValue(delayedTaskData.Time, out var delayedTaskList))
            {
                bool removeSuccess = delayedTaskList.DelayedTaskDataList.Remove(delayedTaskData);
                if (removeSuccess)
                    delayedTaskData.EarlyRemoveCallback?.Invoke();
                if (delayedTaskList.DelayedTaskDataList.Count == 0)
                {
                    _delayedTaskDict.Remove(delayedTaskData.Time);
                    if (_delayedTaskQueue.Delete(delayedTaskList))
                    {
                        ObjectPoolFactory.Instance.RecycleItem(delayedTaskList.DelayedTaskDataList);
                        ObjectPoolFactory.Instance.RecycleItem(delayedTaskList);
                        ObjectPoolFactory.Instance.RecycleItem(delayedTaskData);
                    }
                    else
                    {
                        ObjectPoolFactory.Instance.RecycleItem(delayedTaskData);
                        throw new Exception("DelayedTaskScheduler RemoveDelayedTask Error");
                    }
                }
            }
            else
            {
                ObjectPoolFactory.Instance.RecycleItem(delayedTaskData);
            }

            return true;
        }

        /// <summary>
        /// TODO:根据自己游戏的逻辑调整调用时机
        /// </summary>
        /// <param name="time"></param>
        public void UpdateTime(long time)
        {
            CurrentTime = time;
            while (_delayedTaskQueue.Count > 0 && _delayedTaskQueue.GetHead().Time <= time)
            {
                long targetTime = _delayedTaskQueue.GetHead().Time;
                _delayedTaskDict.Remove(targetTime);
                var delayedTaskList = _delayedTaskQueue.DeleteHead();
                foreach (DelayedTaskData delayedTaskData in delayedTaskList)
                {
                    delayedTaskData.Action?.Invoke();
                    _taskDic.Remove(delayedTaskData.Token);
                    ObjectPoolFactory.Instance.RecycleItem(delayedTaskData);
                }

                //回收时记得把列表清空，防止下次使用时出现问题！！！！！不要问我为什么这么多感叹号 
                delayedTaskList.DelayedTaskDataList.Clear();
                ObjectPoolFactory.Instance.RecycleItem(delayedTaskList.DelayedTaskDataList);
                ObjectPoolFactory.Instance.RecycleItem(delayedTaskList);
            }
        }

        #endregion

        #region Mono方法与测试的设置时间代码

        private void Awake()
        {
            Instance = this;
            UpdateTime(TimerUtil.GetTimeStamp(true));
        }

        public void Update()
        {
            UpdateTime(TimerUtil.GetTimeStamp(true));
        }

        private void OnDestroy()
        {
            Dispose();
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _taskDic.Clear();
                    _delayedTaskDict.Clear();
                    _delayedTaskQueue?.Dispose();
                    Instance = null;
                }

                _disposed = true;
            }
        }

        ~DelayedTaskScheduler()
        {
            Dispose(false);
        }

        #endregion
    }
}