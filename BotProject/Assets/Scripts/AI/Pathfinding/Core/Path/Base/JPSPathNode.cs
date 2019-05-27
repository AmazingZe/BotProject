namespace GameAI.Pathfinding.Core
{
    public class JPSPathNode : PathNode
    {
        #region Properties

        #endregion

        public override void Open(Path path, IPathHandler pathHandler)
        {
            
        }

        public new static void Bake(IPathHandler handler, GridGraph graph)
        {
            //Todo: Deal with map
        }
    }
}