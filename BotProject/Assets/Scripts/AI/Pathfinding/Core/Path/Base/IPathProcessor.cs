namespace GameAI.Pathfinding.Core
{
    using System;

    public interface IPathProcessor
    {
        void SetSearchType(AlgorithmType type);
        void SetMaxFrameTime(float time);

        void InitNode(NavNode node);
        void DestoryNode(NavNode node);
        void NonMultiThreadTick();

        void BakeGraph2Handler(NavGraph mapType);
    }
}