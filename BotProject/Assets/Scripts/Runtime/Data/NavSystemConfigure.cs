namespace GameRuntime
{
    using UnityEngine;
    using UnityEditor;

    using GameAI.Pathfinding.Core;

    public class NavSystemConfigure : Configure
    {
        #region Properties
        public GridGraph gridGraph;

        public int Width;
        public int Depth;
        public int NodeSize;
        public Vector3 Center;
        public float MaxSlope;
        #endregion

        protected override void OnDraw()
        {
            Width = EditorUtils.IntFieldWithLabel("Map Width", Width);
            Depth = EditorUtils.IntFieldWithLabel("Map Depth", Depth);
            NodeSize = EditorUtils.IntFieldWithLabel("Node Size", NodeSize);
            MaxSlope = EditorUtils.FloatFieldWithLabel("Max Slope", MaxSlope);
            Center = EditorGUILayout.Vector3Field("Map Center", Center);
        }
    }
}