namespace GameAI.Pathfinding.Core
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    [Serializable]
    public abstract class NavNode
    {
        #region Properties
        private const int NodexIndexOffset = 0;
        private const int NodeIndexMask = 0xFFFF << NodexIndexOffset;
        private const int GraphIndexOffset = 16;
        private const int GraphIndexMask = 0xFF << GraphIndexOffset;

        private int m_Index = 0;
        #endregion

        #region Public_Properties
        public int NodeIndex
        {
            get { return m_Index & NodeIndexMask; }
            set { m_Index = (m_Index & ~NodeIndexMask) | value; }
        }
        public int GraphIndex
        {
            get { return m_Index & GraphIndexMask; }
            set { m_Index = (m_Index & ~GraphIndexMask) | (value << GraphIndexMask); }
        }
        public abstract Vector3 Position { get; set; }
        public abstract bool Walkable { get; set; }
        #endregion

        #region Graph_API
        public abstract void GetNodes(Action<NavNode> action);
        public abstract void GetNeighbor(List<NavNode> list);
        public abstract int GetNeighborCost(int dir);
        public abstract void GetConnection(Action<NavNode> action);
        #endregion

        public virtual int GetGizmosHashCode()
        {
            return Position.GetHashCode();
        }
    }
}