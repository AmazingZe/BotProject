namespace GameAI.Pathfinding.RVO
{
    using UnityEngine;

    using System.Threading;
    using System.Collections.Generic;

    public class WorkerContext
    {
        public VOBuffer vos = new VOBuffer(16);

        public const int KeepCount = 3;
        public Vector2[] bestPos = new Vector2[KeepCount];
        public float[] bestSizes = new float[KeepCount];
        public float[] bestScores = new float[KeepCount + 1];

        public Vector2[] samplePos = new Vector2[50];
        public float[] sampleSize = new float[50];
    }

    public class RVOWorker
    {
        #region Properties
        private readonly AutoResetEvent runFlag = new AutoResetEvent(false);
        private readonly ManualResetEvent waitFlag = new ManualResetEvent(true);
        private readonly RVOSimulator simulator;

        public int start, end;

        private int task = 0;
        private bool terminate = false;

        private WorkerContext context = new WorkerContext();
        #endregion

        public RVOWorker(RVOSimulator sim)
        {
            this.simulator = sim;
            var thread = new Thread(new ThreadStart(Run));
            thread.IsBackground = true;
            thread.Name = "RVO Simulator Thread";
            thread.Start();
        }

        public void Execute(int task)
        {
            this.task = task;
            waitFlag.Reset();
            runFlag.Set();
        }
        public void WaitOne()
        {
            if (!terminate) waitFlag.WaitOne();
        }
        public void Terminate()
        {
            WaitOne();
            terminate = true;
            Execute(-1);
        }
        public void Run()
        {
            runFlag.WaitOne();

            while (!terminate)
            {
                try
                {
                    List<Agent> agents = simulator.GetAgents();
                    if (task == 0)
                    {
                        for (int i = start; i < end; i++)
                        {
                            agents[i].CalculateNeighbours();
                            agents[i].CalculateVelocity(context);
                        }
                    }
                    else if (task == 1)
                    {
                        for (int i = start; i < end; i++)
                        {
                            agents[i].BufferSwitch();
                        }
                    }
                    else if (task == 2)
                    {
                        simulator.BuildQuadtree();
                    }
                    else
                    {
                        Debug.LogError("Invalid Task Number: " + task);
                        throw new System.Exception("Invalid Task Number: " + task);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
                waitFlag.Set();
                runFlag.WaitOne();
            }
        }
    }
}