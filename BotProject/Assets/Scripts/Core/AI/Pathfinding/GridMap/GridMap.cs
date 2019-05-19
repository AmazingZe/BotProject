namespace GameAI.Pathfinding
{
    using UnityEngine;

    using GameAI.Pathfinding.Grid;
    using GameAI.Pathfinding.Utils;

    using GameUtils;

    public class GridMap : Singleton<GridMap>, IMap
    {
        #region Properties
        public Vector2Int GraphSize;
        public int GridSize;


        private static int NextGridNodeIndex;

        private GridGraph[] m_Graphs;
        private int m_FirstFree;

        private GraphTransform m_Transform;
        private TerrainScanner m_Scanner;
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
        public static GridGraph CreateMap(int width, int depth)
        {
            var newGraph = new GridGraph(width, depth);
            var oldGraphs = Instance.m_Graphs;
            int oldLen = oldGraphs.Length;
            var newGraphs = new GridGraph[oldLen + 1];

            int i = 0;
            for (; i < oldLen; i++)
                newGraphs[i] = oldGraphs[i];
            newGraphs[i] = newGraph;
            oldGraphs = newGraphs;

            return newGraph;
        }
        public static void RemoveMap(int graphIndex)
        {
            ChechIndexValid(graphIndex);

            var graphs = Instance.m_Graphs;
            int size = graphs.Length;
            for (int i = graphIndex; i < size - 1; i++)
                graphs[i] = graphs[i + 1];
        } 
        
        public void ScanAsync(Vector2 orientPos)
        {
            ScanInternal(Mathf.RoundToInt(orientPos.x), Mathf.RoundToInt(orientPos.y) );
        }
        #endregion

        private static void ChechIndexValid(int graphIndex)
        {
            if (Instance.m_Graphs.Length <= graphIndex || graphIndex < 0)
                throw new System.IndexOutOfRangeException();
        }

        private void ScanInternal(int x, int y)
        {
            int width = GraphSize.x / GridSize;
            int depth = GraphSize.y / GridSize;

            var graph = CreateMap(width, depth);

            GridNode node;

            #region Initialize
            for (int i = 0; i < depth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    node = graph[i * width + j];
                    node.Position = new Vector2(x + GridSize * j + GridSize / 2,
                                                y + GridSize * i + GridSize / 2);
                }
            }
            #endregion

            m_Scanner.Initialize(m_Transform);
            for (int i = 0; i < depth; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    RaycastHit hit;
                    bool walkAble;
                    node = graph[i * width + j];
                    Vector3 worldPos = new Vector3(node.Position.x, 0, node.Position.y);
                    var position = m_Scanner.CheckHeight(worldPos, out hit, out walkAble);

                    if (walkAble)
                    {

                    }
                }
            }
        }
    }
}