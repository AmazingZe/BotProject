namespace GameAI.Pathfinding.Core
{
    using GameUtils.Heap;

    public class PathNode : IHeapNode
    {
        #region Properties
        private const int CostOffset = 28;
        private const uint CostMask = (1 << CostOffset) - 1;

        private ushort m_PathID;

        private uint m_Flag;
        private uint g, h;
        #endregion

        #region Public_Properties
        public uint G
        {
            get { return g; }
            set { g = value; }
        }
        public uint H
        {
            get { return h; }
            set { h = value; }
        }
        public uint F
        {
            get { return h + g; }
        }
        public uint Cost
        {
            get { return (m_Flag & CostMask); }
            set { m_Flag = ((m_Flag & ~CostMask) | value); }
        }

        public PathNode Parent;
        public NavNode Node;

        public int HeapIndex = PathNodeHeap.NotInHeap;
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