namespace GameAI.Pathfinding.Core
{
    using UnityEngine;

    using GameUtils.Pool;

    public class ABPath : Path, IPoolable
    {
        #region Properties
        private Vector3 m_StartPos;
        private Vector3 m_EndPos;
        #endregion

        public ABPath(Vector3 start, Vector3 end) : base()
        {

        }

        #region IPoolable
        public void Recycle()
        {

        }
        #endregion
    }
}