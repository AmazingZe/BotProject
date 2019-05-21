namespace GameAI.Pathfinding
{
    using UnityEngine;

    using GameAI.Pathfinding.Grid;
    using GameAI.Pathfinding.Utils;

    using GameUtils;

    public class GridMap : Singleton<GridMap>, IMap
    {
        #region Properties
        private int NextGridNodeIndex;

        private Vector2Int m_GraphSize;
        private int m_GridSize;

        private float m_DeltaX;
        private float m_DeltaZ;

        private GraphTransform m_Transform;
        private TerrainScanner m_Scanner;

        private GridGraph[] m_Graphs;
        private int m_FirstFree;
        
        #endregion

        #region ISingleton
        private GridMap() { }
        public override void OnInit()
        {
            m_FirstFree = 0;
            NextGridNodeIndex = 0;

            m_Scanner = new TerrainScanner();
            m_Transform = new GraphTransform(Matrix4x4.identity);
        }
        #endregion

        #region Public_API
        public void Update(Vector3 center)
        {

        }
        #endregion
    }
}