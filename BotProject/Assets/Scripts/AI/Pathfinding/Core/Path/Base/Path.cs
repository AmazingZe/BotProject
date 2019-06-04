namespace GameAI.Pathfinding.Core
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameUtils.Pool;

    public abstract class Path : IPoolable
    {
        #region Properties
        private const int HeuristicScale = 1;

        public IPathHandler Handler;
        private List<NavNode> m_Path;

        private Vector3 m_HTarget;
        #endregion

        #region Public_Properties
        public int PathID;
        public PathNode CurNode;
        public Path Next;
        public OnPathComplete OnComplete;
        public PathState PipeLineState = PathState.Ready;
        public PathCompleteState CompleteState = PathCompleteState.NotCalculated;
        #endregion

        public void PrepareBase(IPathHandler handler)
        {
            Handler = handler;
            Handler.Init(this);
        }
        public void AdvanceState(PathState pathState)
        {
            PipeLineState = (PathState)System.Math.Max((int)PipeLineState, (int)pathState);
        }
        public void Trace(IPathNode endNode)
        {

        }

        #region Abstracts
        public abstract void Recycle();

        public abstract void InitPath();
        public abstract void Prepare();
        public abstract void Process(long targetTick);
        public abstract void CleanUp();
        #endregion

        public virtual void ReturnPath()
        {
            if (OnComplete != null)
                OnComplete(this);
        }

        public int CalculateHScore(NavNode node)
        {
            int retMe = 0;

            switch (Handler.HeuristicType)
            {
                case Heuristic.Manhattan:
                    retMe = (int)System.Math.Round((m_HTarget - node.Position).magnitude) * HeuristicScale;
                    break;
                case Heuristic.Euclidean:
                    Vector3 nodePosition = node.Position;
                    retMe = (int)(System.Math.Abs(nodePosition.x - nodePosition.x) +
                                  System.Math.Abs(nodePosition.y - nodePosition.y) +
                                  System.Math.Abs(nodePosition.z - nodePosition.z));
                    break;
                case Heuristic.DiagonalManhattan:
                    Vector3 posOffset = m_HTarget - node.Position;
                    posOffset.x = System.Math.Abs(posOffset.x);
                    posOffset.y = System.Math.Abs(posOffset.y);
                    posOffset.z = System.Math.Abs(posOffset.z);
                    int dis1 = (int)System.Math.Round(System.Math.Min(posOffset.x, posOffset.z)),
                        dis2 = (int)System.Math.Round(System.Math.Max(posOffset.x, posOffset.z));
                    retMe = (((14 * dis1) / 10) + (dis2 - dis1) + (int)posOffset.y) * HeuristicScale;
                    break;
                case Heuristic.NotUse:

                    break;
            }

            return retMe;
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
    public enum Heuristic
    {
        Manhattan,
        Euclidean,
        DiagonalManhattan,
        NotUse,
    }
}