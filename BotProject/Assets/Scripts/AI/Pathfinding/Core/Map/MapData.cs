namespace GameAI.Pathfinding.Core
{
    using UnityEditor;
    using UnityEngine;

    public class MapData : ScriptableObject
    {
        #region Properties
        private NavGraph[] m_Graphs = new NavGraph[0];
        #endregion

        #region Callback
        private void OnEnable()
        {

        }
        private void OnDisable()
        {
            
        }
        private void OnDestroy()
        {
            
        }
        #endregion

        #region Public_API
        public int Count
        {
            get { return m_Graphs.Length; }
        }
        public NavGraph this[int index]
        {
            get
            {
                CheckIndexValid(index);
                return m_Graphs[index];
            }
        }
        #endregion

        private void CheckIndexValid(int index)
        {
            if (index >= m_Graphs.Length || index < 0)
                throw new System.IndexOutOfRangeException();
        }
    }
}