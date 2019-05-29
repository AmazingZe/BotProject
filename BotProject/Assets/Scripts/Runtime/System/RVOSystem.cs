namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameAI.Pathfinding.RVO;

    public class RVOSystem : MonoBehaviour
    {
        #region Properties
        public int agentCount = 100;
        public RVOExampleType type = RVOExampleType.Line;
        public float exampleScale = 100;
        public float radius = 3;
        public float maxSpeed = 2;
        public float agentTimeHorizon = 10;
        public float obstacleTimeHorizon = 10;
        public bool debug = false;
        public int maxNeighbours = 10;
        public Vector3 renderingOffset = Vector3.up * 0.1f;
        private Mesh mesh;
        private List<IAgent> agents;
        private List<Vector3> goals;
        private List<Color> colors;
        Vector3[] verts;
        Vector2[] uv;
        int[] tris;
        Color[] meshColors;
        Vector2[] interpolatedVelocities;
        Vector2[] interpolatedRotations;
        /// <summary>
        /// /////////////////////////////////////////////////////////////
        /// </summary>

        public float symmetryBreakingBias = 0.1f;
        public int desiredSimulationFPS = 30;
        public int threadCount = 1;
        public static RVOSimulator Simulator { get;set; }
        #endregion

        #region UnityCallback
        private void Start()
        {
            mesh = new Mesh();

            if (Simulator == null && Application.isPlaying) 
                Simulator = new RVOSimulator(threadCount, true, MovementPlane.XZ);
            GetComponent<MeshFilter>().mesh = mesh;

            CreateAgent();
        }
        private void Update()
        {
            if (!Application.isPlaying) return;

            if (agents == null || mesh == null) return;

            SetAgentSettings();

            if (interpolatedVelocities == null || interpolatedVelocities.Length < agents.Count)
            {
                var velocities = new Vector2[agents.Count];
                var directions = new Vector2[agents.Count];
                // Copy over the old velocities
                if (interpolatedVelocities != null) for (int i = 0; i < interpolatedVelocities.Length; i++) velocities[i] = interpolatedVelocities[i];
                if (interpolatedRotations != null) for (int i = 0; i < interpolatedRotations.Length; i++) directions[i] = interpolatedRotations[i];
                interpolatedVelocities = velocities;
                interpolatedRotations = directions;
            }

            for (int i = 0; i < agents.Count; i++)
            {
                IAgent agent = agents[i];

                // Move agent
                // This is the responsibility of this script, not the RVO system
                Vector2 pos = agent.Position;
                var deltaPosition = Vector2.ClampMagnitude(agent.CalculatedTargetPoint - pos, agent.CalculatedSpeed * Time.deltaTime);
                pos += deltaPosition;
                agent.Position = pos;

                // All agents are on the same plane
                agent.ElevationCoordinate = 0;

                // Set the desired velocity for all agents
                var target = new Vector2(goals[i].x, goals[i].z);
                var dist = (target - pos).magnitude;
                agent.SetTarget(target, Mathf.Min(dist, maxSpeed), maxSpeed * 1.1f);

                interpolatedVelocities[i] += deltaPosition;
                if (interpolatedVelocities[i].magnitude > maxSpeed * 0.1f)
                {
                    interpolatedVelocities[i] = Vector2.ClampMagnitude(interpolatedVelocities[i], maxSpeed * 0.1f);
                    interpolatedRotations[i] = Vector2.Lerp(interpolatedRotations[i], interpolatedVelocities[i], agent.CalculatedSpeed * Time.deltaTime * 4f);
                }

                //Debug.DrawRay(new Vector3(pos.x, 0, pos.y), new Vector3(interpolatedVelocities[i].x, 0, interpolatedVelocities[i].y) * 10);
                // Create a square with the "forward" direction along the agent's velocity
                Vector3 forward = new Vector3(interpolatedRotations[i].x, 0, interpolatedRotations[i].y).normalized * agent.Radius;
                if (forward == Vector3.zero) forward = new Vector3(0, 0, agent.Radius);
                Vector3 right = Vector3.Cross(Vector3.up, forward);
                Vector3 orig = new Vector3(agent.Position.x, agent.ElevationCoordinate, agent.Position.y) + renderingOffset;


                int vc = 4 * i;
                int tc = 2 * 3 * i;
                verts[vc + 0] = (orig + forward - right);
                verts[vc + 1] = (orig + forward + right);
                verts[vc + 2] = (orig - forward + right);
                verts[vc + 3] = (orig - forward - right);

                uv[vc + 0] = (new Vector2(0, 1));
                uv[vc + 1] = (new Vector2(1, 1));
                uv[vc + 2] = (new Vector2(1, 0));
                uv[vc + 3] = (new Vector2(0, 0));

                meshColors[vc + 0] = colors[i];
                meshColors[vc + 1] = colors[i];
                meshColors[vc + 2] = colors[i];
                meshColors[vc + 3] = colors[i];

                tris[tc + 0] = (vc + 0);
                tris[tc + 1] = (vc + 1);
                tris[tc + 2] = (vc + 2);

                tris[tc + 3] = (vc + 0);
                tris[tc + 4] = (vc + 2);
                tris[tc + 5] = (vc + 3);
            }

            var sim = GetSimulator();
            sim.DesiredDeltaTime = 1.0f / desiredSimulationFPS;
            sim.symmetryBreakingBias = symmetryBreakingBias;
            sim.Update();

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uv;
            mesh.colors = meshColors;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
        }
        private void OnDestroy()
        {
            if (Simulator != null) Simulator.OnDestroy();
        }
        #endregion

        public RVOSimulator GetSimulator()
        {
            if (Simulator == null)
                Start(); 
            return Simulator;
        }
        private float uniformDistance(float radius)
        {
            float v = Random.value + Random.value;

            if (v > 1) return radius * (2 - v);
            else return radius * v;
        }
        private void CreateAgent()
        {
            agents = new List<IAgent>(agentCount);
            goals = new List<Vector3>(agentCount);
            colors = new List<Color>(agentCount);

            Simulator.ClearAgents();

            if (type == RVOExampleType.Line)
            {
                for (int i = 0; i < agentCount; i++)
                {
                    Vector3 pos = new Vector3((i % 2 == 0 ? 1 : -1) * exampleScale, 0, (i / 2) * radius * 2.5f);
                    IAgent agent = Simulator.AddAgent(new Vector2(pos.x, pos.z), pos.y);
                    agents.Add(agent);
                    goals.Add(new Vector3(-pos.x, pos.y, pos.z));
                    colors.Add(i % 2 == 0 ? Color.red : Color.blue);
                }
            }
            SetAgentSettings();

            verts = new Vector3[4 * agents.Count];
            uv = new Vector2[verts.Length];
            tris = new int[agents.Count * 2 * 3];
            meshColors = new Color[verts.Length];
        }
        public static Color HSVToRGB(float h, float s, float v)
        {
            float r = 0, g = 0, b = 0;

            float Chroma = s * v;
            float Hdash = h / 60.0f;
            float X = Chroma * (1.0f - System.Math.Abs((Hdash % 2.0f) - 1.0f));

            if (Hdash < 1.0f)
            {
                r = Chroma;
                g = X;
            }
            else if (Hdash < 2.0f)
            {
                r = X;
                g = Chroma;
            }
            else if (Hdash < 3.0f)
            {
                g = Chroma;
                b = X;
            }
            else if (Hdash < 4.0f)
            {
                g = X;
                b = Chroma;
            }
            else if (Hdash < 5.0f)
            {
                r = X;
                b = Chroma;
            }
            else if (Hdash < 6.0f)
            {
                r = Chroma;
                b = X;
            }

            float Min = v - Chroma;

            r += Min;
            g += Min;
            b += Min;

            return new Color(r, g, b);
        }
        private void SetAgentSettings()
        {
            for (int i = 0; i < agents.Count; i++)
            {
                IAgent agent = agents[i];
                agent.Radius = radius;
                agent.AgentTimeHorizon = agentTimeHorizon;
                agent.ObstacleTimeHorizon = obstacleTimeHorizon;
                agent.MaxNeighbours = maxNeighbours;
                agent.DebugDraw = i == 0 && debug;
            }
        }
    }

    public enum RVOExampleType
    {
        Circle,
        Line,
        Point,
        RandomStreams,
        Crossing
    }
}