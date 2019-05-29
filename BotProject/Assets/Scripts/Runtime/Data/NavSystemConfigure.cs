namespace GameRuntime
{
    using UnityEngine;
    using UnityEditor;

    public class NavSystemConfigure : Configure
    {
        #region Properties
        public int Width;
        public int Depth;
        public int NodeSize;
        public Vector3 Center;
        #endregion

        protected override void OnDraw()
        {
            Width = EditorUtils.IntFieldWithLabel("Map Width", Width);
            Depth = EditorUtils.IntFieldWithLabel("Map Depth", Depth);
            NodeSize = EditorUtils.IntFieldWithLabel("Node Size", NodeSize);
            Center = EditorGUILayout.Vector3Field("Map Center", Center);
        }
    }
}