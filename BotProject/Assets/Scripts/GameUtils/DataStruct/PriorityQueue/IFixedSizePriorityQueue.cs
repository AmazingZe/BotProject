namespace GameUtils.PriorityQueue
{
    using System;

    public interface IFixedSizePriorityQueue<TItem, in TPriority> : IPriorityQueue<TItem, TPriority>
        where TPriority : IComparable<TPriority>
    {
        void Size(int maxSize);
        
        int MaxSize { get; }
        void ResetNode(TItem item);
    }
}