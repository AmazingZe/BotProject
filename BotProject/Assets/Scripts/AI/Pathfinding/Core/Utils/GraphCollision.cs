namespace GameAI.Pathfinding.Core
{
    using UnityEngine;

    public class GraphCollision
    {
        public float diameter = 1F;
        public float height = 2F;
        public float fromHeight = 100;
        public float collisionOffset;

        public bool collisionCheck = true;
        public bool heightCheck = true;

        public Vector3 up;
        private Vector3 upheight;
        private float finalRadius;

        public void Initialize(GraphTransform transform, float scale)
        {
            var tmpUp = transform.Transform(Vector3.up);
            var tmp = transform.Transform(Vector3.zero);
            up = (tmpUp - tmp).normalized;
            upheight = up * height;
            finalRadius = diameter * scale * 0.5F;
        }

        public bool Check(Vector3 position)
        {
            position += up * collisionOffset;

            bool result = Physics.CheckSphere(position, finalRadius);
            return !result;
        }

        public Vector3 CheckHeight(Vector3 position, out RaycastHit hit, out bool walkable)
        {
            walkable = false;

            if (Physics.Raycast(position + up * fromHeight, -up, out hit, fromHeight + 0.005F))
            {
                walkable = true;
                return hit.point;
            }
                
            return position;
        }
    }
}