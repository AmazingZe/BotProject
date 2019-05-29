namespace GameAI.Pathfinding.RVO
{
    using UnityEngine;

    public interface IAgent
    {
        Vector2 Position { get; set; }
        float ElevationCoordinate { get; set; }
        Vector2 CalculatedTargetPoint { get; }
        float CalculatedSpeed { get; }
        bool Locked { get; set; }
        float Radius { get; set; }
        float Height { get; set; }
        float AgentTimeHorizon { get; set; }
        float ObstacleTimeHorizon { get; set; }
        int MaxNeighbours { get; set; }
        int NeighbourCount { get; }
        float Priority { get; set; }
        bool DebugDraw { get; set; }
        System.Action PreCalculationCallback { set; }
        void SetCollisionNormal(Vector2 normal);
        void ForceSetVelocity(Vector2 velocity);

        void SetTarget(Vector2 targetPoint, float desiredSpeed, float maxSpeed);

    }
}