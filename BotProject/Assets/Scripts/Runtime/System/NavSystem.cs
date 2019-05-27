namespace GameRuntime
{
    using UnityEngine;
    
    using GameAI.Pathfinding.Core;

    public class NavSystem : MonoBehaviour
    {
        #region Properties
        private IPathProcessor m_PathProcessor;
        private PathReturnQueue m_ReturnQueue;
        #endregion

        #region Public_API
        public int ThreadCount;
        public AlgorithmType SearchType;
        public float MaxFrameTime;
        #endregion

        #region Unity_Callbacks
        private void Start()
        {
            m_ReturnQueue = new PathReturnQueue();

            IPathProcessor processor;
            if (SearchType == AlgorithmType.AStar)
                processor = new PathProcessor<PathNode>(ThreadCount, m_ReturnQueue);
            else if (SearchType == AlgorithmType.AStarWithJPS)
                processor = new PathProcessor<JPSPathNode>(ThreadCount, m_ReturnQueue);
            else
                throw new System.Exception("Undifined SearchType!");
            processor.SetSearchType(SearchType);
            processor.SetMaxFrameTime(MaxFrameTime);
            m_PathProcessor = processor;

            //Todo: Bake_Graph
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