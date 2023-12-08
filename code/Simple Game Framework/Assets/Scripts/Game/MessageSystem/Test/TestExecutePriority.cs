using System;
using UnityEngine;

namespace Game.MessageSystem.Test
{
    public class TestExecutePriority : MonoBehaviour
    {
        private void Awake()
        {
            MessageManager.Instance.Register<int>(TestMessageDefine.OnScoreChange, (score) =>
            {
                Debug.Log("先注册的UI事件");
            }, MessageDispatchPriority.UI);
            MessageManager.Instance.Register<int>(TestMessageDefine.OnScoreChange, (score) =>
            {
                Debug.Log("后注册的数据事件");
            }, MessageDispatchPriority.CommonDataUpdate);
            MessageManager.Instance.Register<int>(TestMessageDefine.OnScoreChange, (score) =>
            {
                Debug.Log("后注册的UI事件");
            }, MessageDispatchPriority.UI);
        }
    }
}