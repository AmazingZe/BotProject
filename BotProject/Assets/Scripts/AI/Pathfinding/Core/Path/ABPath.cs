namespace GameAI.Pathfinding.Core
{
    using UnityEngine;

    using GameUtils.Pool;

    public class ABPath : Path
    {
        #region Properties
        private Vector3 m_StartPos;
        private Vector3 m_EndPos;

        private NavNode m_StartNode;
        private NavNode m_EndNode;
        #endregion

        #region Public_API
        public static ABPath Construct(Vector3 start, Vector3 end, OnPathComplete callback = null)
        {
            var path = PathPool.GetPath<ABPath>();
            path.SetUp(start, end, callback);
            return path;
        }

        public PathNode CurNode;
        public PathNode PartialBestNode;
        public bool CalculatePartial = false;
        #endregion

        private void SetUp(Vector3 start, Vector3 end, OnPathComplete callback)
        {
            OnComplete = callback;
            UpdateStartEnd(start, end);
        }

        private void UpdateStartEnd(Vector3 start, Vector3 end)
        {
            m_StartPos = start;
            m_EndPos = end;
        }

        public override void Recycle()
        {
            OnComplete = null;
        }

        #region Path_API
        public override void Prepare()
        {
            //Todo: Calculate Start/End;
        }
        public override void InitPath()
        {
            PathNode startPNode = Handler.GetPathNode(m_StartNode.NodeIndex);
            startPNode.Node = m_StartNode;
            startPNode.PathID = Handler.PathID;
            startPNode.Parent = null;
            startPNode.Cost = 0;
            //startPNode.G
            //startPNode.H
            //Todo: Check if end == start

            if (CompleteState == PathCompleteState.Complete) return;

            //Todo: Open

            if (Handler.Heap.IsEmpty)
            {
                if (CalculatePartial)
                {
                    CompleteState = PathCompleteState.PartialComplete;
                    Trace(PartialBestNode);
                }
                else
                {

                }
                return;
            }

            CurNode = Handler.Heap.Dequeue();
        }
        public override void Process(AlgorithmType type)
        {
            if (type == AlgorithmType.AStar)
            {

            }
        }
        public override void CleanUP()
        {
            
        }
        #endregion
    }
}