namespace GameAI.Pathfinding.RVO
{
    using UnityEngine;

    public class ObstacleVertex
    {
        public bool ignore;
        
        public Vector3 position;
        public Vector2 dir;
        
        public float height;
        
        public ObstacleVertex next;

        public ObstacleVertex prev;
    }
}