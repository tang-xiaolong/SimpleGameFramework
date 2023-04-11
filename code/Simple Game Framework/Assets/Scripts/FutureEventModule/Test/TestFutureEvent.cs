using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FutureEventModule.Test
{
    public class TestFutureEvent : MonoBehaviour
    {
        private void Start()
        {
            AddLaterExecuteFunc(1.5f);
            AddLaterExecuteFunc(4.5f);
        }

        public int forceTestCount = 100000;
        public List<FutureEventData> futureEventDataList = new List<FutureEventData>(100000);
        public List<long> testTimes = new List<long>(100000);
        
        [ContextMenu("暴力测试")]
        public void ForceTest()
        {
            testTimes.Clear();
            for (int i = 0; i < forceTestCount; i++)
            {
                testTimes.Add(TimerUtil.GetLaterMilliSecondsBySecond(UnityEngine.Random.Range(1, 15.0f)));
            }
            futureEventDataList.Clear();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < forceTestCount; i++)
            {
                futureEventDataList.Add(FutureEventScheduler.Instance.AddFutureEvent(testTimes[i], TestFunc));
            }

            for (int i = 0; i < forceTestCount; i++)
            {
                FutureEventScheduler.Instance.RemoveFutureEvent(futureEventDataList[i]);
            }
            
            stopwatch.Stop();
            Debug.Log($"暴力测试完成，共耗时{stopwatch.ElapsedMilliseconds / 1000.0f}秒");
        }

        void TestFunc()
        {
            Debug.Log("测试方法执行了");
        }

        private FutureEventData AddLaterExecuteFunc(float time, Action completeAction = null, Action earlyRemoveAction = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            return FutureEventScheduler.Instance.AddFutureEvent(TimerUtil.GetLaterMilliSecondsBySecond(time),
                () =>
                {
                    stopwatch.Stop();
                    Debug.Log($"{time}秒后了,执行了对应方法。实际过去了{stopwatch.ElapsedMilliseconds / 1000.0f}秒");
                    completeAction?.Invoke();
                }, () =>
                {
                    earlyRemoveAction?.Invoke();
                    stopwatch.Stop();
                    Debug.Log($"提前移除了，已经过去了{stopwatch.ElapsedMilliseconds / 1000.0f}秒");
                });
        }

        FutureEventData _futureEventData;

        void RecycleFutureEvent()
        {
            if (_futureEventData != null)
            {
                FutureEventScheduler.Instance.RemoveFutureEvent(_futureEventData);
                _futureEventData = null;
            }
        }

        private void Update()
        {
            //持续按下时不断创建和回收
            if (Input.GetKeyDown(KeyCode.C))
            {
                RecycleFutureEvent();
                _futureEventData = AddLaterExecuteFunc(UnityEngine.Random.Range(1, 5.0f), () => _futureEventData = null);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RecycleFutureEvent();
            }
        }
    }
}