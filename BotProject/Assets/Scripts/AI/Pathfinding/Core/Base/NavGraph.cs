namespace GameAI.Pathfinding.Core
{
    using System.Collections.Generic;

    public abstract class NavGraph : INavGraph
    {
        #region Properties

        #endregion

        #region INavGraph
        public abstract void GetNodes(System.Action<NavNode> action);
        public abstract void OnDestroy();
        public abstract void GetNeighbor(int nodeIndex, List<NavNode> list);
        public abstract void BakeGraph2Handler(IPathHandler handler);
        #endregion
    }
}