namespace GameAI.Pathfinding
{
    using UnityEngine;

    public abstract class NavNode
    {
        #region Properties
        private int m_Index;
        private bool m_Walkable;
        private Vector2 m_Pos;
        #endregion

        #region API
        public int NodeIndex
        {
            get { return m_Index; }
            set { m_Index = value; }
        }
        public Vector2 Position
        {
            get { return m_Pos; }
            set { m_Pos = value; }
        }
        public bool Walkable
        {
            get { return m_Walkable; }
            set { m_Walkable = false; }
        }
        #endregion
    }
}