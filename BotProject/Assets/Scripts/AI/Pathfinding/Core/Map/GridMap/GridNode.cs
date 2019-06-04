namespace GameAI.Pathfinding.Core
{
    using UnityEngine;

    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GridNode : NavNode
    {
        #region Properties
        private const int WalkableOffset = 0;
        private const int WalkableMask = 1 << WalkableOffset;

        private int m_Flags;
        private Vector3 m_Pos;
        #endregion

        #region Public_Properties
        public override Vector3 Position
        {
            get { return m_Pos; }
            set { m_Pos = value; }
        }
        public override bool Walkable
        {
            get { return (m_Flags & WalkableMask) == 1; }
            set { m_Flags = (m_Flags & ~WalkableMask) | (value ? 1 : 0); }
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
        public override void GetConnection(Action<NavNode> action)
        {
            GridGraph graph = GridGraph.GetGridGraph(GraphIndex);
            List<NavNode> nodes = new List<NavNode>();
            graph.GetNeighbor(NodeIndex, nodes);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] == null) continue;
                action(nodes[i]);
            }
        }
        #endregion

        public override int GetGizmosHashCode()
        {
            var hash = base.GetGizmosHashCode();

            m_Flags ^= 19 * m_Flags;

            return hash;
        }
    }
}