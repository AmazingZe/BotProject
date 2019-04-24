namespace GameRuntime
{
    using GameAI.Component;

    using UnityEngine;

    using System.Collections.Generic;

    public class Controller : MonoBehaviour
    {
        #region Properties
        private List<IComponent> m_Components;
        #endregion

        #region Unity-Callback
        private void Start()
        {
            m_Components = new List<IComponent>();
            var components = GetComponents<IComponent>();
            
        }
        private void Update()
        {
            
        }
        #endregion
    }
}