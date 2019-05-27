namespace GameAI.Pathfinding.Core
{
    using System;

    public interface IPathHandler
    {
        PathNodeHeap Heap { get; }
        int PathID { get; }
        Heuristic HeuristicType { get; set; }
        AlgorithmType SearchType { get; set; }

        IPathNode GetPathnode(NavNode node);
        void Init(Path path);
        void InitializeNode(NavNode node);
        void ClearNode(NavNode node);
        void SetNodeParam(Action<IPathNode> action);
    }
}