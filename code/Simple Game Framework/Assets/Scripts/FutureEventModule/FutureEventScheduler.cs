using System;
using System.Collections.Generic;
using LDataStruct;
using UnityEngine;

namespace FutureEventModule
{
    [DefaultExecutionOrder(1)]
    public class FutureEventScheduler : MonoBehaviour, IDisposable
    {
        private Dictionary<long, FutureEventList> _futureEventDict = new Dictionary<long, FutureEventList>();
        private Heap<FutureEventList> _futureEventQueue = new Heap<FutureEventList>(10, HeapType.MinHeap);
        private bool _disposed = false;
        [SerializeField] private long CurrentTime;
        public static FutureEventScheduler Instance { get; private set; }

        #region 时间事件管理

        /// <summary>
        /// 增加一个时间事件对象
        /// </summary>
        /// <param name="time">毫秒数</param>
        /// <param name="action"></param>
        public FutureEventData AddFutureEvent(long time, Action action, Action earlyRemoveCallback = null)
        {
            if (time < CurrentTime)
            {
                Debug.LogError($"The time is pass. Time is {time} CurrentTime is {CurrentTime}");
                return null;
            }

            if (!_futureEventDict.TryGetValue(time, out var futureEventList))
            {
                futureEventList = new FutureEventList();
                futureEventList.Time = time;
                futureEventList.FutureEventDataList = new List<FutureEventData>();
                futureEventList.FutureEventDataList.Clear();
                _futureEventQueue.Insert(futureEventList);
                _futureEventDict.Add(time, futureEventList);
            }

            var newEventData = new FutureEventData();
            newEventData.Time = time;
            newEventData.Action = action;
            newEventData.EarlyRemoveCallback = earlyRemoveCallback;
            futureEventList.FutureEventDataList.Add(newEventData);
            return newEventData;
        }

        /// <summary>
        /// 移除一个时间事件对象
        /// </summary>
        /// <param name="futureEventData"></param>
        /// <exception cref="Exception"></exception>
        public void RemoveFutureEvent(FutureEventData futureEventData)
        {
            if (futureEventData == null)
                return;
            if (_futureEventDict.TryGetValue(futureEventData.Time, out var futureEventList))
            {
                _futureEventDict.Remove(futureEventData.Time);
                bool removeSuccess = futureEventList.FutureEventDataList.Remove(futureEventData);
                if (removeSuccess)
                    futureEventData.EarlyRemoveCallback?.Invoke();
                if (futureEventList.FutureEventDataList.Count == 0)
                {
                    if (_futureEventQueue.Delete(futureEventList))
                    {
                        futureEventList.Dispose();
                    }
                    else
                    {
                        throw new Exception("FutureEventScheduler RemoveFutureEvent Error");
                    }
                }
                else
                {
                    Debug.LogError("移除后数量不为0 removeSuccess is " + removeSuccess);
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
            while (_futureEventQueue.Count > 0 && _futureEventQueue.GetHead().Time <= time)
            {
                long targetTime = _futureEventQueue.GetHead().Time;
                _futureEventDict.Remove(targetTime);
                var futureEventList = _futureEventQueue.DeleteHead();
                foreach (FutureEventData futureEventData in futureEventList)
                {
                    futureEventData.Action?.Invoke();
                }
                futureEventList.Dispose();
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
                    _futureEventQueue?.Dispose();
                    Instance = null;
                }

                _disposed = true;
            }
        }

        ~FutureEventScheduler()
        {
            Dispose(false);
        }

        #endregion
    }
}