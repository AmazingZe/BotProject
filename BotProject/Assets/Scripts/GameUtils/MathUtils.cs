namespace GameUtils
{
    using UnityEngine;

    public static class MathUtils
    {
        public static int Factorial(int num)
        {
            int ans = 1;

            for (int i = 2; i <= num; i++)
                ans *= i;

            return ans;
        }

        public static int C(int n, int m)
        {
            if (n < m) return -1;

            return Factorial(n) / (Factorial(m) * Factorial(n - m));
        }

        public static float CloestPointOnLineFast(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            var dir = lineEnd - lineStart;
            float sqrMag = dir.sqrMagnitude;

            if (sqrMag < 0.0001) return 0;

            return Vector3.Dot(point - lineStart, dir) / sqrMag;
        }
        public static float CloestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            var dir = (lineEnd - lineStart).normalized;
            float mag = dir.magnitude;

            if (mag < 0.01) return 0;

            return Vector3.Dot(point - lineStart, dir) / mag;
        }

        public static float LineCircleIntersectionFactor(Vector3 circleCenter,
                                                         Vector3 linePoint1,
                                                         Vector3 linePoint2,
                                                         float radius)
        {
            float segmentLength;
            var normalizedDir = Normalize(linePoint2 - linePoint1, out segmentLength);
            var dir2Start = linePoint1 - circleCenter;

            var dot = Vector3.Dot(dir2Start, normalizedDir);
            var discriminant = dot * dot - (dir2Start.sqrMagnitude - radius * radius);

            if (discriminant < 0)
                discriminant = 0;

            var t = -dot + Mathf.Sqrt(discriminant);
            return segmentLength > 0.001f ? t / segmentLength : 1f;
        }

        public static Vector3 Normalize(Vector3 vec, out float magnitude)
        {
            magnitude = vec.magnitude;

            if (magnitude > 0.001)
                return vec.normalized;
            else
                return Vector3.zero;
        }

        public static Vector3 ComplexMultiplyConjugate(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x + a.z * b.z, 0, a.z * b.x - a.x * b.z);
        }
        public static Vector3 ComplexMultiply(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x - a.z * b.z, 0, a.x * b.z + a.z * b.x);
        }
    }
}