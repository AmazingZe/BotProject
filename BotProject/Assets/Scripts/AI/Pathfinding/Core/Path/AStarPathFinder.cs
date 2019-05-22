namespace GameAI.Pathfinding.Core
{
    using System;

    using GameUtils;

    public class AStarPathFinder<NodeType> : Singleton<AStarPathFinder<NodeType>>, IPathfinder 
                                             where NodeType : NavNode
    {
        #region Properties
        private long m_MaxTick = 10000;
        #endregion

        #region ISingleton
        private AStarPathFinder() { }
        public override void OnInit()
        {
            
        }
        public override void OnRelease()
        {

            base.OnRelease();
        }
        #endregion

        #region IPathFinder
        public void CalculateStep(Path path)
        {
            
        }
        #endregion
    }
}