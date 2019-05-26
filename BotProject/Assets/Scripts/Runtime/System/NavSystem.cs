namespace GameRuntime
{
    using UnityEngine;
    
    using GameAI.Pathfinding.Core;

    public class NavSystem : MonoBehaviour
    {
        #region Properties
        private PathProcessor m_PathProcessor;
        private PathReturnQueue m_ReturnQueue;
        #endregion

        #region Public_API
        public int ThreadCount;

        public float MaxFrameTime
        {
            get { return m_PathProcessor.MaxFrameTime; }
            set { m_PathProcessor.MaxFrameTime = value; }
        }
        #endregion

        #region Unity_Callbacks
        private void Start()
        {
            m_ReturnQueue = new PathReturnQueue();
            m_PathProcessor = new PathProcessor(ThreadCount, m_ReturnQueue);
        }
        private void Update()
        {
            if (!Application.isPlaying) return;

            m_PathProcessor.NonMultiThreadTick();
            m_ReturnQueue.ReturnPaths(true);
        }
        #endregion

        #region Public_API

        #endregion
    }
}