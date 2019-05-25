namespace GameAI.Pathfinding.Core
{
    public class PathHandler
    {
        #region Properties
        private int m_ThreadID;
        private int m_ThreadCount;

        private int m_PathID;

        private PathNode[] m_Nodes = new PathNode[0];
        #endregion

        #region Public_Properties
        public PathNodeHeap Heap;
        #endregion

        public PathHandler(int thread, int threadCount)
        {
            m_ThreadID = thread;
            m_ThreadCount = threadCount;

            Heap = new PathNodeHeap(128);
        }

        #region Public_API
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
                PathNode[] newArray = new PathNode[System.Math.Max(128, m_Nodes.Length * 2)];
                m_Nodes.CopyTo(newArray, 0);
                for (int i = m_Nodes.Length; i < newArray.Length; i++)
                {
                    newArray[i] = new PathNode();
                    newArray[i].Handler = this;
                }
                m_Nodes = newArray;
            }

            m_Nodes[index].Node = node;
        }
        public void ClearNode(NavNode node)
        {
            PathNode pn = GetPathNode(node.NodeIndex);

            pn.G = 0;
            pn.H = 0;
            pn.PathID = 0;
            pn.Parent = null;
            pn.Node = null;
        }

        public PathNode GetPathNode(int index) { return m_Nodes[index]; }
        #endregion
    }
}