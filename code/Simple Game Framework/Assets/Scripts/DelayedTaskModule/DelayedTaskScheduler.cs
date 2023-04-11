using System;
using System.Collections.Generic;
using LDataStruct;
using UnityEngine;

namespace DelayedTaskModule
{
    /// <summary>
    /// 延时任务调度器
    /// </summary>
    [DefaultExecutionOrder(1)]
    public class DelayedTaskScheduler : MonoBehaviour, IDisposable
    {
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
        public DelayedTaskData AddDelayedTask(long time, Action action, Action earlyRemoveCallback = null)
        {
            if (time < CurrentTime)
            {
                Debug.LogError($"The time is pass. Time is {time} CurrentTime is {CurrentTime}");
                return null;
            }

            if (!_delayedTaskDict.TryGetValue(time, out var delayedTaskList))
            {
                delayedTaskList = new DelayedTaskList();
                delayedTaskList.Time = time;
                delayedTaskList.DelayedTaskDataList = new List<DelayedTaskData>();
                delayedTaskList.DelayedTaskDataList.Clear();
                _delayedTaskQueue.Insert(delayedTaskList);
                _delayedTaskDict.Add(time, delayedTaskList);
            }

            var newEventData = new DelayedTaskData();
            newEventData.Time = time;
            newEventData.Action = action;
            newEventData.EarlyRemoveCallback = earlyRemoveCallback;
            delayedTaskList.DelayedTaskDataList.Add(newEventData);
            return newEventData;
        }

        /// <summary>
        /// 移除一个时间事件对象
        /// </summary>
        /// <param name="delayedTaskData"></param>
        /// <exception cref="Exception"></exception>
        public void RemoveDelayedTask(DelayedTaskData delayedTaskData)
        {
            if (delayedTaskData == null)
                return;
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
                        delayedTaskList.Dispose();
                    }
                    else
                    {
                        throw new Exception("DelayedTaskScheduler RemoveDelayedTask Error");
                    }
                }
            }
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
                }

                delayedTaskList.Dispose();
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