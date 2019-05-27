namespace GameAI.Pathfinding.Core
{
    using GameUtils.Heap;

    public interface IPathNode : IHeapNode
    {
        int PathID { get; set; }

        int G { get; set; }
        int H { get; set; }
        int F { get; }
        int Cost { get; set; }
        IPathHandler Handler { get; set; }
        NavNode Node { get; set; }
        IPathNode Parent { get; set; }
        void UpdateG();

        void Reset();
        void Open(Path path, IPathHandler pathHandler);
    }
}