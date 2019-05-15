namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameRuntime.NavSystem;

    public enum TestEnum
    {
        testa,
        testb,
        testc
    }

    public class GameManager : MonoBehaviour
    {
        #region Properties
        private List<ISystem> m_systems = new List<ISystem>();
        #endregion

        #region Unity_Callbacks
        private void Start()
        {
            m_systems.Add(NavigationSystem.Instance);
        }
        private void Update()
        {
            foreach (var system in m_systems)
                system.OnUpdate();
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