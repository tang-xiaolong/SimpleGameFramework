using System;
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
            AddLaterExecuteFunc(1.5f);
            AddLaterExecuteFunc(1.5f);
            AddLaterExecuteFunc(4.5f);
            AddLaterExecuteFunc(4.5f);
            AddLaterExecuteFunc(4.5f);
        }

        private FutureEventData AddLaterExecuteFunc(float time, Action completeAction = null, Action earlyRemoveAction = null)
        {
            var pressTime = Time.time;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            return FutureEventScheduler.Instance.AddFutureEvent(TimerUtil.GetLaterMilliSecondsBySecond(time),
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
            if (Input.GetKey(KeyCode.C))
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