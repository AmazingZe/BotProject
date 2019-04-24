namespace Game.Map
{
    using UnityEngine;

    public class HexagonNode : NavNode
    {
        private Vector3 position;

        public HexagonNode(Vector3 position)
        {
            this.position = position;
        }
    }
}