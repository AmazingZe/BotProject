namespace GameEditor
{
    using System.IO;

    using UnityEngine;
    using UnityEditor;

    using System.Collections.Generic;

    using GameRuntime;

    [CustomEditor(typeof(GameManager))]
    public class GameManagerWindow : EditorWindow
    {
        #region Properties
        public const string ConfigurePath = "Assets/Resources/ScriptableObjects/";

        private static List<Configure> Configures = new List<Configure>();
        #endregion

        #region EditorWindow_Callback
        [MenuItem("Pathfinding Manager/Open Window")]
        private static void Init()
        {
            GameManagerWindow window = EditorWindow.GetWindow<GameManagerWindow>();
            window.Show();
        }
        private void OnEnable()
        {
            Configures.Clear();
            CheckConfigure();
        }
        private void OnDisable()
        {

        }
        private void OnGUI()
        {
            if (Configures.Count == 0) return;

            for (int i = 0; i < Configures.Count; i++)
                Configures[i].OnGUI();
        }
        #endregion

        #region Draw_Area

        #endregion

        private void CheckConfigure()
        {
            if (new DirectoryInfo(ConfigurePath).Exists == false)
                Debug.LogError("Path Not Exist!");
            Configures.Add(LoadScriptableObject<RVOConfigure>("RVOConfigure.asset"));
            Configures.Add(LoadScriptableObject<MoveConfigure>("MoveConfigure.asset"));
        }
        private type LoadScriptableObject<type>(string path) 
                                                where type : ScriptableObject
        {
            type instance;
            string assetPath = ConfigurePath + path;
            instance = AssetDatabase.LoadAssetAtPath<type>(assetPath);
            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance<type>();
                AssetDatabase.CreateAsset(instance, assetPath);
            }

            return instance;
        }
    }
}