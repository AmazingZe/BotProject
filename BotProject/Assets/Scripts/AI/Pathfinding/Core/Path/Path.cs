namespace GameAI.Pathfinding.Core
{
    using System.Collections.Generic;

    public abstract class Path
    {
        #region Properties
        private PathHandler m_Handler;
        private List<NavNode> m_Path;
        private PathNode m_CurNode;
        #endregion

        #region Public_Properties
        public Path Next;
        public OnPathComplete OnComplete;
        public PathState PipeLineState = PathState.Ready;
        public PathCompleteState CompleteState = PathCompleteState.NotCalculated;
        #endregion
    }

    public delegate void OnPathComplete(Path path);
    public enum PathState
    {
        Ready,
        PathQueue,
        Processing,
        ReturnQueue,
        Returned,
    }
    public enum PathCompleteState
    {
        NotCalculated,
        Complete,
        PartialComplete,
        Error
    }
}