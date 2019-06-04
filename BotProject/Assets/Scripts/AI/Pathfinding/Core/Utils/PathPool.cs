namespace GameAI.Pathfinding.Core
{
    using System;
    using System.Collections.Generic;

    public class PathPool
    {
        #region Properties
        private static readonly Dictionary<Type, Stack<Path>> m_Pool = new Dictionary<Type, Stack<Path>>();
        private static readonly Dictionary<Type, int> createdNum = new Dictionary<Type, int>();
        #endregion

        #region Public_API
        public static void Recycle<T>(T path) where T : Path
        {
            if (path == null) return;

            var type = typeof(T);
            path.Recycle();
            m_Pool[type].Push(path);
            path = null;
        }
        public static T GetPath<T>() where T : Path, new()
        {
            T retMe;
            Stack<Path> pool;
            var type = typeof(T);

            if (!m_Pool.TryGetValue(typeof(T), out pool))
            {
                retMe = new T();
                m_Pool[type] = new Stack<Path>();
                createdNum[type] = 0;
            }
            else
            {
                if (pool.Count == 0)
                    retMe = new T();
                else
                    retMe = pool.Pop() as T;
            }
            
            createdNum[type]++;
            return retMe;
        }
        #endregion
    }
}