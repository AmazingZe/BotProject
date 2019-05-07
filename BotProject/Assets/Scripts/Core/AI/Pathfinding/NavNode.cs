namespace GameAI.Pathfinding
{
    using UnityEngine;

    public abstract class NavNode
    {
        #region Properties
        private int m_Index;
        private Vector3 m_Pos;
        #endregion

        #region API
        public int NodeIndex
        {
            get { return m_Index; }
            set { m_Index = value; }
        }
        public Vector3 Position
        {
            get { return m_Pos; }
            set { m_Pos = value; }
        }

        public abstract void AddConnection(NavNode node);
        public abstract void RemoveConnection(NavNode node);
        public abstract bool ContainsConnection(NavNode node);
        public abstract void Open();
        #endregion
    }
}