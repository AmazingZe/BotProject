namespace GameRuntime
{
    using UnityEngine;

    public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        #region Properties
        private static T m_instance = null;
        private static bool m_instanciated = false;

        public T Instance
        {
            get
            {
                if (m_instanciated) return m_instance;

                if (m_instance == null)
                {
                    var targetObj = GameObject.Find("GameManager");

                    m_instance = targetObj.AddComponent<T>();
                    m_instance.OnInit();
                    m_instanciated = true;
                }

                return m_instance;
            }
        }
        #endregion

        public abstract void OnInit();
    }
}