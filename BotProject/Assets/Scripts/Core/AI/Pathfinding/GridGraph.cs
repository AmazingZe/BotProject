namespace GameAI.Pathfinding
{
    using System;
    using System.Collections;

    public class GridGraph : NavGraph
    {
        #region Properties
        public int Width;
        public int Depth;
        public float NodeSize;

        private GridNode[] m_Nodes;
        #endregion

        private GridGraph(int width, int depth)
        {
            Width = width;
            Depth = depth;
            m_Nodes = new GridNode[Width * Depth];
        }

        #region API
        public static GridGraph Create(int width, int depth)
        {
            return new GridGraph(width, depth);
        }

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
        //public override IEnumerable AsyncScan()
        //{
            
        //}
        #endregion
    }
}