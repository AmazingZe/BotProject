namespace GameAI.Pathfinding.Core
{
    public sealed class PathNodeHeap
    {
        #region Properties
        public const int NotInHeap = -1;

        private PathNode[] m_Nodes;
        private int m_FirstFree;
        #endregion

        public PathNodeHeap(int capacity)
        {
            m_Nodes = new PathNode[capacity + 1];

            m_FirstFree = 1;
        }

        #region Public_API
        public PathNode First
        {
            get { return m_Nodes[0]; }
        }
        public int Count
        {
            get { return m_FirstFree - 1; }
        }
        public int Capacity
        {
            get { return m_Nodes.Length; }
        }

        public void Enqueue(PathNode node, float priority)
        {
            if (node.HeapIndex != -1) return;

            node.Priority = priority;
            node.HeapIndex = m_FirstFree;
            m_Nodes[m_FirstFree++] = node;
            AdjustUp(node);
        }
        public PathNode Dequeue()
        {
            if (m_FirstFree == 1) return null;
            
            PathNode retMe = m_Nodes[1];

            if(m_FirstFree == 2)
            {
                m_Nodes[--m_FirstFree] = null;
                return retMe;
            }

            m_Nodes[1] = m_Nodes[--m_FirstFree];
            m_Nodes[m_FirstFree] = null;
            AdjustDown(m_Nodes[1]);
            retMe.HeapIndex = NotInHeap;

            return retMe;
        }

        public void Update(PathNode node, float priority)
        {
            if (node.HeapIndex == NotInHeap) return;

            bool flag = node.Priority > priority;
            node.Priority = priority;
            if (node.HeapIndex == Count) AdjustUp(node);

            int parent = node.HeapIndex >> 1;
            if (!flag) AdjustDown(node);
            else AdjustUp(node);
        }
        public void Remove(PathNode node)
        {
            if (node.HeapIndex == NotInHeap) return;

            if(node.HeapIndex == Count)
            {
                m_Nodes[node.HeapIndex] = null;
                m_FirstFree--;
                node.HeapIndex = NotInHeap;
                return;
            }

            PathNode lastNode = m_Nodes[--m_FirstFree];
            m_Nodes[m_FirstFree] = null;
            m_Nodes[node.HeapIndex] = lastNode;
            lastNode.HeapIndex = node.HeapIndex;
            node.HeapIndex = NotInHeap;

            AdjustDown(lastNode);
        }

        public void Resize(int newSize)
        {
            var newArray = new PathNode[newSize + 1];
            int highestSize = System.Math.Min(newSize, m_Nodes.Length);

            System.Array.Copy(m_Nodes, newArray, highestSize);
            m_Nodes = newArray;
        }

        public void Clear()
        {
            for (int i = 0; i < m_FirstFree; i++)
            {
                m_Nodes[i].Reset();
                m_Nodes[i] = null;
            }
            m_FirstFree = 1;
        }
        #endregion

        private void AdjustDown(PathNode node)
        {
            int finalIndex = node.HeapIndex;
            int childleft = finalIndex * 2;

            if (childleft > Count) return;

            int childRight = childleft + 1;
            PathNode child0Node = m_Nodes[childleft];
            if (child0Node.Priority < node.Priority)
            {
                if (childRight > Count)
                {

                }
                else
                {
                    PathNode child1Node = m_Nodes[childRight];
                    if (child1Node.Priority < child0Node.Priority)
                    {

                    }
                }
            }
            else
            {
                if(childRight <= Count)
                {
                    PathNode child1Node = m_Nodes[childRight];
                    if (child1Node.Priority < node.Priority)
                    {

                    }
                }
            }

            while ()
            {

            }
        }
        private void AdjustUp(PathNode node)
        {
            int parent;
            if (node.HeapIndex > 1)
            {
                parent = node.HeapIndex >> 1;
                PathNode parentNode = m_Nodes[parent];
                if (parentNode.Priority <= node.Priority) return;

                m_Nodes[node.HeapIndex] = parentNode;
                parentNode.HeapIndex = node.HeapIndex;
                node.HeapIndex = parent;
            }
            else
            {
                return;
            }

            while (parent > 1)
            {
                parent >>= 1;
                PathNode parentNode = m_Nodes[parent];
                if (parentNode.Priority <= node.Priority) break;

                m_Nodes[node.HeapIndex] = parentNode;
                parentNode.HeapIndex = node.HeapIndex;
                node.HeapIndex = parent;
            }

            m_Nodes[node.HeapIndex] = node;
        }
    }
}