namespace GameRuntime
{
    using UnityEngine;
    
    using GameAI.Pathfinding.Core;
    using GameUtils;

    public class NavSystem : MonoBehaviour
    {
        private const string ConfigurePath = "ScriptableObjects/NavSystemConfigure";
        private static NavSystemConfigure Configure = null;

        #region Properties
        private GridGraph m_Graph;
        private IPathProcessor m_PathProcessor;
        private PathReturnQueue m_ReturnQueue;
        #endregion

        #region Public_API
        public int ThreadCount;
        public AlgorithmType SearchType;
        public float MaxFrameTime;
        #endregion

        #region Debug_API
        public bool showSearchTree = true;
        public bool showMeshSurface = true;
        public bool showMeshOutline = true;
        public bool showNodeConnections = true;
        public IPathHandler debugHandler;
        public int debugPathID;
        public float debugFloor = 0f;
        public float debugRoof = 10f;

        private int lastRenderedFrame = -1;
        private RetainedGizmos gizmos = new RetainedGizmos();
        #endregion

        #region Unity_Callbacks
        private void Start()
        {
            InitFromConfigure();

            //m_ReturnQueue = new PathReturnQueue();

            //IPathProcessor processor;
            //if (SearchType == AlgorithmType.AStar)
            //    processor = new PathProcessor<PathNode>(ThreadCount, m_ReturnQueue, this);
            //else if (SearchType == AlgorithmType.AStarWithJPS)
            //    processor = new PathProcessor<JPSPathNode>(ThreadCount, m_ReturnQueue, this);
            //else
            //    throw new System.Exception("Undifined SearchType!");
            //processor.SetSearchType(SearchType);
            //processor.SetMaxFrameTime(MaxFrameTime);
            //m_PathProcessor = processor;

            //Todo: Bake_Graph
        }
        private void Update()
        {
            if (!Application.isPlaying || m_Graph == null) return;

            //m_PathProcessor.NonMultiThreadTick();
            //m_ReturnQueue.ReturnPaths(true);
        }
        private void OnDrawGizmos()
        {
            
        }
        private void OnApplicationQuit()
        {

        }
        #endregion

        private void InitFromConfigure()
        {
            if (Configure == null)
                Configure = Resources.Load<NavSystemConfigure>(ConfigurePath);

            m_Graph = Configure.gridGraph;
        }

        private void OnDrawGraphGizmos(GridGraph graph)
        {
            using (var helper = gizmos.GetSingleFrameGizmoHelper(this))
            {
                var bounds = new Bounds();
                int depth = m_Graph.Depth,
                    width = m_Graph.Width;
                bounds.SetMinMax(Vector3.zero, new Vector3(width, 0, depth));
                var m = m_Graph.UpdateTransform();
                helper.builder.DrawWireCube(m, bounds, Color.white);

                var color = new Color(1, 1, 1, 0.2f);
                for (int z = 0; z < depth; z++)
                {
                    helper.builder.DrawLine(m.Transform(new Vector3(0, 0, z)), m.Transform(new Vector3(width, 0, z)), color);
                }
                for (int x = 0; x < width; x++)
                {
                    helper.builder.DrawLine(m.Transform(new Vector3(x, 0, 0)), m.Transform(new Vector3(x, 0, depth)), color);
                }
            }

            const int chunkWidth = 1;
            GridNode[] allNodes = ArrayPool<GridNode>.Claim(chunkWidth * chunkWidth);
            for (int cx = m_Graph.Width / chunkWidth; cx >= 0; cx--)
            {
                for (int cz = m_Graph.Depth / chunkWidth; cz >= 0; cz--)
                {
                    var allNodesCount = m_Graph.GetNodesInRegion(new IntRect(cx * chunkWidth, cz * chunkWidth, (cx + 1) * chunkWidth - 1, (cz + 1) * chunkWidth - 1), allNodes);
                    var hasher = new RetainedGizmos.Hasher(this);
                    hasher.AddHash(showMeshOutline ? 1 : 0);
                    hasher.AddHash(showMeshSurface ? 1 : 0);
                    hasher.AddHash(showNodeConnections ? 1 : 0);

                    for (int i = 0; i < allNodesCount; i++)
                        hasher.HashNode(allNodes[i]);

                    if (!gizmos.Draw(hasher))
                    {
                        using (var helper = gizmos.GetGizmoHelper(this, hasher))
                        {
                            if (showNodeConnections)
                            {
                                for (int i = 0; i < allNodesCount; i++)
                                {
                                    if (allNodes[i].Walkable)
                                        helper.DrawConnections(allNodes[i]);
                                }
                            }
                        }
                    }
                }
            }

            ArrayPool<GridNode>.Release(ref allNodes);
        }
    }
}