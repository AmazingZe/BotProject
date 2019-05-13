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

        #region API
        public override void GetNodes(Action<NavNode> action)
        {
            
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