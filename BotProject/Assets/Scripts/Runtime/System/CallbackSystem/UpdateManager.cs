namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    public class UpdateManager : SingletonMono<UpdateManager>
    {
        #region Properties
        private List<ISystem> m_systems;
        #endregion

        public override void OnInit()
        {
            m_systems = new List<ISystem>();
        }

        private void Update()
        {
            foreach (var system in m_systems)
                system.OnUpdate();
        }

        #region Public_API
        public void RegisterSystem(ISystem system)
        {

        }
        #endregion
    }
}