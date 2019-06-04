namespace GameAI.Pathfinding.Core
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    [Serializable]
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
        public static GridGraph CreateGraph(int width, int depth, int nodesize, Vector3 center)
        {
            var graph = new GridGraph(width, depth, nodesize);

            //int oldLen = GridGraphs.Length;
            //var newArray = new GridGraph[oldLen + 1];
            //int i = 0;
            //for (; i < oldLen; i++)
            //    newArray[i] = GridGraphs[i];
            //newArray[i] = graph;
            //GridGraphs = newArray;

            graph.Center = center;
            graph.UpdateTransform();

            return graph;
        }
        #endregion

        #region Properties
        private int m_GraphIndex;
        private int FreeNodeIndex = 0;
        private GraphTransform transform;
        private GraphCollision collision;
        private int m_Width;
        private int m_Depth;
        private int m_NodeSize;
        private GridNode[] m_GridNodes;
        private int m_FirstFree;

        private int[] neighborIndexOffset;
        private int[] neighborNodeCost = new int[8]
        {
            1,1,1,1,
            1,1,1,1
        };
        #endregion

        #region Public_Properties
        public float MaxSlope;
        public Vector3 Center = Vector3.zero;
        public Vector3 Rotation;
        public int Width
        {
            get { return m_Width; }
        }
        public int Depth
        {
            get { return m_Depth; }
        }
        public int NodeSize
        {
            get { return m_NodeSize; }
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

        private GridGraph(int width, int depth, int nodesize)
        {
            m_GraphIndex = GridGraphIndex++;
            m_Width = width;
            m_Depth = depth;
            m_NodeSize = nodesize;
            collision = new GraphCollision();
            transform = new GraphTransform(Matrix4x4.identity);
            collision.Initialize(transform, nodesize);

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
        public void RecalculateCell(int x, int z)
        {
            var node = m_GridNodes[x + z * m_Width];
            node.Position = GraphPointToWorld(x, z, 0);

            RaycastHit hit;
            bool walkable = false;
            Vector3 position = collision.CheckHeight(node.Position, out hit, out walkable);
            node.Position = position;

            //Todo: Penalty

            if (walkable)
            {
                if (hit.normal != Vector3.zero)
                {
                    float angle = Vector3.Dot(hit.normal.normalized, collision.up);
                    float cosAngle = Mathf.Cos(MaxSlope * Mathf.Deg2Rad);
                    if (angle < cosAngle)
                        walkable = false;
                }
            }

            node.Walkable = walkable && collision.Check(node.Position);
            //node.Walkable = walkable;

            int j = 1;
            //Todo: Walkable Erosion
        }
        #endregion

        #region NavGraph
        public override void GetNodes(Action<NavNode> action)
        {
            for (int i = 0; i < m_GridNodes.Length; i++)
            {
                action(m_GridNodes[i]);
            }
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
        public override GraphTransform UpdateTransform()
        {
            var boundMatrix = Matrix4x4.TRS(Center, Quaternion.Euler(Rotation), new Vector3(1, 1, 1));
            var m = Matrix4x4.TRS(boundMatrix.MultiplyPoint3x4(-new Vector3(m_Width * m_NodeSize, 0, m_Depth * m_NodeSize) * 0.5f),
                                                               Quaternion.Euler(Rotation),
                                                               new Vector3(1, 1, 1));
            transform = new GraphTransform(m);
            
            return transform;
        }
        #endregion

        public Vector3 GraphPointToWorld(int x, int z, float height)
        {
            return transform.Transform(new Vector3(x + 0.5f, height, z + 0.5f));
        }
        public int GetNodesInRegion(IntRect rect, GridNode[] buffer)
        {
            var gridRect = new IntRect(0, 0, m_Width - 1, m_Depth - 1);

            rect = IntRect.Intersection(rect, gridRect);

            if (m_GridNodes == null || !rect.IsValid() || m_GridNodes.Length != m_Width * m_Depth) return 0;

            if (buffer.Length < rect.Width * rect.Height) throw new ArgumentException("Buffer is too small");

            int counter = 0;
            for (int z = rect.ymin; z <= rect.ymax; z++, counter += rect.Width)
            {
                Array.Copy(m_GridNodes, z * Width + rect.xmin, buffer, counter, rect.Width);
            }

            return counter;
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