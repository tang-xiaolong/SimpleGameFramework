using System.Collections.Generic;
using LDataStruct;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestHeap : MonoBehaviour
{
    [Tooltip("堆的大小")] public int count = 8;
    private Heap<int> heap;
    [Tooltip("等待插入的元素")] public int waitInsertItem = 10;
    [Tooltip("等待删除的元素")] public int waitDeleteItem = 10;

    public string nunSequence =
        "398 400 398 400 400 398 398 400 400 400 400 398 398 400 398 400 400 400 400 400 400 400 400 400 400 398 398";

    [ContextMenu("根据大小创建最小堆")]
    public void CreateMinHeap()
    {
        heap = new Heap<int>(count, HeapType.MinHeap);
        for (int i = 0; i < count; i++)
            heap.Insert(Random.Range(1, 100));
        Debug.Log(heap);
    }

    [Space]
    [Header("暴力测试")]
    [Tooltip("要暴力测试的堆的类型")] public HeapType forceTestHeapType = HeapType.MinHeap;
    [Tooltip("暴力测试初始的堆数值数量")] public int forceTestOriginCount = 100;

    //暴力测试插入和删除后堆是否有效
    [ContextMenu("暴力测试堆算法有效性")]
    public void ForceTest()
    {
        List<int> list = new List<int>();
        //先随机生成指定数量的数字
        int testCount = forceTestOriginCount;
        for (int i = 0; i < testCount; i++)
            list.Add(Random.Range(1, 100));
        //将数字插入到堆中
        heap = new Heap<int>(testCount, forceTestHeapType);
        bool hasValid = true;

        void TestValid()
        {
            if (!heap.CheckHeapValid())
            {
                hasValid = false;
                Debug.LogError("堆不满足要求");
                Debug.LogError(heap);
            }
        }

        foreach (var item in list)
        {
            heap.Insert(item);
            TestValid();
        }

        //如果堆数量不为0
        while (!heap.IsEmpty())
        {
            //生成一个0-1的随机数，如果随机数大于0.35，则随机删除一个元素，并将其从列表中移除，否则随机生成一个数字插入到堆中
            if (Random.Range(0f, 1f) > 0.35f)
            {
                int index = Random.Range(0, list.Count);
                heap.Delete(list[index]);
                list.RemoveAt(index);
                TestValid();
            }
            else
            {
                int value = Random.Range(1, 100);
                heap.Insert(value);
                list.Add(value);
                TestValid();
            }
        }
        heap.Dispose();

        if (hasValid)
        {
            Debug.Log("堆算法有效");
        }
    }

    [ContextMenu("根据大小创建最大堆")]
    public void CreateMaxHeap()
    {
        heap = new Heap<int>(count, HeapType.MaxHeap);
        for (int i = 0; i < count; i++)
            heap.Insert(Random.Range(1, 100));
        Debug.Log(heap);
    }

    [ContextMenu("检测堆是否有效")]
    public void CheckHeapValid()
    {
        if (heap != null)
        {
            Debug.Log(heap.CheckHeapValid());
        }
    }

    [ContextMenu("插入元素")]
    public void InsertValue()
    {
        if (heap != null)
        {
            heap.Insert(waitInsertItem);
            Debug.Log(heap);
        }
    }

    [ContextMenu("删除堆首的元素")]
    public void DeleteValue()
    {
        if (heap != null && heap.Count > 0)
        {
            Debug.Log($"Delete:{heap.DeleteHead()}");
            Debug.Log(heap);
        }
    }

    [ContextMenu("删除等待删除的元素")]
    public void DeleteWaitValue()
    {
        if (heap != null && heap.Count > 0)
        {
            Debug.Log($"Delete:{heap.Delete(waitDeleteItem)}");
            Debug.Log(heap);
        }
    }

    [ContextMenu("打印堆")]
    public void PrintHeap()
    {
        if (heap != null && heap.Count > 0)
        {
            // heap.GetString();
            Debug.Log(heap);
        }
    }
}