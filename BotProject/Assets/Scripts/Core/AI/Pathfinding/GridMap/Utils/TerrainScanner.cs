namespace GameAI.Pathfinding.Utils
{
    using UnityEngine;

    public class TerrainScanner
    {
        #region Properties
        public float FromHeight = 2f;

        public Vector3 UpHeight;
        public Vector3 Up;
        #endregion

        #region Public_API
        public void Initialize(GraphTransform transform)
        {
            Up = (transform.TransformPoint(Vector3.up) - transform.TransformPoint(Vector3.zero)).normalized;
            UpHeight = Up * FromHeight;
        }

        public Vector3 CheckHeight(Vector3 position, out RaycastHit hit, out bool walkable)
        {
            walkable = true;

            if (Physics.Raycast(position + Up * FromHeight, -Up, out hit, FromHeight))
                return hit.point;

            walkable = false;
            return position;
        }
        #endregion
    }
}