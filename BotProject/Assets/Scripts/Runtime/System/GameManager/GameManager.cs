namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameRuntime.NavSystem;

    public class GameManager : MonoBehaviour
    {
        #region Properties
        private List<ISystem> m_systems = new List<ISystem>();
        public Transform target;
        #endregion

        #region Unity_Callbacks
        private void Start()
        {
            m_systems.Add(NavigationSystem.Instance);
        }
        private void Update()
        {
            Vector3 relativePos = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = rotation;

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