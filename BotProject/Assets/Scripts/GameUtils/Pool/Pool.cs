namespace GameUtils
{
    using System.Collections.Generic;

    using GameUtils.ObjectPool;

    public class Pool<T> : Singleton<Pool<T>> where T : class, IPoolable, new()
    {
        #region Properties
        private T[] m_Pool;
        private int m_FirstFree;
        private int m_AllocatedNum;

        private HashSet<T> m_Checks;
        #endregion

        #region API
        public static void Recycle(ref T item)
        {
            if (ReferenceEquals(item, null)) return;

            Instance._Recycle(item);
            item.Recycle();
            item = null;
        }
        public static T Allocate()
        {
            return Instance._Allocate();
        }
        #endregion

        #region Singleton
        public override void OnInit()
        {
            m_FirstFree = 0;
            m_AllocatedNum = 0;

            m_Checks = new HashSet<T>();
        }
        public override void OnRelease()
        {
            m_Checks.Clear();

            base.OnRelease();
        }
        private Pool() { }
        #endregion

        #region Pool-Implementation
        private void _Recycle(T item)
        {
            var len = m_Pool.Length;
            if (!m_Checks.Add(item)) return;

            if (m_FirstFree >= len)
            {
                //Todo : ArrayPool
                var newArray = new T[len * 2];
                for (int i = 0; i < len; i++)
                    newArray[i] = m_Pool[i];
                m_Pool = newArray;
            }

            m_Pool[++m_FirstFree] = item;
        }
        private T _Allocate()
        {
            if (m_FirstFree < 0) return new T();
            else
            {
                var retMe = m_Pool[m_FirstFree--];
                m_Checks.Remove(retMe);
                return retMe;
            }
        }
        #endregion
    }
}