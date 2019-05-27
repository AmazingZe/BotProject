namespace GameAI.Pathfinding.Core
{
    using System.Collections.Generic;

    public class PathNode : IPathNode
    {
        #region Properties
        private const int CostOffset = 28;
        private const int CostMask = (1 << CostOffset) - 1;

        private int m_Flag;
        private int g, h;
        private List<NavNode> m_Neighbors = new List<NavNode>();
        #endregion

        #region IPathNode_API
        public int G
        {
            get { return g; }
            set { g = value; }
        }
        public int H
        {
            get { return h; }
            set { h = value; }
        }
        public int F
        {
            get { return h + g; }
        }
        public int Cost
        {
            get { return (m_Flag & CostMask); }
            set { m_Flag = ((m_Flag & ~CostMask) | value); }
        }
        public IPathHandler Handler { get; set; }
        public NavNode Node { get; set; }
        public int PathID { get; set; }
        public IPathNode Parent { get; set; }
        public void UpdateG() { g = Parent.G + Cost; }

        public virtual void Reset()
        {
            g = 0;
            h = 0;
            PathID = 0;
            Parent = null;
            Node = null;
        }
        #endregion

        #region Public_Properties  
        public int m_HeapIndex = PathNodeHeap.NotInHeap;
        #endregion

        #region IHeapNode
        public int HeapIndex
        {
            get { return m_HeapIndex; }
            set { m_HeapIndex = value; }
        }
        public float Priority { get;set; }
        #endregion 

        public virtual void Open(Path path, IPathHandler pathHandler)
        {
            NavNode node = Node,
                    tmpNode;
            IPathHandler handler = Handler;
            node.GetNeighbor(m_Neighbors);
            for (int i = 0; i < m_Neighbors.Count; i++)
            {
                tmpNode = m_Neighbors[i];
                if (tmpNode == null) continue;
                IPathNode tmpPN = handler.GetPathnode(tmpNode);
                int cost = node.GetNeighborCost(i);
                if (PathID != tmpPN.PathID)
                {
                    tmpPN.Parent = this;
                    tmpPN.PathID = PathID;
                    tmpPN.Cost = cost;
                    tmpPN.H = path.CalculateHScore(tmpNode);
                    tmpPN.UpdateG();
                    handler.Heap.Enqueue(tmpPN, tmpPN.F);
                }
                else
                {
                    if (g + cost < tmpPN.G)
                    {
                        tmpPN.Parent = this;
                        tmpPN.Cost = cost;
                        tmpPN.UpdateG();
                        handler.Heap.Enqueue(tmpPN, tmpPN.F);
                    }
                }
            }
        }

        public static void Bake(IPathHandler handler, GridGraph graph)
        {
            //Todo: Deal with map
        }
    }
}