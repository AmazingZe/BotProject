namespace GameAI.Pathfinding
{
    using System.Collections;

    public abstract class NavGraph : INavGraph
    {
        #region Properties
        
        #endregion

        #region API
        public abstract void GetNodes(System.Action<NavNode> action);
        #endregion

        #region INavGraph
        public abstract void OnDestroy();
        public abstract IEnumerable AsyncScan();
        #endregion
    }
}