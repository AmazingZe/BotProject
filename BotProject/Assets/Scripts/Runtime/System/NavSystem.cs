namespace GameRuntime
{
    using UnityEngine;
    
    using GameAI.Pathfinding.Core;

    public class NavSystem : MonoBehaviour
    {
        private const string ConfigurePath = "ScriptableObjects/NavSystemConfigure";
        private static NavSystemConfigure Configure = null;

        #region Properties
        private int width, depth;
        private Vector3 Center;

        private GridGraph m_Graph;
        private IPathProcessor m_PathProcessor;
        private PathReturnQueue m_ReturnQueue;
        #endregion

        #region Public_API
        public int ThreadCount;
        public AlgorithmType SearchType;
        public float MaxFrameTime;
        #endregion

        #region Unity_Callbacks
        private void Start()
        {
            InitFromConfigure();

            //m_ReturnQueue = new PathReturnQueue();

            //IPathProcessor processor;
            //if (SearchType == AlgorithmType.AStar)
            //    processor = new PathProcessor<PathNode>(ThreadCount, m_ReturnQueue);
            //else if (SearchType == AlgorithmType.AStarWithJPS)
            //    processor = new PathProcessor<JPSPathNode>(ThreadCount, m_ReturnQueue);
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
        #endregion

        #region Public_API
        public void Bake()
        {
            m_Graph = GridGraph.CreateGraph(width, depth);
            bool[] walkbility = new bool[width * depth];

            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    RecalculateCell(x, z);
                }
            }

            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++) 
                    CalculateConnections(x, z); 
            }

            m_Graph.BakeGraph(walkbility);
        }
        #endregion

        private void RecalculateCell(int x, int z)
        {
            var node = m_Graph[z * width + x];
            bool walkable = false;



            node.Walkable = false;
        }
        private void CalculateConnections(int x, int z)
        {

        }

        private Vector3 GraphPointToWorld(int x, int z)
        {

        }
        private void InitFromConfigure()
        {
            if (Configure == null)
                Configure = Resources.Load<NavSystemConfigure>(ConfigurePath);

            width = Configure.Width;
            depth = Configure.Depth;
            Center = Configure.Center;
        }
    }
}