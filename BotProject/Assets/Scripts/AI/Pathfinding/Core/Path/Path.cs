namespace GameAI.Pathfinding.Core
{
    using System.Collections.Generic;

    using GameUtils.Pool;

    public abstract class Path : IPoolable
    {
        #region Properties
        protected PathHandler Handler;
        private List<NavNode> m_Path;
        #endregion

        #region Public_Properties
        public int PathID;
        public PathNode CurNode;
        public Path Next;
        public OnPathComplete OnComplete;
        public PathState PipeLineState = PathState.Ready;
        public PathCompleteState CompleteState = PathCompleteState.NotCalculated;
        #endregion

        public void PrepareBase(PathHandler handler)
        {
            Handler = handler;
            Handler.Init(this);
        }
        public void AdvanceState(PathState pathState)
        {
            PipeLineState = (PathState)System.Math.Max((int)PipeLineState, (int)pathState);
        }
        public void Trace(PathNode endNode)
        {

        }

        public abstract void Recycle();

        public abstract void InitPath();
        public abstract void Prepare();
        public abstract void Process(AlgorithmType type);
        public abstract void CleanUP();

        public virtual void ReturnPath()
        {
            if (OnComplete != null)
                OnComplete(this);
        }
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
    public enum AlgorithmType
    {
        AStar,
        AStarWithJPS,
    }
}