namespace GameEditor
{
    using UnityEditor;
    using UnityEngine;

    using GameRuntime;
    using GameAI.Pathfinding.Core;

    [CustomEditor(typeof(NavSystem))]
    public class NavSystemEditor : Editor
    {
        #region Properties
        private static NavSystemConfigure Configure = null;
        private GridGraph m_Graph;
        private int m_Width, m_Depth, m_NodeSize;
        #endregion

        #region Editor_Callback
        private void OnEnable()
        {
            if(Configure == null)
                Configure = AssetDatabase.LoadAssetAtPath<NavSystemConfigure>(GameManagerWindow.ConfigurePath + "NavSystemConfigure.asset");
        }
        #endregion

        public override void OnInspectorGUI()
        {
            NavSystem system = target as NavSystem;

            if (Configure == null)
                Configure = AssetDatabase.LoadAssetAtPath<NavSystemConfigure>(GameManagerWindow.ConfigurePath + "NavSystemConfigure.asset");
            if (Configure == null)
            {
                Configure = ScriptableObject.CreateInstance<NavSystemConfigure>();
                AssetDatabase.CreateAsset(Configure, "Assets/Resources/ScriptableObjects/NavSystemConfigure.asset");
            }

            Configure.OnGUI();

            if (GUILayout.Button("Bake"))
                Bake();

            base.OnInspectorGUI();
        }
        private void Bake()
        {
            m_Width = Configure.Width;
            m_Depth = Configure.Depth;
            m_NodeSize = Configure.NodeSize;

            m_Graph = GridGraph.CreateGraph(m_Width, m_Depth, m_NodeSize, Configure.Center);
            
            m_Graph.MaxSlope = Configure.MaxSlope;

            for (int z = 0; z < m_Depth; z++)
            {
                for (int x = 0; x < m_Width; x++)
                {
                    m_Graph.RecalculateCell(x, z);
                }
            }
            Configure.gridGraph = m_Graph;
            EditorUtility.SetDirty(Configure);
        }
        
    }
}