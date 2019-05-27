namespace GameAI.Pathfinding.Core
{
    public class PathHandler<T> : IPathHandler
                                  where T : IPathNode, new()
    {
        #region Properties
        private int m_ThreadID;
        private int m_ThreadCount;

        private int m_PathID;

        private T[] m_Nodes = new T[0];
        private PathNodeHeap m_Heap;
        private Heuristic m_HeuristicType;
        private AlgorithmType m_AlgorithmType;
        #endregion

        public PathHandler(int thread, int threadCount)
        {
            m_ThreadID = thread;
            m_ThreadCount = threadCount;

            m_Heap = new PathNodeHeap(128);
        }

        #region IPathHandler
        public PathNodeHeap Heap
        {
            get { return m_Heap; }
        }
        public int PathID
        {
            get { return m_PathID; }
        }
        public Heuristic HeuristicType
        {
            get { return m_HeuristicType; }
            set { m_HeuristicType = value; }
        }
        public AlgorithmType SearchType
        {
            get { return m_AlgorithmType; }
            set { m_AlgorithmType = value; }
        }

        public IPathNode GetPathnode(NavNode node)
        {
            int nodeIndex = node.NodeIndex;
            return m_Nodes[nodeIndex];
        }
        public void Init(Path path)
        {
            m_PathID = path.PathID;
            Heap.Clear();
        }
        public void InitializeNode(NavNode node)
        {
            int index = node.NodeIndex;

            if (index >= m_Nodes.Length)
            {
                T[] newArray = new T[System.Math.Max(128, m_Nodes.Length * 2)];
                m_Nodes.CopyTo(newArray, 0);
                for (int i = m_Nodes.Length; i < newArray.Length; i++)
                {
                    newArray[i] = new T();
                    newArray[i].Handler = this;
                }
                m_Nodes = newArray;
            }

            m_Nodes[index].Node = node;
        }
        public void ClearNode(NavNode node)
        {
            T pn = GetPathNode(node.NodeIndex);
            pn.Reset();
        }
        public void SetNodeParam(System.Action<IPathNode> action)
        {
            for (int i = 0; i < m_Nodes.Length; i++)
                action(m_Nodes[i]);
        }
        #endregion

        #region Public_API
        public T GetPathNode(int index) { return m_Nodes[index]; }
        #endregion
    }
}