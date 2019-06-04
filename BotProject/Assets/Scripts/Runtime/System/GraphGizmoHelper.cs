namespace GameRuntime
{
    using UnityEngine;

    using System;

    using GameUtils.Pool;
    using GameAI.Pathfinding.Core;

    using GameUtils;

    public class GraphGizmoHelper : IPoolable, IDisposable
    {
        public RetainedGizmos.Hasher hasher { get; private set; }
        public RetainedGizmos.Builder builder { get; private set; }

        private readonly System.Action<NavNode> drawConnection;

        private RetainedGizmos gizmos;
        private IPathHandler debugData;
        private int debugPathID;
        private bool showSearchTree;
        private Vector3 drawConnectionStart;
        private Color drawConnectionColor;

        float debugFloor;
        float debugRoof;

        public GraphGizmoHelper() { drawConnection = DrawConnections; }
        public void Init(NavSystem active, RetainedGizmos.Hasher hasher, RetainedGizmos gizmos)
        {
            if (active != null)
            {
                debugData = active.debugHandler;
                debugPathID = active.debugPathID;
                debugFloor = active.debugFloor;
                debugRoof = active.debugRoof;
                showSearchTree = active.showSearchTree && debugData != null;
            }
            this.gizmos = gizmos;
            this.hasher = hasher;
            builder = Pool<RetainedGizmos.Builder>.Allocate();
        }

        public void DrawConnections(NavNode node)
        {
            if (showSearchTree)
            {
                if (InSearchTree(node, debugData, debugPathID))
                {
                    var pnode = debugData.GetPathnode(node);
                    if (pnode.Parent != null)
                        builder.DrawLine(node.Position,
                                         debugData.GetPathnode(node).Parent.Node.Position,
                                         NodeColor(node));
                }
            }
            else
            {
                drawConnectionColor = NodeColor(node);
                drawConnectionStart = node.Position;
                node.GetConnection(drawConnection);
            }
        }

        public Color NodeColor(NavNode node)
        {
            if (showSearchTree && !InSearchTree(node, debugData, debugPathID)) return Color.clear;

            Color color;

            if (node.Walkable)
            {
                if (debugData == null)
                    color = AStarColor.SolidColor;
                else
                {
                    var pathNode = debugData.GetPathnode(node);
                    float value = pathNode.G;
                    color = Color.Lerp(AStarColor.ConnectionLowLerp, AStarColor.ConnectionHighLerp, (value - debugFloor) / (debugRoof - debugFloor));
                }
            }
            else
            {
                color = AStarColor.UnwalkableNode;
            }

            return color;
        }

        public void DrawWireTriangles(Vector3[] vertices, Color[] colors, int numTriangles)
        {
            for (int i = 0; i < numTriangles; i++)
            {
                DrawWireTriangle(vertices[i * 3 + 0], vertices[i * 3 + 1], vertices[i * 3 + 2], colors[i * 3 + 0]);
            }
        }
        public void DrawWireTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
        {
            builder.DrawLine(a, b, color);
            builder.DrawLine(b, c, color);
            builder.DrawLine(c, a, color);
        }
        public void DrawTriangles(Vector3[] vertices, Color[] colors, int numTriangles)
        {
            var triangles = ListPool<int>.Claim(numTriangles);

            for (int i = 0; i < numTriangles * 3; i++) triangles.Add(i);
            builder.DrawMesh(gizmos, vertices, triangles, colors);
            ListPool<int>.Release(ref triangles);
        }

        public static bool InSearchTree(NavNode node, IPathHandler handler, int pathID)
        {
            return handler.GetPathnode(node).PathID == pathID;
        }

        public void Recycle()
        {
            var bld = builder;
            Pool<RetainedGizmos.Builder>.Recycle(ref bld);
            builder = null;
            debugData = null;
        }

        public void Submit()
        {
            builder.Submit(gizmos, hasher);
        }
        void System.IDisposable.Dispose()
        {
            var tmp = this;

            Submit();
            Pool<GraphGizmoHelper>.Recycle(ref tmp);
        }
    }
}