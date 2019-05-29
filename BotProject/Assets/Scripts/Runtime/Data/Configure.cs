namespace GameRuntime
{
    using UnityEngine;
    using UnityEditor;

    public abstract class Configure : ScriptableObject
    {
        public virtual string TitleName
        {
            get { return ""; }
        }

        public void OnGUI()
        {
            if (!string.IsNullOrEmpty(TitleName))
                EditorGUILayout.LabelField("====================" + TitleName + "====================");

            EditorGUI.BeginChangeCheck();
            OnDraw();
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(this);
        }

        protected abstract void OnDraw();
    }
}