namespace GameAI.Pathfinding.Core
{
    using UnityEngine;

    using GameUtils.Pool;

    public class ABPath : Path
    {
        #region Properties
        private Vector3 m_StartPos;
        private Vector3 m_EndPos;
        #endregion

        #region Public_API
        public static ABPath Construct(Vector3 start, Vector3 end, OnPathComplete callback = null)
        {
            var path = PathPool.GetPath<ABPath>();
            path.SetUp(start, end, callback);
            return path;
        }
        #endregion

        private void SetUp(Vector3 start, Vector3 end, OnPathComplete callback)
        {
            OnComplete = callback;
            UpdateStartEnd(start, end);
        }

        private void UpdateStartEnd(Vector3 start, Vector3 end)
        {
            m_StartPos = start;
            m_EndPos = end;
        }

        public override void Recycle()
        {
            OnComplete = null;
        }

        #region Path_API
        public override void InitPath()
        {
            
        }
        public override void Prepare()
        {
            
        }
        public override void Process()
        {
            
        }
        public override void CleanUP()
        {
            
        }
        #endregion
    }
}