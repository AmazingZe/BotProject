namespace GameRuntime.NavSystem
{
    using UnityEngine;

    public class NavigationSystem : SingletonMono<NavigationSystem>, ISystem
    {
        #region Properties

        private int m_Priority;
        #endregion

        public override void OnInit()
        {
            
        }

        #region ISystem
        int ISystem.Priority
        {
            get { return m_Priority; }
        }
        void ISystem.OnUpdate()
        {

        }
        void ISystem.OnRelease()
        {

        }
        #endregion

        #region Public_API

        #endregion
    }
}