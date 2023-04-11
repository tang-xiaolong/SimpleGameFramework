using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DelayedTaskModule
{
    public class TestDelayTask : MonoBehaviour
    {
        private void Start()
        {
            AddLaterExecuteFunc(1.5f);
            AddLaterExecuteFunc(1.5f);
            AddLaterExecuteFunc(1.5f);
            AddLaterExecuteFunc(4.5f);
            AddLaterExecuteFunc(4.5f);
            AddLaterExecuteFunc(4.5f);
        }
        
        public int forceTestCount = 100000;
        List<DelayedTaskData> futureEventDataList = new List<DelayedTaskData>(100000);
        List<long> testTimes = new List<long>(100000);
        
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
                futureEventDataList.Add(DelayedTaskScheduler.Instance.AddDelayedTask(testTimes[i], TestFunc));
            }

            for (int i = 0; i < forceTestCount; i++)
            {
                DelayedTaskScheduler.Instance.RemoveDelayedTask(futureEventDataList[i]);
            }
            
            stopwatch.Stop();
            Debug.Log($"暴力测试完成，共耗时{stopwatch.ElapsedMilliseconds / 1000.0f}秒");
        }

        void TestFunc()
        {
            Debug.Log("测试方法执行了");
        }

        private DelayedTaskData AddLaterExecuteFunc(float time, Action completeAction = null, Action earlyRemoveAction = null)
        {
            var pressTime = Time.time;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            return DelayedTaskScheduler.Instance.AddDelayedTask(TimerUtil.GetLaterMilliSecondsBySecond(time),
                () =>
                {
                    stopwatch.Stop();
                    // Debug.Log($"{time}秒后了,执行了对应方法。实际过去了{Time.time - pressTime}秒");
                    Debug.Log($"{time}秒后了,执行了对应方法。实际过去了{stopwatch.ElapsedMilliseconds / 1000.0f}秒");
                    completeAction?.Invoke();
                }, () =>
                {
                    earlyRemoveAction?.Invoke();
                    stopwatch.Stop();
                    Debug.Log($"提前移除了，已经过去了{stopwatch.ElapsedMilliseconds / 1000.0f}秒");
                });
        }

        DelayedTaskData _delayedTaskData;

        void RecycleDelayedTask()
        {
            if (_delayedTaskData != null)
            {
                DelayedTaskScheduler.Instance.RemoveDelayedTask(_delayedTaskData);
                _delayedTaskData = null;
            }
        }

        private void Update()
        {
            //持续按下时不断创建和回收
            if (Input.GetKey(KeyCode.C))
            {
                RecycleDelayedTask();
                _delayedTaskData = AddLaterExecuteFunc(UnityEngine.Random.Range(1, 5.0f), () => _delayedTaskData = null);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RecycleDelayedTask();
            }
        }
    }
}