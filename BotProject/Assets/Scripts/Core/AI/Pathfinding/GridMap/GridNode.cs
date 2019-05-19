namespace GameAI.Pathfinding.Grid
{
    using UnityEngine;

    public class GridNode : NavNode
    {
        #region Properties
        private const int NeighborOffset = 0;
        private const int NeighborBit0 = 1;
        private const int NeighborMask = 0xFF << NeighborOffset;
        private ushort m_neighborFlags = 0;
        #endregion

        #region Public_API
        public bool HasConnectionInDir(GridDir gridDir)
        {
            var dir = gridDir.GetHashCode();

            return (m_neighborFlags >> dir & NeighborBit0) != 0;
        }
        public void SetConnectionInDir(GridDir gridDir, bool value)
        {
            var dir = gridDir.GetHashCode();
            m_neighborFlags = (ushort)(m_neighborFlags & ~(1 << NeighborOffset << dir) | (value ? 1 : 0) << NeighborOffset << dir);
        }
        #endregion

        public enum GridDir
        {
            Up,
            Right,
            Bottom,
            Left,
            UpRight,
            RightBottom,
            LeftBottom,
            LeftUp
        }
    }
}