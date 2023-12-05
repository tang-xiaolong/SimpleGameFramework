using System.Collections.Generic;
using UnityEngine;

public class TestMessage : MonoBehaviour
{
    [SerializeField] private int loopCount = 100000;

    void Start()
    {
        PoolModule.ObjectPoolFactory.Instance.GetItem<List<int>>();
        MessageManager.Instance.Register<int>(MessageDefine.TEST_MESSAGE, TestTMessage);
    }

    private void OnDestroy()
    {
        MessageManager.Instance.Remove<int>(MessageDefine.TEST_MESSAGE, TestTMessage);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 0; i < loopCount; i++)
            {
                MessageManager.Instance.Send<int>(MessageDefine.TEST_MESSAGE, 1);
            }
        }
    }

    void TestTMessage(int data)
    {
        int i = data + 1;
    }
}