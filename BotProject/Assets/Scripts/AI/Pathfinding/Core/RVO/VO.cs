namespace GameAI.Pathfinding.RVO
{
    using UnityEngine;

    using GameUtils;

    public struct VO
    {
        #region Properties
        private Vector2 line1, line2, dir1, dir2;

        private Vector2 cutoffLine, cutoffDir;
        private Vector2 circleCenter;

        private bool colliding;
        private float radius;
        private float weightFactor;
        private float weightBonus;

        private Vector2 segmentStart, segmentEnd;
        private bool segment;
        #endregion

        public VO(Vector2 center, Vector2 offset, float radius, float inverseDt, float inverseDeltaTime)
        {            
            weightFactor = 1;
            weightBonus = 0;
            
            Vector2 globalCenter;

            circleCenter = center * inverseDt + offset;

            this.weightFactor = 4 * Mathf.Exp(-Sqr(center.sqrMagnitude / (radius * radius))) + 1;
            
            if (center.magnitude < radius)
            {
                colliding = true;
                               
                line1 = center.normalized * (center.magnitude - radius - 0.001f) * 0.3f * inverseDeltaTime;
                dir1 = new Vector2(line1.y, -line1.x).normalized;
                line1 += offset;

                cutoffDir = Vector2.zero;
                cutoffLine = Vector2.zero;
                dir2 = Vector2.zero;
                line2 = Vector2.zero;
                this.radius = 0;
            }
            else
            {
                colliding = false;

                center *= inverseDt;
                radius *= inverseDt;
                globalCenter = center + offset;
                
                var cutoffDistance = center.magnitude - radius + 0.001f;

                cutoffLine = center.normalized * cutoffDistance;
                cutoffDir = new Vector2(-cutoffLine.y, cutoffLine.x).normalized;
                cutoffLine += offset;

                float alpha = Mathf.Atan2(-center.y, -center.x);

                float delta = Mathf.Abs(Mathf.Acos(radius / center.magnitude));

                this.radius = radius;
                
                line1 = new Vector2(Mathf.Cos(alpha + delta), Mathf.Sin(alpha + delta));
                
                dir1 = new Vector2(line1.y, -line1.x);

                // Point on circle
                line2 = new Vector2(Mathf.Cos(alpha - delta), Mathf.Sin(alpha - delta));
                
                dir2 = new Vector2(line2.y, -line2.x);

                line1 = line1 * radius + globalCenter;
                line2 = line2 * radius + globalCenter;
            }

            segmentStart = Vector2.zero;
            segmentEnd = Vector2.zero;
            segment = false;
        }
        public static VO SegmentObstacle(Vector2 segmentStart, Vector2 segmentEnd, Vector2 offset, float radius, float inverseDt, float inverseDeltaTime)
        {
            var vo = new VO();
            
            vo.weightFactor = 1;

            vo.weightBonus = Mathf.Max(radius, 1) * 40;

            var closestOnSegment = MathUtils.CloestPointOnSegement(segmentStart, segmentEnd, Vector2.zero);
            
            if (closestOnSegment.magnitude <= radius)
            {
                vo.colliding = true;

                vo.line1 = closestOnSegment.normalized * (closestOnSegment.magnitude - radius) * 0.3f * inverseDeltaTime;
                vo.dir1 = new Vector2(vo.line1.y, -vo.line1.x).normalized;
                vo.line1 += offset;

                vo.cutoffDir = Vector2.zero;
                vo.cutoffLine = Vector2.zero;
                vo.dir2 = Vector2.zero;
                vo.line2 = Vector2.zero;
                vo.radius = 0;

                vo.segmentStart = Vector2.zero;
                vo.segmentEnd = Vector2.zero;
                vo.segment = false;
            }
            else
            {
                vo.colliding = false;

                segmentStart *= inverseDt;
                segmentEnd *= inverseDt;
                radius *= inverseDt;

                var cutoffTangent = (segmentEnd - segmentStart).normalized;
                vo.cutoffDir = cutoffTangent;
                vo.cutoffLine = segmentStart + new Vector2(-cutoffTangent.y, cutoffTangent.x) * radius;
                vo.cutoffLine += offset;

                // See documentation for details
                // The call to Max is just to prevent floating point errors causing NaNs to appear
                var startSqrMagnitude = segmentStart.sqrMagnitude;
                var normal1 = -MathUtils.ComplexMultiply2D(segmentStart, new Vector2(radius, Mathf.Sqrt(Mathf.Max(0, startSqrMagnitude - radius * radius)))) / startSqrMagnitude;
                var endSqrMagnitude = segmentEnd.sqrMagnitude;
                var normal2 = -MathUtils.ComplexMultiply2D(segmentEnd, new Vector2(radius, -Mathf.Sqrt(Mathf.Max(0, endSqrMagnitude - radius * radius)))) / endSqrMagnitude;

                vo.line1 = segmentStart + normal1 * radius + offset;
                vo.line2 = segmentEnd + normal2 * radius + offset;

                // Note that the normals are already normalized
                vo.dir1 = new Vector2(normal1.y, -normal1.x);
                vo.dir2 = new Vector2(normal2.y, -normal2.x);

                vo.segmentStart = segmentStart;
                vo.segmentEnd = segmentEnd;
                vo.radius = radius;
                vo.segment = true;
            }

            return vo;
        }
        public static float SignedDistanceFromLine(Vector2 a, Vector2 dir, Vector2 p)
        {
            return (p.x - a.x) * (dir.y) - (dir.x) * (p.y - a.y);
        }
        public Vector2 ScaledGradient(Vector2 p, out float weight)
        {
            var grad = Gradient(p, out weight);

            if (weight > 0)
            {
                const float Scale = 2;
                grad *= Scale * weightFactor;
                weight *= Scale * weightFactor;
                weight += 1 + weightBonus;
            }

            return grad;
        }
        public Vector2 Gradient(Vector2 p, out float weight)
        {
            if (colliding)
            {
                // Calculate double signed area of the triangle consisting of the points
                // {line1, line1+dir1, p}
                float l1 = SignedDistanceFromLine(line1, dir1, p);

                // Serves as a check for which side of the line the point p is
                if (l1 >= 0)
                {
                    weight = l1;
                    return new Vector2(-dir1.y, dir1.x);
                }
                else
                {
                    weight = 0;
                    return new Vector2(0, 0);
                }
            }

            float det3 = SignedDistanceFromLine(cutoffLine, cutoffDir, p);
            if (det3 <= 0)
            {
                weight = 0;
                return Vector2.zero;
            }
            else
            {
                // Signed distances to the two edges along the sides of the VO
                float det1 = SignedDistanceFromLine(line1, dir1, p);
                float det2 = SignedDistanceFromLine(line2, dir2, p);
                if (det1 >= 0 && det2 >= 0)
                {
                    // We are inside both of the half planes
                    // (all three if we count the cutoff line)
                    // and thus inside the forbidden region in velocity space

                    // Actually the negative gradient because we want the
                    // direction where it slopes the most downwards, not upwards
                    Vector2 gradient;

                    // Check if we are in the semicircle region near the cap of the VO
                    if (Vector2.Dot(p - line1, dir1) > 0 && Vector2.Dot(p - line2, dir2) < 0)
                    {
                        if (segment)
                        {
                            // This part will only be reached for line obstacles (i.e not other agents)
                            if (det3 < radius)
                            {
                                var closestPointOnLine = (Vector2)MathUtils.CloestPointOnSegement(segmentStart, segmentEnd, p);
                                var dirFromCenter = p - closestPointOnLine;
                                float distToCenter;
                                gradient = MathUtils.Normalize2D(dirFromCenter, out distToCenter);
                                // The weight is the distance to the edge
                                weight = radius - distToCenter;
                                return gradient;
                            }
                        }
                        else
                        {
                            var dirFromCenter = p - circleCenter;
                            float distToCenter;
                            gradient = MathUtils.Normalize2D(dirFromCenter, out distToCenter);
                            // The weight is the distance to the edge
                            weight = radius - distToCenter;
                            return gradient;
                        }
                    }

                    if (segment && det3 < det1 && det3 < det2)
                    {
                        weight = det3;
                        gradient = new Vector2(-cutoffDir.y, cutoffDir.x);
                        return gradient;
                    }

                    // Just move towards the closest edge
                    // The weight is the distance to the edge
                    if (det1 < det2)
                    {
                        weight = det1;
                        gradient = new Vector2(-dir1.y, dir1.x);
                    }
                    else
                    {
                        weight = det2;
                        gradient = new Vector2(-dir2.y, dir2.x);
                    }

                    return gradient;
                }

                weight = 0;
                return Vector2.zero;
            }
        }

        private static float Sqr(float x) { return x * x; }
    }

    public class VOBuffer
    {
        public VO[] buffer;
        public int length;

        public void Clear()
        {
            length = 0;
        }

        public VOBuffer(int n)
        {
            buffer = new VO[n];
            length = 0;
        }

        public void Add(VO vo)
        {
            if (length >= buffer.Length)
            {
                var nbuffer = new VO[buffer.Length * 2];
                buffer.CopyTo(nbuffer, 0);
                buffer = nbuffer;
            }
            buffer[length++] = vo;
        }
    }
}