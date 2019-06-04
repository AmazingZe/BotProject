namespace GameUtils
{
    using System.Collections.Generic;

    public static class ListPool<T>
    {
        static readonly List<List<T>> pool = new List<List<T>>();

        static readonly List<List<T>> largePool = new List<List<T>>();
        static readonly HashSet<List<T>> inPool = new HashSet<List<T>>();

        const int MaxCapacitySearchLength = 8;
        const int LargeThreshold = 5000;
        const int MaxLargePoolSize = 8;

        public static List<T> Claim()
        {

            if (pool.Count > 0)
            {
                List<T> ls = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
                inPool.Remove(ls);
                return ls;
            }

            return new List<T>();
        }

        static int FindCandidate(List<List<T>> pool, int capacity)
        {
            List<T> list = null;
            int listIndex = -1;
            for (int i = 0; i < pool.Count && i < MaxCapacitySearchLength; i++)
            {
                // ith last item
                var candidate = pool[pool.Count - 1 - i];

                // Find the largest list that is not too large (arbitrary decision to try to prevent some memory bloat if the list was not just a temporary list).
                if ((list == null || candidate.Capacity > list.Capacity) && candidate.Capacity < capacity * 16)
                {
                    list = candidate;
                    listIndex = pool.Count - 1 - i;

                    if (list.Capacity >= capacity)
                        return listIndex;
                }
            }

            return listIndex;
        }

        public static List<T> Claim(int capacity)
        {
            var currentPool = pool;
            var listIndex = FindCandidate(pool, capacity);

            if (capacity > LargeThreshold)
            {
                var largeListIndex = FindCandidate(largePool, capacity);
                if (largeListIndex != -1)
                {
                    currentPool = largePool;
                    listIndex = largeListIndex;
                }
            }

            if (listIndex == -1)
            {
                return new List<T>(capacity);
            }
            else
            {
                var list = currentPool[listIndex];
                // Swap current item and last item to enable a more efficient removal
                inPool.Remove(list);
                currentPool[listIndex] = currentPool[currentPool.Count - 1];
                currentPool.RemoveAt(currentPool.Count - 1);
                return list;
            }
        }

        /// <summary>
        /// Makes sure the pool contains at least count pooled items with capacity size.
        /// This is good if you want to do all allocations at start.
        /// </summary>
        public static void Warmup(int count, int size)
        {
            var tmp = new List<T>[count];
            for (int i = 0; i < count; i++) tmp[i] = Claim(size);
            for (int i = 0; i < count; i++) Release(tmp[i]);
        }


        /// <summary>
        /// Releases a list and sets the variable to null.
        /// After the list has been released it should not be used anymore.
        ///
        /// \throws System.InvalidOperationException
        /// Releasing a list when it has already been released will cause an exception to be thrown.
        ///
        /// See: <see cref="Claim"/>
        /// </summary>
        public static void Release(ref List<T> list)
        {
            Release(list);
            list = null;
        }

        public static void Release(List<T> list)
        {
            if (list.Capacity > LargeThreshold)
            {
                largePool.Add(list);
                
                if (largePool.Count > MaxLargePoolSize) 
                    largePool.RemoveAt(0); 
            }
            else 
                pool.Add(list); 
        }

        public static void Clear()
        {
            inPool.Clear();
            pool.Clear();
        }

        /// <summary>Number of lists of this type in the pool</summary>
        public static int GetSize()
        {
            return pool.Count;
        }
    }
}