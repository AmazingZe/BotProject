namespace GameAI.Pathfinding.Core
{
    using GameUtils.Heap;

    public class PathNode : IHeapNode
    {
        #region Properties
        private const int CostOffset = 28;
        private const int CostMask = (1 << CostOffset) - 1;

        private int m_Flag;
        private int g, h;
        #endregion

        #region Public_Properties
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

        public PathHandler Handler;
        public PathNode Parent;
        public NavNode Node;

        public int HeapIndex = PathNodeHeap.NotInHeap;
        public int PathID;
        #endregion

        #region IHeapNode
        public float Priority { get;set; }
        #endregion

        public void UpdateG() { g = Parent.g + Cost; }
        public void Reset()
        {

        }
    }
}