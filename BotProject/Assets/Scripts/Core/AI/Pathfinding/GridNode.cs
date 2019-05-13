namespace GameAI.Pathfinding
{
    using UnityEngine;

    public class GridNode : NavNode
    {
        #region Properties
        private Vector3 m_pos;

        private const int NeighborOffset = 0;
        private const int NeighborBit0 = 1;
        private const int NeighborMask = 0xFF << NeighborOffset;
        private ushort m_neighborFlags;
        #endregion

        #region Public_API
        public bool HasConnectionInDir(GridDir gridDir)
        {
            return false;
        }
        public void SetConnectionInDir()
        {

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