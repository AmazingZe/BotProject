namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameAI.Component;

    public class GameManager : MonoBehaviour
    {
        #region Properties
        private List<ISystem> m_systems = new List<ISystem>();
        private MsgCenter center;
        #endregion

        #region Unity_Callbacks
        private void Start()
        {
            center = new MsgCenter();
            center.OnInit();

            
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