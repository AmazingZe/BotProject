namespace GameAI.Pathfinding
{
    public abstract class NavNode
    {
        #region Properties
        private int m_Index;
        #endregion

        #region API
        public int NodeIndex
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        public abstract void AddConnection(NavNode node);
        public abstract void RemoveConnection(NavNode node);
        public abstract bool ContainsConnection(NavNode node);
        public abstract void Open();
        #endregion
    }
}