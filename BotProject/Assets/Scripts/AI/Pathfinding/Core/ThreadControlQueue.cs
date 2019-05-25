namespace GameAI.Pathfinding.Core
{
    using System.Threading;

    public class ThreadControlQueue
    {
        #region Properties
        private readonly int m_ReceiveNum;
        private readonly object m_Lock = new object();
        private int m_BlockedNum;
        private bool m_Starve;
        private bool m_Block;
        private bool m_Terminate;
        private ManualResetEvent m_BolckEvent = new ManualResetEvent(true);

        private Path m_Head, m_Tail;
        #endregion

        public ThreadControlQueue(int threadNum)
        {
            m_ReceiveNum = threadNum;
        }

        #region Public_API
        public bool IsEmpty
        {
            get { return m_Head == null; }
        }

        public void Starve()
        {
            m_Starve = true;
            m_BolckEvent.Reset();
        }

        public void Block()
        {
            lock (m_Lock)
            {
                m_Block = true;
                m_BolckEvent.Reset();
            }
        }
        public void Unblock()
        {
            lock (m_Lock)
            {
                m_Block = false;
                m_BolckEvent.Set();
            }
        }
        
        public void PushFront(Path path)
        {
            lock (m_Lock)
            {
                if (m_Terminate) return;

                if (m_Head == null)
                {
                    m_Head = path;
                    m_Tail = path;

                    if (m_Starve && !m_Block)
                    {
                        m_Starve = false;
                        m_BolckEvent.Set();
                    }
                    else
                        m_Starve = false;
                }
                else
                {
                    path.Next = m_Head;
                    m_Head = path;
                }
            }
        }
        public void Push(Path path)
        {
            lock (m_Lock)
            {
                if (m_Terminate) return;

                if (m_Tail == null)
                {
                    m_Head = path;
                    m_Tail = path;

                    if (m_Starve && !m_Block)
                    {
                        m_Starve = false;
                        m_BolckEvent.Set();
                    }
                    else
                        m_Starve = false;
                }
                else
                {
                    m_Tail.Next = path;
                    m_Tail = path;
                }
            }
        }
        public Path Pop()
        {
            Monitor.Enter(m_Lock);
            try
            {
                if (m_Terminate)
                {
                    m_BlockedNum++;
                    throw new System.Exception("QueueTerminationException");
                }

                if (m_Head == null)
                    Starve();

                while (m_Block || m_Starve)
                {
                    m_BlockedNum++;
                    if(m_BlockedNum > m_ReceiveNum)
                        throw new System.InvalidOperationException("More receivers are blocked than specified in constructor (" + m_BlockedNum + " > " + m_ReceiveNum + ")");
                    Monitor.Exit(m_Lock);

                    m_BolckEvent.WaitOne();

                    Monitor.Enter(m_Lock);
                    if (m_Terminate)
                        throw new System.Exception("QueueTerminationException");

                    m_BlockedNum--;

                    if (m_Head == null)
                        Starve();
                }

                Path p = m_Head;

                var newHead = m_Head.Next;
                if (newHead == null) m_Tail = null;
                m_Head.Next = null;
                m_Head = newHead;
                return p;
            }
            finally
            {
                Monitor.Exit(m_Lock);
            }
        }
        public Path PopNoBlock(bool blockedBefore)
        {
            Monitor.Enter(m_Lock);
            try
            {
                if (m_Terminate)
                {
                    m_BlockedNum++;
                    throw new System.Exception("QueueTerminationException");
                }

                if (m_Head == null)
                    Starve();

                if (m_Block || m_Starve)
                {
                    if (!blockedBefore)
                    {
                        m_BlockedNum++;
                        if (m_Terminate)
                            throw new System.Exception("QueueTerminationException");
                        if (m_BlockedNum == m_ReceiveNum)
                        {

                        }else if (m_BlockedNum > m_ReceiveNum)
                        {
                            throw new System.InvalidOperationException("More receivers are blocked than specified in constructor (" + m_BlockedNum + " > " + m_ReceiveNum + ")");
                        }
                    }
                    return null;
                }

                if (blockedBefore) m_BlockedNum--;
                Path p = m_Head;

                var newHead = m_Head.Next;
                if (newHead == null) m_Tail = null;
                m_Head.Next = null;
                m_Head = newHead;
                return p;
            }
            finally
            {
                Monitor.Exit(m_Lock);
            }
        }
        #endregion
    }
}