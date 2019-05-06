namespace GameAI.Pathfinding
{
    using System;

    using UnityEngine;

    public struct Int3 : IEquatable<Int3>
    {
        #region Properties
        private const int PrecisionFactor = 1000;
        private const float InvPrecisionFactor = 0.001F;

        public int x;
        public int y;
        public int z;
        #endregion

        public Int3(Vector3 vector)
        {
            x = (int)Mathf.Round(vector.x * PrecisionFactor);
            y = (int)Mathf.Round(vector.y * PrecisionFactor);
            z = (int)Mathf.Round(vector.z * PrecisionFactor);
        }
        public Int3(int _x, int _y, int _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        #region API
        public static explicit operator Int3(Vector3 vector)
        {
            return new Int3(
                (int)Mathf.Round(vector.x * PrecisionFactor),
                (int)Mathf.Round(vector.y * PrecisionFactor),
                (int)Mathf.Round(vector.z * PrecisionFactor)
                );
        }
        public static explicit operator Vector3(Int3 int3)
        {
            return new Vector3(
                int3.x * InvPrecisionFactor,
                int3.y * InvPrecisionFactor,
                int3.z * InvPrecisionFactor
                );
        }
        public static Int3 operator - (Int3 lhs, Int3 rhs)
        {
            return new Int3(
                lhs.x - rhs.x,
                lhs.y - rhs.y,
                lhs.z - rhs.z
                );
        }
        public static Int3 operator + (Int3 lhs, Int3 rhs)
        {
            return new Int3(
                lhs.x + rhs.x,
                lhs.y + rhs.y,
                lhs.z + rhs.z
                );
        }

        public bool Equals(Int3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }
        #endregion
    }
}