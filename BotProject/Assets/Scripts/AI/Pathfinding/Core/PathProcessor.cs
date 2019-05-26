namespace GameAI.Pathfinding.Core
{
    using System;
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class PathProcessor
    {
        #region Properties
        private readonly PathReturnQueue m_ReturnQueue;
        private readonly ThreadControlQueue m_ControlQueue;
        private readonly Thread[] m_Threads;
        private readonly PathHandler[] m_Handlers;
        private readonly Stack<int> m_IndexPool;

        private IEnumerator threadCourtine;
        private IPathfinder m_Pathfinder;
        private float m_MaxFrameTime;
        #endregion

        #region Public_Properties
        public int ThreadNum
        {
            get
            {
                if (IsMultiThread)
                    return m_Threads.Length;
                else
                    return 1;
            }
        }
        public float MaxFrameTime
        {
            get { return m_MaxFrameTime; }
            set { m_MaxFrameTime = value; }
        }

        public event Action<Path> OnPrePathSearch;
        public event Action<Path> OnPostPathSearch;
        public event Action OnQueueUnblock;

        public bool IsMultiThread = false;
        public AlgorithmType SearchType = AlgorithmType.AStar;
        #endregion

        public PathProcessor(int threadNum, PathReturnQueue returnQueue)
        {
            if (threadNum < 0)
                throw new ArgumentOutOfRangeException("processors illegal");

            if (threadNum > 0)
            {
                m_Threads = new Thread[threadNum];
                m_Handlers = new PathHandler[threadNum];
                for (int i = 0; i < threadNum; i++)
                {

                }
                IsMultiThread = true;
            }
            else
            {
                m_Threads = new Thread[0];
                m_Handlers = new PathHandler[1];
                m_Handlers[0] = new PathHandler(0, 0);
                threadCourtine = CalculatePath(m_Handlers[0]);
                IsMultiThread = false;
            }

            m_ReturnQueue = returnQueue;
            m_ControlQueue = new ThreadControlQueue(threadNum);
            m_IndexPool = new Stack<int>();
            m_Pathfinder = AStarPathFinder<GridNode>.Instance;

            m_MaxFrameTime = 0;
        }

        #region Public_API
        public void InitNode(NavNode node)
        {
            if (!m_ControlQueue.AllReceivorBlocked)
                throw new Exception("Not safe to init");
            
            for (int i = 0; i < m_Handlers.Length; i++)
                m_Handlers[i].InitializeNode(node);
        }
        public void DestoryNode(NavNode node)
        {
            if (node.NodeIndex == -1) return;

            m_IndexPool.Push(node.NodeIndex);

            for (int i = 0; i < m_Handlers.Length; i++)
                m_Handlers[i].ClearNode(node);
        }

        public void NonMultiThreadTick()
        {
            if (threadCourtine != null)
            {
                try
                {
                    threadCourtine.MoveNext();
                }
                catch(Exception e)
                {
                    threadCourtine = null;
                    if (!(e is ThreadControlQueueException))
                    {
                        Debug.LogException(e);
                        Debug.LogError("Unhandled exception during pathfinding. Terminating.");
                        m_ControlQueue.TerminateReceivers();

                        try
                        {
                            m_ControlQueue.PopNoBlock(false);
                        }
                        catch { }
                    }
                }
            }
        }
        #endregion

        private IEnumerator CalculatePath(PathHandler handler)
        {
            long maxTicks = (long)(m_MaxFrameTime * 10000);
            long targetTick = DateTime.UtcNow.Ticks + maxTicks;

            while (true)
            {
                Path path = null;
                bool blockBefore = false;

                while (path == null)
                {
                    try
                    {
                        path = m_ControlQueue.PopNoBlock(blockBefore);
                        blockBefore |= (path == null);
                    }
                    catch(ThreadControlQueueException)
                    {
                        yield break;
                    }

                    if (path == null)
                        yield return null;
                }

                maxTicks = (long)(m_MaxFrameTime * 10000);
                long startTicks = DateTime.UtcNow.Ticks;
                long totalTicks = 0;

                path.PrepareBase(handler);
                
                var preCallback = OnPrePathSearch;
                if (preCallback != null) preCallback(path);

                path.Prepare();
                path.AdvanceState(PathState.Processing);

                if (path.CompleteState == PathCompleteState.NotCalculated)
                {
                    path.InitPath();

                    while (path.CompleteState == PathCompleteState.NotCalculated)
                    {
                        path.Process(SearchType);

                        if (path.CompleteState != PathCompleteState.NotCalculated)
                            break;
                    }
                    totalTicks += DateTime.UtcNow.Ticks - startTicks;
                }

                path.CleanUP();

                var onComplete = path.OnComplete;
                if (onComplete != null) onComplete(path);

                var postCallBack = OnPostPathSearch;
                if (postCallBack != null) postCallBack(path);

                m_ReturnQueue.Enqueue(path);
                path.AdvanceState(PathState.ReturnQueue);

                if (DateTime.UtcNow.Ticks > targetTick)
                {
                    yield return null;
                    targetTick = DateTime.UtcNow.Ticks + maxTicks;
                }
            }
        }
    }
}