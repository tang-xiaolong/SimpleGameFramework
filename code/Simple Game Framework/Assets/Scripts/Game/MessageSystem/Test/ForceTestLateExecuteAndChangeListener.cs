using System;
using UnityEngine;

namespace Game.MessageSystem.Test
{
    public class ForceTestLateExecuteAndChangeListener : MonoBehaviour
    {
        int count = 0;
        private void Awake()
        {
            MessageManager.Instance.Register(TestMessageDefine.LoopDispatch, () =>
            {
                count++;
                Debug.Log("LoopDispatch, Count:" + count);
                MessageManager.Instance.Send(TestMessageDefine.LoopDispatch);
            }, MessageDispatchPriority.UI);
            MessageManager.Instance.Register<int>(TestMessageDefine.LoopDispatch1, (placehold) =>
            {
                count++;
                Debug.Log("LoopDispatch1, Count:" + count);
                MessageManager.Instance.Send(TestMessageDefine.LoopDispatch1, 1);
            }, MessageDispatchPriority.UI);
            
            MessageManager.Instance.Register(TestMessageDefine.MultipleLoopDispatch1, () =>
            {
                count++;
                Debug.Log("MultipleLoopDispatch1, Count:" + count);
                MessageManager.Instance.Send(TestMessageDefine.MultipleLoopDispatch2);
            }, MessageDispatchPriority.UI);
            MessageManager.Instance.Register(TestMessageDefine.MultipleLoopDispatch2, () =>
            {
                count++;
                Debug.Log("MultipleLoopDispatch2, Count:" + count);
                MessageManager.Instance.Send(TestMessageDefine.MultipleLoopDispatch1);
            }, MessageDispatchPriority.UI);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                count = 0;
                MessageManager.Instance.Send(TestMessageDefine.LoopDispatch);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                count = 0;
                MessageManager.Instance.Send(TestMessageDefine.LoopDispatch1, 1);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                count = 0;
                MessageManager.Instance.Send(TestMessageDefine.MultipleLoopDispatch1);
            }
        }
    }
}