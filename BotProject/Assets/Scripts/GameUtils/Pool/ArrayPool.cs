namespace GameUtils
{
    using System.Collections.Generic;

    public static class ArrayPool<T>
    {
        const int MaximumExactArrayLength = 256;

        static readonly Stack<T[]>[] pool = new Stack<T[]>[31];
        static readonly Stack<T[]>[] exactPool = new Stack<T[]>[MaximumExactArrayLength + 1];
        static readonly HashSet<T[]> inPool = new HashSet<T[]>();

        public static T[] Claim(int minimumLength)
        {
            if (minimumLength <= 0)
            {
                return ClaimWithExactLength(0);
            }

            int bucketIndex = 0;
            while ((1 << bucketIndex) < minimumLength && bucketIndex < 30)
            {
                bucketIndex++;
            }

            if (bucketIndex == 30)
                throw new System.ArgumentException("Too high minimum length");

            lock (pool)
            {
                if (pool[bucketIndex] == null)
                {
                    pool[bucketIndex] = new Stack<T[]>();
                }

                if (pool[bucketIndex].Count > 0)
                {
                    var array = pool[bucketIndex].Pop();
                    inPool.Remove(array);
                    return array;
                }
            }
            return new T[1 << bucketIndex];
        }

        public static T[] ClaimWithExactLength(int length)
        {
            bool isPowerOfTwo = length != 0 && (length & (length - 1)) == 0;
            if (isPowerOfTwo)
            {
                // Will return the correct array length
                return Claim(length);
            }

            if (length <= MaximumExactArrayLength)
            {
                lock (pool)
                {
                    Stack<T[]> stack = exactPool[length];
                    if (stack != null && stack.Count > 0)
                    {
                        var array = stack.Pop();
                        inPool.Remove(array);
                        return array;
                    }
                }
            }
            return new T[length];
        }

        public static void Release(ref T[] array, bool allowNonPowerOfTwo = false)
        {
            if (array == null) return;
            if (array.GetType() != typeof(T[]))
            {
                throw new System.ArgumentException("Expected array type " + typeof(T[]).Name + " but found " + array.GetType().Name + "\nAre you using the correct generic class?\n");
            }

            bool isPowerOfTwo = array.Length != 0 && (array.Length & (array.Length - 1)) == 0;
            if (!isPowerOfTwo && !allowNonPowerOfTwo && array.Length != 0) throw new System.ArgumentException("Length is not a power of 2");

            lock (pool)
            {
                if (isPowerOfTwo)
                {
                    int bucketIndex = 0;
                    while ((1 << bucketIndex) < array.Length && bucketIndex < 30)
                    {
                        bucketIndex++;
                    }

                    if (pool[bucketIndex] == null)
                    {
                        pool[bucketIndex] = new Stack<T[]>();
                    }

                    pool[bucketIndex].Push(array);
                }
                else if (array.Length <= MaximumExactArrayLength)
                {
                    Stack<T[]> stack = exactPool[array.Length];
                    if (stack == null) stack = exactPool[array.Length] = new Stack<T[]>();
                    stack.Push(array);
                }
            }
            array = null;
        }
    }

    /// <summary>Extension methods for List<T></summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Identical to ToArray but it uses ArrayPool<T> to avoid allocations if possible.
        ///
        /// Use with caution as pooling too many arrays with different lengths that
        /// are rarely being reused will lead to an effective memory leak.
        /// </summary>
        public static T[] ToArrayFromPool<T>(this List<T> list)
        {
            var arr = ArrayPool<T>.ClaimWithExactLength(list.Count);

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = list[i];
            }
            return arr;
        }

        /// <summary>
        /// Clear a list faster than List<T>.Clear.
        /// It turns out that the List<T>.Clear method will clear all elements in the underlaying array
        /// not just the ones up to Count. If the list only has a few elements, but the capacity
        /// is huge, this can cause performance problems. Using the RemoveRange method to remove
        /// all elements in the list does not have this problem, however it is implemented in a
        /// stupid way, so it will clear the elements twice (completely unnecessarily) so it will
        /// only be faster than using the Clear method if the number of elements in the list is
        /// less than half of the capacity of the list.
        ///
        /// Hopefully this method can be removed when Unity upgrades to a newer version of Mono.
        /// </summary>
        public static void ClearFast<T>(this List<T> list)
        {
            if (list.Count * 2 < list.Capacity)
            {
                list.RemoveRange(0, list.Count);
            }
            else
            {
                list.Clear();
            }
        }
    }
}