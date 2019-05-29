namespace GameEditor
{
    using UnityEditor;
    using UnityEngine;

    using GameRuntime;

    [CustomEditor(typeof(NavSystem))]
    public class NavSystemEditor : Editor
    {
        #region Properties
        private static NavSystemConfigure Configure = null;
        #endregion

        #region Editor_Callback
        private void OnEnable()
        {
            if(Configure == null)
                Configure = AssetDatabase.LoadAssetAtPath<NavSystemConfigure>(GameManagerWindow.ConfigurePath + "NavSystemConfigure.asset");
        }
        #endregion
        private void OnSceneGUI()
        {
            
        }

        public override void OnInspectorGUI()
        {
            NavSystem system = target as NavSystem;

            Configure.OnGUI();

            if (GUILayout.Button("Bake"))
                system.Bake();
        }
    }
}