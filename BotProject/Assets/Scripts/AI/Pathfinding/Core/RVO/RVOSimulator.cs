namespace GameAI.Pathfinding.RVO
{
    using UnityEngine;

    using System.Collections.Generic;

    public class RVOSimulator
    {
        #region Properties
        private readonly bool doubleBuffering = true;
        public readonly MovementPlane movementPlane = MovementPlane.XZ;
        private readonly RVOWorker[] workers;

        private float desiredDeltaTime = 0.05f;
        private float lastStep = -99999;
        private float deltaTime;
        private bool doUpdateObstacles = false;
        private bool doCleanObstacles = false;
        public float symmetryBreakingBias = 0.1f;
        public float DeltaTime { get { return deltaTime; } }
        public bool Multithreading { get { return workers != null && workers.Length > 0; } }
        public float DesiredDeltaTime
        {
            get { return desiredDeltaTime; }
            set { desiredDeltaTime = System.Math.Max(value, 0.0f); }
        }

        private List<Agent> agents;
        public List<ObstacleVertex> obstacles;
        private WorkerContext coroutineWorkerContext = new WorkerContext();
        public RVOQuadtree Quadtree { get; private set; }
        #endregion

        public RVOSimulator(int workers, bool doubleBuffering, MovementPlane movementPlane)
        {
            this.workers = new RVOWorker[workers];
            this.doubleBuffering = doubleBuffering;
            this.DesiredDeltaTime = 1;
            this.movementPlane = movementPlane;
            Quadtree = new RVOQuadtree();

            for (int i = 0; i < workers; i++) this.workers[i] = new RVOWorker(this);

            agents = new List<Agent>();
            obstacles = new List<ObstacleVertex>();
        }

        #region Agents/Obstacle
        public List<Agent> GetAgents() { return agents; }
        public void ClearAgents()
        {
            BlockUntilSimulationStepIsDone();

            for (int i = 0; i < agents.Count; i++)
                agents[i].simulator = null;
            agents.Clear();
        }
        public void RemoveAgent(IAgent agent)
        {
            if (agent == null)
                throw new System.ArgumentNullException("Agent must not be null");

            Agent agentReal = agent as Agent;
            if (agentReal == null)
                throw new System.ArgumentException("The agent must be of type Agent. Agent was of type " + agent.GetType());

            if (agentReal.simulator != this)
                throw new System.ArgumentException("The agent is not added to this simulation");

            BlockUntilSimulationStepIsDone();

            agentReal.simulator = null;

            if (!agents.Remove(agentReal))
            {
                throw new System.ArgumentException("Critical Bug! This should not happen. Please report this.");
            }
        }
        public IAgent AddAgent(IAgent agent)
        {
            if (agent == null)
                throw new System.ArgumentNullException("Agent must not be null");

            Agent agentReal = agent as Agent;
            if (agentReal == null)
                throw new System.ArgumentException("The agent must be of type Agent. Agent was of type " + agent.GetType());

            if (agentReal.simulator != null && agentReal.simulator == this)
                throw new System.ArgumentException("The agent is already in the simulation");
            else if (agentReal.simulator != null)
                throw new System.ArgumentException("The agent is already added to another simulation");
            agentReal.simulator = this;

            BlockUntilSimulationStepIsDone();

            agents.Add(agentReal);
            return agent;
        }
        public IAgent AddAgent(Vector2 position, float elevationCoordinate)
        {
            return AddAgent(new Agent(position, elevationCoordinate));
        }

        public List<ObstacleVertex> GetObstacles() { return obstacles; }
        public ObstacleVertex AddObstacle(ObstacleVertex v)
        {
            if (v == null)
                throw new System.ArgumentNullException("Obstacle must not be null");

            //Don't interfere with ongoing calculations
            BlockUntilSimulationStepIsDone();

            obstacles.Add(v);
            UpdateObstacles();
            return v;
        }
        public ObstacleVertex AddObstacle(Vector3[] vertices, float height, bool cycle = true)
        {
            return AddObstacle(vertices, height, Matrix4x4.identity, cycle);
        }
        public ObstacleVertex AddObstacle(Vector3[] vertices, float height, Matrix4x4 matrix, bool cycle = true)
        {
            if (vertices == null)
                throw new System.ArgumentNullException("Vertices must not be null");
            if (vertices.Length < 2)
                throw new System.ArgumentException("Less than 2 vertices in an obstacle");

            ObstacleVertex first = null;
            ObstacleVertex prev = null;

            // Don't interfere with ongoing calculations
            BlockUntilSimulationStepIsDone();

            for (int i = 0; i < vertices.Length; i++)
            {
                var v = new ObstacleVertex
                {
                    prev = prev,
                    height = height
                };

                if (first == null) first = v;
                else prev.next = v;

                prev = v;
            }

            if (cycle)
            {
                prev.next = first;
                first.prev = prev;
            }

            UpdateObstacle(first, vertices, matrix);
            obstacles.Add(first);
            return first;
        }
        public ObstacleVertex AddObstacle(Vector3 a, Vector3 b, float height)
        {
            ObstacleVertex first = new ObstacleVertex();
            ObstacleVertex second = new ObstacleVertex();

            first.prev = second;
            second.prev = first;
            first.next = second;
            second.next = first;

            first.position = a;
            second.position = b;
            first.height = height;
            second.height = height;
            second.ignore = true;

            first.dir = new Vector2(b.x - a.x, b.z - a.z).normalized;
            second.dir = -first.dir;

            BlockUntilSimulationStepIsDone();

            obstacles.Add(first);

            UpdateObstacles();
            return first;
        }
        public void UpdateObstacle(ObstacleVertex obstacle, Vector3[] vertices, Matrix4x4 matrix)
        {
            if (vertices == null)
                throw new System.ArgumentNullException("Vertices must not be null");
            if (obstacle == null)
                throw new System.ArgumentNullException("Obstacle must not be null");

            if (vertices.Length < 2)
                throw new System.ArgumentException("Less than 2 vertices in an obstacle");

            bool identity = matrix == Matrix4x4.identity;

            // Don't interfere with ongoing calculations
            BlockUntilSimulationStepIsDone();

            int count = 0;

            // Obstacles are represented using linked lists
            var vertex = obstacle;
            do
            {
                if (count >= vertices.Length)
                {
                    Debug.DrawLine(vertex.prev.position, vertex.position, Color.red);
                    throw new System.ArgumentException("Obstacle has more vertices than supplied for updating (" + vertices.Length + " supplied)");
                }

                // Premature optimization ftw!
                vertex.position = identity ? vertices[count] : matrix.MultiplyPoint3x4(vertices[count]);
                vertex = vertex.next;
                count++;
            } while (vertex != obstacle && vertex != null);

            vertex = obstacle;
            do
            {
                if (vertex.next == null)
                {
                    vertex.dir = Vector2.zero;
                }
                else
                {
                    Vector3 dir = vertex.next.position - vertex.position;
                    vertex.dir = new Vector2(dir.x, dir.z).normalized;
                }

                vertex = vertex.next;
            } while (vertex != obstacle && vertex != null);

            ScheduleCleanObstacles();
            UpdateObstacles();
        }
        public void UpdateObstacles()
        {
            doUpdateObstacles = true;
        }
        private void ScheduleCleanObstacles()
        {
            doCleanObstacles = true;
        }
        private void CleanObstacles()
        {

        }
        public void RemoveObstacle(ObstacleVertex v)
        {
            if (v == null) throw new System.ArgumentNullException("Vertex must not be null");

            BlockUntilSimulationStepIsDone();

            obstacles.Remove(v);
            UpdateObstacles();
        }
        #endregion

        public void BuildQuadtree()
        {
            Quadtree.Clear();
            if (agents.Count > 0)
            {
                Rect bounds = Rect.MinMaxRect(agents[0].position.x, agents[0].position.y, agents[0].position.x, agents[0].position.y);
                for (int i = 1; i < agents.Count; i++)
                {
                    Vector2 p = agents[i].position;
                    bounds = Rect.MinMaxRect(Mathf.Min(bounds.xMin, p.x), Mathf.Min(bounds.yMin, p.y), Mathf.Max(bounds.xMax, p.x), Mathf.Max(bounds.yMax, p.y));
                }
                Quadtree.SetBounds(bounds);

                for (int i = 0; i < agents.Count; i++)
                    Quadtree.Insert(agents[i]);

                //quadtree.DebugDraw ();
            }

            Quadtree.CalculateSpeeds();
        }

        private void PreCalculation()
        {
            for (int i = 0; i < agents.Count; i++)
                agents[i].PreCalculation();
        }

        public void Update()
        {
            if (lastStep < 0)
            {
                lastStep = Time.time;
                deltaTime = DesiredDeltaTime;
            }

            if (Time.time - lastStep >= DesiredDeltaTime)
            {
                deltaTime = Time.time - lastStep;
                lastStep = Time.time;

                // Prevent a zero delta time
                deltaTime = System.Math.Max(deltaTime, 1.0f / 2000f);

                if (Multithreading)
                {
                    if (doubleBuffering)
                    {
                        for (int i = 0; i < workers.Length; i++) workers[i].WaitOne();
                        for (int i = 0; i < agents.Count; i++) agents[i].PostCalculation();
                    }

                    PreCalculation();
                    CleanAndUpdateObstaclesIfNecessary();
                    BuildQuadtree();

                    for (int i = 0; i < workers.Length; i++)
                    {
                        workers[i].start = i * agents.Count / workers.Length;
                        workers[i].end = (i + 1) * agents.Count / workers.Length;
                    }
                    
                    for (int i = 0; i < workers.Length; i++) workers[i].Execute(1);
                    for (int i = 0; i < workers.Length; i++) workers[i].WaitOne();
                    
                    for (int i = 0; i < workers.Length; i++) workers[i].Execute(0);
                    
                    if (!doubleBuffering)
                    {
                        for (int i = 0; i < workers.Length; i++) workers[i].WaitOne();
                        for (int i = 0; i < agents.Count; i++) agents[i].PostCalculation();
                    }
                }
                else
                {
                    PreCalculation();
                    CleanAndUpdateObstaclesIfNecessary();
                    BuildQuadtree();

                    for (int i = 0; i < agents.Count; i++)
                    {
                        agents[i].BufferSwitch();
                    }

                    for (int i = 0; i < agents.Count; i++)
                    {
                        agents[i].CalculateNeighbours();
                        agents[i].CalculateVelocity(coroutineWorkerContext);
                    }

                    for (int i = 0; i < agents.Count; i++) agents[i].PostCalculation();
                }
            }
        }
        public void OnDestroy()
        {
            if (workers != null)
            {
                for (int i = 0; i < workers.Length; i++)
                    workers[i].Terminate();
            }
        }
        private void CleanAndUpdateObstaclesIfNecessary()
        {
            if (doCleanObstacles)
            {
                CleanObstacles();
                doCleanObstacles = false;
                doUpdateObstacles = true;
            }

            if (doUpdateObstacles) 
                doUpdateObstacles = false; 
        }

        private void BlockUntilSimulationStepIsDone()
        {
            if (Multithreading && doubleBuffering)
            {
                for (int j = 0; j < workers.Length; j++)
                    workers[j].WaitOne();
            }
        }
    }

    public enum MovementPlane
    {
        XZ,
        XY
    }
}