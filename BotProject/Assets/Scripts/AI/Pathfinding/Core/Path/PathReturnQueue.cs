namespace GameAI.Pathfinding.Core
{
    using System.Collections.Generic;

    public class PathReturnQueue
    {
        #region Properties
        private Queue<Path> m_PathReturnQueue = new Queue<Path>();
        private object m_Lock;
        #endregion

        #region Public_API
        public void Enqueue(Path path)
        {
            lock (m_Lock)
            {
                m_PathReturnQueue.Enqueue(path);
            }
        }
        public void ReturnPaths(bool timeSlice)
        {
            long targetTick = timeSlice ? System.DateTime.UtcNow.Ticks + 1 * 10000 : 0;
            int count = 0;

            while (true)
            {
                Path path;
                lock (m_Lock)
                {
                    if (m_PathReturnQueue.Count == 0) break;
                    path = m_PathReturnQueue.Dequeue();
                }

                path.ReturnPath();
                path.AdvanceState(PathState.Returned);

                count++;

                if (count > 5 && timeSlice)
                {
                    count = 0;
                    if (System.DateTime.UtcNow.Ticks >= targetTick)
                        return;
                }
            }
        }
        #endregion
    }
}