namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameAI.Pathfinding.Core;

    public class GameManager : MonoBehaviour
    {
        #region Properties
        private List<ISystem> m_systems = new List<ISystem>();
        public Transform target;
        #endregion

        #region Unity_Callbacks
        private void Start()
        {
            
        }
        private void Update()
        {
            
        }
        private void OnDisable()
        {
            foreach (var system in m_systems)
                system.OnRelease();
        }
        #endregion

        #region Public_API
        public void RegisterSystem(ISystem system)
        {

        }
        #endregion
    }
}