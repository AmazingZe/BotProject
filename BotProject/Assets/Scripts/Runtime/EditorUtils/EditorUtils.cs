namespace GameRuntime
{
    using UnityEngine;
    using UnityEditor;

    public static class EditorUtils
    {
        private static GUILayoutOption LabelOption = GUILayout.Width(EditorGUIUtility.currentViewWidth / 3);
        private static GUILayoutOption FieldOption = GUILayout.Width(EditorGUIUtility.currentViewWidth / 2);

        public static int IntFieldWithLabel(string label, int value)
        {
            int retMe = 0;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, LabelOption);
            retMe = EditorGUILayout.IntField(value, FieldOption);
            EditorGUILayout.EndHorizontal();

            return retMe;
        }
        public static float FloatFieldWithLabel(string label, float value)
        {
            float retMe = 0;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, LabelOption);
            retMe = EditorGUILayout.FloatField(value, FieldOption);
            EditorGUILayout.EndHorizontal();

            return retMe;
        }
        public static bool BoolFieldWithLabel(string label, bool value)
        {
            bool retMe = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, LabelOption);
            retMe = EditorGUILayout.Toggle(value, FieldOption);
            EditorGUILayout.EndHorizontal();

            return retMe;
        }
    }
}