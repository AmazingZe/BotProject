namespace GameAI.Pathfinding.Core
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public class GridGraph : NavGraph
    {
        #region Static
        private static int GridGraphIndex = 1; 
        private static GridGraph[] GridGraphs = new GridGraph[0];
        public static GridGraph GetGridGraph(int graphIndex)
        {
            for(int i = 0; i < GridGraphs.Length; i++)
            {
                if (GridGraphs[i].m_GraphIndex == graphIndex)
                    return GridGraphs[i];
            }

            throw new IndexOutOfRangeException();
        }
        public static GridGraph CreateGraph(int width, int depth)
        {
            var graph = new GridGraph(width, depth);

            int oldLen = GridGraphs.Length;
            var newArray = new GridGraph[oldLen + 1];
            int i = 0;
            for (; i < oldLen; i++)
                newArray[i] = GridGraphs[i];
            newArray[i] = graph;
            GridGraphs = newArray;

            return graph;
        }
        #endregion

        #region Properties
        private int m_GraphIndex;
        private int FreeNodeIndex = 0;

        private int m_Width;
        private int m_Depth;
        private GridNode[] m_GridNodes;
        private int m_FirstFree;

        private int[] neighborIndexOffset;
        private int[] neighborNodeCost = new int[8]
        {
            1,1,1,1,
            1,1,1,1
        };

        public Vector3 Center = Vector3.zero;
        #endregion

        #region Public_Properties
        public int Width
        {
            get { return m_Width; }
        }
        public int Depth
        {
            get { return m_Depth; }
        }
        public GridNode this[int index]
        {
            get
            {
                CheckIndexValid(index);
                return m_GridNodes[index];
            }
        }
        #endregion

        private GridGraph(int width, int depth)
        {
            m_GraphIndex = GridGraphIndex++;
            m_Width = width;
            m_Depth = depth;

            m_GridNodes = new GridNode[width * depth];
            for (int i = 0; i < width * depth; i++)
            {
                var node = new GridNode();
                node.NodeIndex = FreeNodeIndex++;
                node.GraphIndex = m_GraphIndex;
                m_GridNodes[i] = node;
            }

            m_FirstFree = FreeNodeIndex;
            SetOffset();
        }

        #region Public_API
        public int GetNeighborCost(int dir) { return neighborNodeCost[dir]; }
        public void ResizeGraph(int width, int depth)
        {
            if (width == m_Depth && depth == m_Depth) return;

            int newSize = width * depth,
                oldSize = m_Width * m_Depth;

            m_Width = width;
            m_Depth = depth;

            if (newSize > oldSize)
            {
                if (m_FirstFree > newSize) 
                    m_FirstFree = newSize; 
                else
                {
                    var newArray = new GridNode[newSize];
                    int i = 0;
                    for (; i < oldSize; i++)
                        newArray[i] = m_GridNodes[i];
                    while (i < newSize)
                    {
                        newArray[i] = new GridNode();
                        newArray[i].NodeIndex = FreeNodeIndex++;
                        newArray[i].GraphIndex = m_GraphIndex;
                    }
                    m_FirstFree = newSize;
                }
            }else if (newSize < oldSize) 
                m_FirstFree = newSize; 

            SetOffset();
        }
        #endregion

        #region NavGraph
        public override void GetNodes(Action<NavNode> action)
        {

        }
        public override void OnDestroy()
        {

        }
        public override void GetNeighbor(int nodeIndex, List<NavNode> list)
        {
            CheckIndexValid(nodeIndex);

            list.Clear();

            for (int i = 0; i < 8; i++)
            {
                int neighborIndex = nodeIndex + neighborIndexOffset[i];
                if (neighborIndex < 0 || neighborIndex > Width * Depth)
                {
                    list.Add(null);
                    continue;
                }
                var node = m_GridNodes[neighborIndex];
                if (!node.Walkable) list.Add(null);
                else list.Add(node);
            }
        }
        public override void BakeGraph2Handler(IPathHandler handler)
        {
            if (handler.SearchType == AlgorithmType.AStar)
                PathNode.Bake(handler, this);
            else if (handler.SearchType == AlgorithmType.AStarWithJPS)
                JPSPathNode.Bake(handler, this);
        }
        public override void UpdateTransform()
        {
            
        }
        #endregion

        public void BakeGraph(bool[] data)
        {
            for (int i = 0; i < data.Length; i++)
                m_GridNodes[i].Walkable = data[i];
        }

        private void CheckIndexValid(int index)
        {
            if (index < 0 || index > m_FirstFree)
                throw new IndexOutOfRangeException();
        }
        private void SetOffset()
        {
            neighborIndexOffset = new int[8]{
                m_Width, 1, -m_Width, -1, 
                1+m_Width, 1-m_Width, -1-m_Width, m_Width-1
            };
        }
    }
}