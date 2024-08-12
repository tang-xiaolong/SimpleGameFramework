using UnityEngine;
using UnityEngine.Profiling;

namespace Game.MessageSystem.Test
{
    class TestDispatchClass
    {
        public int a;
        public string b;
    }
    public class TestDispatch : MonoBehaviour
    {
        public int EventCount = 100000;
        TestDispatchClass testDispatchClass = new TestDispatchClass(){ a = 1, b = "test"};
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Profiler.BeginSample("GameEvent1001");
                for (int i = 0; i < EventCount; i++)
                {
                    // DDDDDTest();
                    MessageManager.Instance.Send("1001", 1);
                }
                Profiler.EndSample();
                Debug.Log($"GameEvent1001 {EventCount} times");
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Profiler.BeginSample("GameEvent1002");
                for (int i = 0; i < EventCount; i++)
                {
                    MessageManager.Instance.Send("1002", testDispatchClass);
                }
                Profiler.EndSample();
                Debug.Log($"GameEvent1002 {EventCount} times");
            }
        }
    }
}