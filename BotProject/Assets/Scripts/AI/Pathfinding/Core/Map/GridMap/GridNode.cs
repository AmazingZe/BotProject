namespace GameAI.Pathfinding.Core
{
    using UnityEngine;

    using System;
    using System.Collections.Generic;

    public class GridNode : NavNode
    {
        #region Properties
        private const int WalkableOffset = 0;
        private const int WalkableMask = 1 << WalkableOffset;

        private int m_Flags;
        private Vector2 m_Pos;
        #endregion

        #region Public_Properties
        public override Vector3 Position
        {
            get { return m_Pos; }
            set { m_Pos = value; }
        }
        public bool Walkable
        {
            get { return (m_Flags & WalkableMask) == 1; }
            set { m_Flags = (m_Flags & ~WalkableMask) | (value ? 1 : 0) << WalkableMask; }
        }
        #endregion

        #region Graph_API
        public override void GetNodes(Action<NavNode> action)
        {
            GridGraph graph = GridGraph.GetGridGraph(GraphIndex);
            graph.GetNodes(action);
        }
        public override void GetNeighbor(List<NavNode> list)
        {
            GridGraph graph = GridGraph.GetGridGraph(GraphIndex);
            graph.GetNeighbor(NodeIndex, list);
        }
        public override int GetNeighborCost(int dir)
        {
            GridGraph graph = GridGraph.GetGridGraph(GraphIndex);
            return graph.GetNeighborCost(dir);
        }
        #endregion
    }
}