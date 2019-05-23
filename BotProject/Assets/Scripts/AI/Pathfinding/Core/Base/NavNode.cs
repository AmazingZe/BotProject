namespace GameAI.Pathfinding.Core
{
    public abstract class NavNode
    {
        #region Properties
        private const int NodexIndexOffset = 0;
        private const int NodeIndexMask = 0xFFFF << NodexIndexOffset;
        private const int GraphIndexOffset = 16;
        private const int GraphIndexMask = 0xFF << GraphIndexOffset;

        private int m_Index;
        #endregion

        #region Public_Properties
        public int NodeIndex
        {
            get { return m_Index & NodeIndexMask; }
            set { m_Index = (m_Index & ~NodeIndexMask) | (value << NodeIndexMask); }
        }
        public int GraphIndex
        {
            get { return m_Index & GraphIndexMask; }
            set { m_Index = (m_Index & ~GraphIndexMask) | (value << GraphIndexMask); }
        }
        #endregion
    }
}