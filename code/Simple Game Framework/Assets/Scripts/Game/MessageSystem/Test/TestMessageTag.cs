using System;
using Game.MessageSystem.Test;
using UnityEngine;

public class TestMessageTag : MonoBehaviour
{
    private string _tag = "验证Tag"; 
    private void Update()
    {
        //按A注册消息，按S和D发送消息，按F移除消息
        if (Input.GetKeyDown(KeyCode.A))
        {
            MessageManager.Instance.Register(TestMessageDefine.MessageTag1, () =>
            {
                Debug.Log("测试消息1");
            }, _tag);
            MessageManager.Instance.Register(TestMessageDefine.MessageTag1, () =>
            {
                Debug.Log("测试消息1 copy");
            }, _tag);
            MessageManager.Instance.Register<int>(TestMessageDefine.MessageTag2, (value) =>
            {
                Debug.Log("测试消息2, Value is " + value);
            }, _tag);
        }
        
        if(Input.GetKeyDown(KeyCode.S))
            MessageManager.Instance.Send(TestMessageDefine.MessageTag1);
        
        if(Input.GetKeyDown(KeyCode.D))
            MessageManager.Instance.Send(TestMessageDefine.MessageTag2, 100);
        
        if(Input.GetKeyDown(KeyCode.F))
            MessageManager.Instance.RemoveMessageByTag(_tag);
            
            
            
    }
}