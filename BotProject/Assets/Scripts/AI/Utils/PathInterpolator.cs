namespace GameAI.Utils
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameUtils;

    public class PathInterpolator
    {
        #region Properties
        private List<Vector3> m_Path;
        private float m_Distance2SegmentStart;
        private float m_CurDistance;
        private float m_CurSegmentLength = float.PositiveInfinity;
        private float m_TotalDistance = float.PositiveInfinity;
        #endregion

        #region Public_API
        public int SegementIndex;
        public Vector3 CurPosition
        {
            get
            {
                float t = m_CurSegmentLength > 0.0001f ? (m_CurDistance - m_Distance2SegmentStart) / m_CurSegmentLength : 0f;
                return Vector3.Lerp(m_Path[SegementIndex], m_Path[SegementIndex + 1], t);
            }
        }
        public Vector3 Tagent
        {
            get { return m_Path[SegementIndex + 1] - m_Path[SegementIndex]; }
        }
        public float RemainingDistance
        {
            get { return m_TotalDistance - m_CurDistance; }
        }
        public float TraveledDistance
        {
            get { return m_CurDistance; }
            set
            {
                m_CurDistance = value;
                while (m_CurDistance < m_Distance2SegmentStart && SegementIndex > 0)
                    PrevSegment();
                while (m_CurDistance > m_Distance2SegmentStart + m_CurSegmentLength && SegementIndex < m_Path.Count - 2)
                    NextSegment();
            }
        }
        public Vector3 EndPoint
        {
            get { return m_Path[m_Path.Count - 1]; }
        }
        public bool Valid
        {
            get { return m_Path != null; }
        }
                
        public void SetPath(List<Vector3> path)
        {
            m_Path = path;
            m_CurDistance = 0;
            SegementIndex = 0;
            m_Distance2SegmentStart = 0;

            if(path == null)
            {
                m_TotalDistance = float.PositiveInfinity;
                m_CurSegmentLength = float.PositiveInfinity;
                return;
            }

            if (path.Count < 2) throw new System.ArgumentException("Path must have a length of at least 2");

            m_CurSegmentLength = (m_Path[1] - m_Path[0]).magnitude;
            m_TotalDistance = 0;

            Vector3 prev = m_Path[0];
            for (int i = 1; i < m_Path.Count; i++)
            {
                var current = m_Path[i];
                m_TotalDistance += (current - prev).magnitude;
                prev = current;
            }
        }
        
        public void MoveToSegement(int index, float factorAlongSegment)
        {
            if (m_Path == null) return;

            while (SegementIndex > index) PrevSegment();
            while (SegementIndex < index) NextSegment();

            TraveledDistance = m_Distance2SegmentStart + Mathf.Clamp01(factorAlongSegment) * m_CurSegmentLength;
        }
        public void MoveToCloestPoint(Vector3 point)
        {
            if (m_Path == null) return;

            float bestDistance = float.PositiveInfinity;
            float bestFactor = 0;
            int bestIndex = 0;

            for (int i = 0; i < m_Path.Count - 1; i++)
            {
                float factor = MathUtils.CloestPointOnLineFast(m_Path[i], m_Path[i + 1], point);
                Vector3 cloest = Vector3.Lerp(m_Path[i], m_Path[i + 1], factor);
                float dist = (point - cloest).sqrMagnitude;
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestFactor = factor;
                    bestIndex = i;
                }
            }

            MoveToSegement(bestIndex, bestFactor);
        }
        public void MoveToCircleIntersection(Vector3 circleCenter, 
                                               float radius)
        {
            if (m_Path == null) return;

            while (SegementIndex < m_Path.Count - 2 &&
                   MathUtils.CloestPointOnLineFast(m_Path[SegementIndex], m_Path[SegementIndex + 1], circleCenter) > 1)
            {
                NextSegment();
            }

            while(SegementIndex < m_Path.Count - 2 &&
                  (m_Path[SegementIndex + 1] - circleCenter).sqrMagnitude <= radius * radius)
            {
                NextSegment();
            }

            var factor = MathUtils.LineCircleIntersectionFactor(circleCenter,
                                                                m_Path[SegementIndex],
                                                                m_Path[SegementIndex + 1],
                                                                radius);

            MoveToSegement(SegementIndex, factor);
        }
        #endregion

        private void PrevSegment()
        {
            SegementIndex--;
            m_CurSegmentLength = (m_Path[SegementIndex + 1] - m_Path[SegementIndex]).magnitude;
            m_Distance2SegmentStart -= m_CurSegmentLength;
        }
        private void NextSegment()
        {
            SegementIndex++;
            m_Distance2SegmentStart += m_CurSegmentLength;
            m_CurSegmentLength = (m_Path[SegementIndex + 1] - m_Path[SegementIndex]).magnitude;
        }
    }
}