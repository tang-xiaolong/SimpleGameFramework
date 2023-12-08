using System;

namespace LDataStruct
{
    public class MaxHeap<T> : Heap<T> where T : IComparable
    {
        public MaxHeap() : base(10, HeapType.MaxHeap)
        {
        }
    }
}