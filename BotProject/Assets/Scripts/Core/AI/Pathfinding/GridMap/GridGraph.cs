namespace GameAI.Pathfinding.Grid
{
    using System;
    using System.Collections;

    using UnityEngine;

    public class GridGraph : NavGraph
    {
        #region Properties
        public int Width;
        public int Depth;
        public float NodeSize;

        private GridNode[] m_Nodes;

        private Rect m_Bound;

        public GridNode this[int index]
        {
            get
            {
                CheckIndexValid(index);
                return m_Nodes[index];
            }
            set
            {
                CheckIndexValid(index);
                m_Nodes[index] = value;
            }
        }
        #endregion

        public GridGraph(int width, int depth)
        {
            Width = width;
            Depth = depth;
            m_Nodes = new GridNode[Width * Depth];
            for (int i = 0; i < m_Nodes.Length; i++)
                m_Nodes[i] = new GridNode();
        }

        #region API
        public override void GetNodes(Action<NavNode> action)
        {
            for (int i = 0; i < m_Nodes.Length; i++)
                action(m_Nodes[i]);
        }
        #endregion

        #region INavGraph
        public override void OnDestroy()
        {
            
        }
        #endregion

        private void CheckIndexValid(int index)
        {
            if (index >= m_Nodes.Length || index < 0)
                throw new IndexOutOfRangeException();
        }
    }
}