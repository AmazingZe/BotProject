namespace GameUtils.PriorityQueue
{
    using System;
    using System.Collections.Generic;

    public interface IPriorityQueue<TItem, in TPriority> : IEnumerable<TItem>
        where TPriority : IComparable<TPriority>
    {
        void Enqueue(TItem item, TPriority priority);
        void Dequeue();
        void Clear();
        bool Contains(TItem item);
        void Remove(TItem item);
        void UpdatePriority(TItem item, TPriority priority);

        TItem First { get; }
        int Count { get; }
    }
}