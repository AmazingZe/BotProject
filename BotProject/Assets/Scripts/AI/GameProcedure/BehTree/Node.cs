namespace GameProcedure.BehTree
{
    using System.Collections.Generic;

    public abstract class Node
    {
        #region Properties
        protected Node parent;
        private List<Node> m_Childs;
        private int m_MaxChildCount;

        public int Count
        {
            get { return m_Childs.Count; }
        }
        #endregion

        public Node(int num = -1)
        {
            m_Childs = new List<Node>();
            if (num >= 0)
                m_Childs.Capacity = num;
            m_MaxChildCount = num;
        }

        public Node AddChild(Node node)
        {
            if (m_MaxChildCount >= 0 && m_Childs.Count >= m_MaxChildCount) return this;
            m_Childs.Add(node);
            node.parent = this;
            return this;
        }
        public T GetChild<T>(int index) where T : Node
        {
            if (index < 0 || index >= m_Childs.Count) return null;

            return (T)m_Childs[index];
        }
        public bool IsIndexVaild(int index) { return index >= 0 && index < m_Childs.Count; }
        #region API

        #endregion
    }
    public enum RunningStatus
    {
        Failed, Running, Finished, Transition
    }
}