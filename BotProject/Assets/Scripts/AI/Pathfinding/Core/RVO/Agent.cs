namespace GameAI.Pathfinding.RVO
{
    using UnityEngine;

    using System.Collections.Generic;

    public class Agent : IAgent
    {
        #region Properties
        public Agent next;
        public RVOSimulator simulator;

        public float radius, height, desiredSpeed, 
                       maxSpeed, agentTimeHorizon, 
                       obstacleTimeHorizon;
        public bool locked = false;
        public Vector2 position;

        float elevationCoordinate;
        Vector2 currentVelocity, 
                desiredTargetPointInVelocitySpace, 
                desiredVelocity, collisionNormal,
                nextTargetPoint;
        float nextDesiredSpeed, nextMaxSpeed;
        bool manuallyControlled, debugDraw;
        int maxNeighbours;

        float calculatedSpeed;
        Vector2 calculatedTargetPoint;

        List<Agent> neighbours = new List<Agent>();
        List<float> neighbourDists = new List<float>();
        List<ObstacleVertex> obstaclesBuffered = new List<ObstacleVertex>();
        List<ObstacleVertex> obstacles = new List<ObstacleVertex>();

        private const float DesiredVelocityWeight = 0.1f;
        #endregion

        #region IAgent
		public Vector2 Position { get; set; }
        public float ElevationCoordinate { get; set; }
        public Vector2 CalculatedTargetPoint { get; private set; }
        public float CalculatedSpeed { get; private set; }
        public bool Locked { get; set; }
        public float Radius { get; set; }
        public float Height { get; set; }
        public float AgentTimeHorizon { get; set; }
        public float ObstacleTimeHorizon { get; set; }
        public int MaxNeighbours { get; set; }
        public int NeighbourCount { get; private set; }
        public bool DebugDraw
        {
            get
            {
                return debugDraw;
            }
            set
            {
                debugDraw = value && simulator != null && !simulator.Multithreading;
            }
        }
        public float Priority { get; set; }
        public System.Action PreCalculationCallback { private get; set; }
        
        public void SetTarget(Vector2 targetPoint, float desiredSpeed, float maxSpeed)
        {
            maxSpeed = System.Math.Max(maxSpeed, 0);
            desiredSpeed = System.Math.Min(System.Math.Max(desiredSpeed, 0), maxSpeed);

            nextTargetPoint = targetPoint;
            nextDesiredSpeed = desiredSpeed;
            nextMaxSpeed = maxSpeed;
        }
        public void SetCollisionNormal(Vector2 normal) { collisionNormal = normal; }
        public void ForceSetVelocity(Vector2 velocity)
        {
            nextTargetPoint = CalculatedTargetPoint = position + velocity * 1000;
            nextDesiredSpeed = CalculatedSpeed = velocity.magnitude;
            manuallyControlled = true;
        }
        #endregion

        private static Color Rainbow(float v)
        {
            Color c = new Color(v, 0, 0);

            if (c.r > 1) { c.g = c.r - 1; c.r = 1; }
            if (c.g > 1) { c.b = c.g - 1; c.g = 1; }
            return c;
        }
        private static Vector3 FromXZ(Vector2 p) { return new Vector3(p.x, 0, p.y); }
        private static Vector2 ToXZ(Vector3 p) { return new Vector2(p.x, p.z); }
        private static void DrawVO(Vector2 circleCenter, float radius, Vector2 origin)
        {
            float alpha = Mathf.Atan2((origin - circleCenter).y, (origin - circleCenter).x);
            float gamma = radius / (origin - circleCenter).magnitude;
            float delta = gamma <= 1.0f ? Mathf.Abs(Mathf.Acos(gamma)) : 0;

            Draw.Debug.CircleXZ(FromXZ(circleCenter), radius, Color.black, alpha - delta, alpha + delta);
            Vector2 p1 = new Vector2(Mathf.Cos(alpha - delta), Mathf.Sin(alpha - delta)) * radius;
            Vector2 p2 = new Vector2(Mathf.Cos(alpha + delta), Mathf.Sin(alpha + delta)) * radius;

            Vector2 p1t = -new Vector2(-p1.y, p1.x);
            Vector2 p2t = new Vector2(-p2.y, p2.x);
            p1 += circleCenter;
            p2 += circleCenter;

            Debug.DrawRay(FromXZ(p1), FromXZ(p1t).normalized * 100, Color.black);
            Debug.DrawRay(FromXZ(p2), FromXZ(p2t).normalized * 100, Color.black);
        }
        private static bool BiasDesiredVelocity(VOBuffer vos, ref Vector2 desiredVelocity, ref Vector2 targetPointInVelocitySpace, float maxBiasRadians)
        {
            var desiredVelocityMagn = desiredVelocity.magnitude;
            var maxValue = 0f;

            for (int i = 0; i < vos.length; i++)
            {
                float value;

                vos.buffer[i].Gradient(desiredVelocity, out value);
                maxValue = Mathf.Max(maxValue, value);
            }
            
            var inside = maxValue > 0;
            
            if (desiredVelocityMagn < 0.001f) 
                return inside; 
            var angle = Mathf.Min(maxBiasRadians, maxValue / desiredVelocityMagn);
            desiredVelocity += new Vector2(desiredVelocity.y, -desiredVelocity.x) * angle;
            targetPointInVelocitySpace += new Vector2(targetPointInVelocitySpace.y, -targetPointInVelocitySpace.x) * angle;
            return inside;
        }

        public Agent(Vector2 pos, float elevationCoordinate)
        {
            AgentTimeHorizon = 2;
            ObstacleTimeHorizon = 2;
            Height = 5;
            Radius = 5;
            MaxNeighbours = 10;
            Locked = false;
            Position = pos;
            ElevationCoordinate = elevationCoordinate;
            Priority = 0.5f;
            CalculatedTargetPoint = pos;
            CalculatedSpeed = 0;
            SetTarget(pos, 0, 0);
        }

        public void BufferSwitch()
        {
            radius = Radius;
            height = Height;
            maxSpeed = nextMaxSpeed;
            desiredSpeed = nextDesiredSpeed;
            agentTimeHorizon = AgentTimeHorizon;
            obstacleTimeHorizon = ObstacleTimeHorizon;
            maxNeighbours = MaxNeighbours;

            locked = Locked && !manuallyControlled;
            position = Position;
            elevationCoordinate = ElevationCoordinate;

            if (locked)
            {
                desiredTargetPointInVelocitySpace = position;
                desiredVelocity = currentVelocity = Vector2.zero;
            }
            else
            {
                desiredTargetPointInVelocitySpace = nextTargetPoint - position;
                
                currentVelocity = (CalculatedTargetPoint - position).normalized * CalculatedSpeed;
                
                desiredVelocity = desiredTargetPointInVelocitySpace.normalized * desiredSpeed;

                if (collisionNormal != Vector2.zero)
                {
                    collisionNormal.Normalize();
                    var dot = Vector2.Dot(currentVelocity, collisionNormal);
                    
                    if (dot < 0)
                    {
                        currentVelocity -= collisionNormal * dot;
                    }
                    
                    collisionNormal = Vector2.zero;
                }
            }
        }
        public void PreCalculation()
        {
            if (PreCalculationCallback != null) PreCalculationCallback(); 
        }
        public void PostCalculation()
        {
            if (!manuallyControlled)
            {
                CalculatedTargetPoint = calculatedTargetPoint;
                CalculatedSpeed = calculatedSpeed;
            }

            List<ObstacleVertex> tmp = obstaclesBuffered;
            obstaclesBuffered = obstacles;
            obstacles = tmp;

            manuallyControlled = false;
        }
        public void CalculateNeighbours()
        {
            neighbours.Clear();
            neighbourDists.Clear();

            if (MaxNeighbours > 0 && !locked) simulator.Quadtree.Query(position, maxSpeed, agentTimeHorizon, radius, this);

            NeighbourCount = neighbours.Count;
        }
        public float InsertAgentNeighbour(Agent agent, float rangeSq)
        {
            //Todo: Collider layer
            if (this == agent) return rangeSq;
            
            float dist = (agent.position - position).sqrMagnitude;

            if (dist < rangeSq)
            {
                if (neighbours.Count < maxNeighbours)
                {
                    neighbours.Add(null);
                    neighbourDists.Add(float.PositiveInfinity);
                }
                
                int i = neighbours.Count - 1;
                if (dist < neighbourDists[i])
                {
                    while (i != 0 && dist < neighbourDists[i - 1])
                    {
                        neighbours[i] = neighbours[i - 1];
                        neighbourDists[i] = neighbourDists[i - 1];
                        i--;
                    }
                    neighbours[i] = agent;
                    neighbourDists[i] = dist;
                }

                if (neighbours.Count == maxNeighbours)
                    rangeSq = neighbourDists[neighbourDists.Count - 1];
            }
            return rangeSq;
        }

        public void CalculateVelocity(WorkerContext context)
        {
            if (manuallyControlled) return; 

            if (locked)
            {
                calculatedSpeed = 0;
                calculatedTargetPoint = position;
                return;
            }
            
            var vos = context.vos;
            vos.Clear();

            GenerateObstacleVOs(vos);
            GenerateNeighbourAgentVOs(vos);

            bool insideAnyVO = BiasDesiredVelocity(vos, ref desiredVelocity, ref desiredTargetPointInVelocitySpace, simulator.symmetryBreakingBias);

            if (!insideAnyVO)
            {
                calculatedTargetPoint = desiredTargetPointInVelocitySpace + position;
                calculatedSpeed = desiredSpeed;
                if (DebugDraw) Draw.Debug.CrossXZ(FromXZ(calculatedTargetPoint), Color.white);
                return;
            }

            Vector2 result = Vector2.zero;

            result = GradientDescent(vos, currentVelocity, desiredVelocity);

            if (DebugDraw) Draw.Debug.CrossXZ(FromXZ(result + position), Color.white);

            calculatedTargetPoint = position + result;
            calculatedSpeed = Mathf.Min(result.magnitude, maxSpeed);
        }
        private void GenerateObstacleVOs(VOBuffer vos)
        {
            var range = maxSpeed * obstacleTimeHorizon;
            
            for (int i = 0; i < simulator.obstacles.Count; i++)
            {
                var obstacle = simulator.obstacles[i];
                var vertex = obstacle;
                // Iterate through all edges (defined by vertex and vertex.dir) in the obstacle
                do
                {
                    //Todo: Collider Layer
                    if (vertex.ignore)
                    {
                        vertex = vertex.next;
                        continue;
                    }

                    // Start and end points of the current segment
                    float elevation1, elevation2;
                    var p1 = To2D(vertex.position, out elevation1);
                    var p2 = To2D(vertex.next.position, out elevation2);

                    Vector2 dir = (p2 - p1).normalized;

                    // Signed distance from the line (not segment, lines are infinite)
                    // TODO: Can be optimized
                    float dist = VO.SignedDistanceFromLine(p1, dir, position);

                    if (dist >= -0.01f && dist < range)
                    {
                        float factorAlongSegment = Vector2.Dot(position - p1, p2 - p1) / (p2 - p1).sqrMagnitude;
                        
                        var segmentY = Mathf.Lerp(elevation1, elevation2, factorAlongSegment);
                        
                        var sqrDistToSegment = (Vector2.Lerp(p1, p2, factorAlongSegment) - position).sqrMagnitude;
                        
                        if (sqrDistToSegment < range * range && 
                            (simulator.movementPlane == MovementPlane.XY || (elevationCoordinate <= segmentY + vertex.height && elevationCoordinate + height >= segmentY)))
                        {
                            vos.Add(VO.SegmentObstacle(p2 - position, p1 - position, Vector2.zero, radius * 0.01f, 1f / ObstacleTimeHorizon, 1f / simulator.DeltaTime));
                        }
                    }

                    vertex = vertex.next;
                } while (vertex != obstacle && vertex != null && vertex.next != null);
            }
        }
        private void GenerateNeighbourAgentVOs(VOBuffer vos)
        {
            float inverseAgentTimeHorizon = 1.0f / agentTimeHorizon;
            
            Vector2 optimalVelocity = currentVelocity;

            for (int o = 0; o < neighbours.Count; o++)
            {
                Agent other = neighbours[o];
                
                if (other == this)
                    continue;
                
                float maxY = System.Math.Min(elevationCoordinate + height, other.elevationCoordinate + other.height);
                float minY = System.Math.Max(elevationCoordinate, other.elevationCoordinate);
                
                if (maxY - minY < 0) continue;

                float totalRadius = radius + other.radius;

                // Describes a circle on the border of the VO
                Vector2 voBoundingOrigin = other.position - position;

                float avoidanceStrength;
                if (other.locked || other.manuallyControlled) 
                    avoidanceStrength = 1; 
                else if (other.Priority > 0.00001f || Priority > 0.00001f) 
                    avoidanceStrength = other.Priority / (Priority + other.Priority); 
                else 
                    avoidanceStrength = 0.5f; 
                                
                Vector2 otherOptimalVelocity = Vector2.Lerp(other.currentVelocity, other.desiredVelocity, 2 * avoidanceStrength - 1);

                var voCenter = Vector2.Lerp(optimalVelocity, otherOptimalVelocity, avoidanceStrength);

                vos.Add(new VO(voBoundingOrigin, voCenter, totalRadius, inverseAgentTimeHorizon, 1 / simulator.DeltaTime));
                if (DebugDraw)
                    DrawVO(position + voBoundingOrigin * inverseAgentTimeHorizon + voCenter, totalRadius * inverseAgentTimeHorizon, position + voCenter);
            }
        }
        private Vector2 GradientDescent(VOBuffer vos, Vector2 sampleAround1, Vector2 sampleAround2)
        {
            float score1;
            var minima1 = Trace(vos, sampleAround1, out score1);

            if (DebugDraw) Draw.Debug.CrossXZ(FromXZ(minima1 + position), Color.yellow, 0.5f);
            
            float score2;
            Vector2 minima2 = Trace(vos, sampleAround2, out score2);
            if (DebugDraw) Draw.Debug.CrossXZ(FromXZ(minima2 + position), Color.magenta, 0.5f);

            return score1 < score2 ? minima1 : minima2;
        }
        private Vector2 EvaluateGradient(VOBuffer vos, Vector2 p, out float value)
        {
            Vector2 gradient = Vector2.zero;

            value = 0;

            // Avoid other agents
            for (int i = 0; i < vos.length; i++)
            {
                float w;
                var grad = vos.buffer[i].ScaledGradient(p, out w);
                if (w > value)
                {
                    value = w;
                    gradient = grad;
                }
            }
            var dirToDesiredVelocity = desiredVelocity - p;
            var distToDesiredVelocity = dirToDesiredVelocity.magnitude;
            if (distToDesiredVelocity > 0.0001f)
            {
                gradient += dirToDesiredVelocity * (DesiredVelocityWeight / distToDesiredVelocity);
                value += distToDesiredVelocity * DesiredVelocityWeight;
            }
            
            var sqrSpeed = p.sqrMagnitude;
            if (sqrSpeed > desiredSpeed * desiredSpeed)
            {
                var speed = Mathf.Sqrt(sqrSpeed);

                if (speed > maxSpeed)
                {
                    const float MaxSpeedWeight = 3;
                    value += MaxSpeedWeight * (speed - maxSpeed);
                    gradient -= MaxSpeedWeight * (p / speed);
                }
                
                float scale = 2 * DesiredVelocityWeight;
                value += scale * (speed - desiredSpeed);
                gradient -= scale * (p / speed);
            }

            return gradient;
        }

        private Vector2 To2D(Vector3 p, out float elevation)
        {
            if (simulator.movementPlane == MovementPlane.XY)
            {
                elevation = -p.z;
                return new Vector2(p.x, p.y);
            }
            else
            {
                elevation = p.y;
                return new Vector2(p.x, p.z);
            }
        }
        private Vector2 Trace(VOBuffer vos, Vector2 p, out float score)
        {
            float stepSize = Mathf.Max(radius, 0.2f * desiredSpeed);

            float bestScore = float.PositiveInfinity;
            Vector2 bestP = p;

            // TODO: Add momentum to speed up convergence?

            const int MaxIterations = 50;

            for (int s = 0; s < MaxIterations; s++)
            {
                float step = 1.0f - (s / (float)MaxIterations);
                step = step * step * stepSize;

                float value;
                var gradient = EvaluateGradient(vos, p, out value);

                if (value < bestScore)
                {
                    bestScore = value;
                    bestP = p;
                }

                // TODO: Add cutoff for performance

                gradient.Normalize();

                gradient *= step;
                Vector2 prev = p;
                p += gradient;

                if (DebugDraw) Debug.DrawLine(FromXZ(prev + position), FromXZ(p + position), Rainbow(s * 0.1f) * new Color(1, 1, 1, 1f));
            }

            score = bestScore;
            return bestP;
        }
    }
}