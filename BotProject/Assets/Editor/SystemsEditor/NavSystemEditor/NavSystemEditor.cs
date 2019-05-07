namespace GameEditor.NavSystem
{
    using UnityEditor;

    using GameRuntime.NavSystem;

    [CustomEditor(typeof(NavigationSystem))]
    public class NavSystemEditor : Editor
    {

        #region UnityEditor_Callbacks
        private void OnSceneGUI()
        {
            
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
        #endregion
    }
}