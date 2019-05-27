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

        private int m_SearchIndex;
        #endregion

        #region Public_API
        public static ABPath Construct(Vector3 start, Vector3 end, OnPathComplete callback = null)
        {
            var path = PathPool.GetPath<ABPath>();
            path.SetUp(start, end, callback);
            return path;
        }

        public IPathNode CurNode;
        public IPathNode PartialBestNode;
        public bool CalculatePartial = false;
        #endregion

        private void SetUp(Vector3 start, Vector3 end, OnPathComplete callback)
        {            
            OnComplete = callback;
            UpdateStartEnd(start, end);

            m_SearchIndex = 0;
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
            //Setup Startnode's Info
            IPathNode startPNode = Handler.GetPathnode(m_StartNode);
            startPNode.Node = m_StartNode;
            startPNode.PathID = Handler.PathID;
            startPNode.Parent = null;
            startPNode.Cost = 0;
            startPNode.G = 0;
            startPNode.H = CalculateHScore(m_StartNode);

            startPNode.Open(this, Handler);
            m_SearchIndex++;

            PartialBestNode = startPNode;

            if (Handler.Heap.IsEmpty)
            {
                if (CalculatePartial)
                {
                    CompleteState = PathCompleteState.PartialComplete;
                    Trace(PartialBestNode);
                }
                else
                {
                    //Todo: Error
                }
            }

            CurNode = Handler.Heap.Dequeue();
        }
        public override void Process(long targetTick)
        {
            int counter = 0;

            while (CompleteState == PathCompleteState.NotCalculated)
            {
                m_SearchIndex++;
                var heap = Handler.Heap;

                if (PartialBestNode.H > CurNode.H)
                    PartialBestNode = CurNode;

                CurNode.Open(this, Handler);

                if (heap.IsEmpty)
                {
                    if (CalculatePartial && PartialBestNode != null)
                    {
                        CompleteState = PathCompleteState.PartialComplete;
                        Trace(PartialBestNode);
                    }
                    else
                    {
                        //Todo: Error
                    }
                }

                CurNode = heap.Dequeue();

                if (counter > 500)
                {
                    if (System.DateTime.UtcNow.Ticks >= targetTick)
                        return;
                    counter = 0;
                    if (m_SearchIndex > 1000000)
                        throw new System.Exception("Probable infinite loop. Over 1,000,000 nodes searched");
                }

                counter++;
            }

            if (CompleteState == PathCompleteState.Complete)
                Trace(CurNode);
            else if (CompleteState == PathCompleteState.PartialComplete)
                Trace(PartialBestNode);
        }
        public override void CleanUp()
        {
            //Todo: Clear Start/End Mark
        }
        #endregion
    }
}