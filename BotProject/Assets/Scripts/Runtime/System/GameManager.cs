namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameAI.Component;

    public class GameManager : MonoBehaviour
    {
        #region Properties
        private Camera mainCamera;
        private Vector3 m_ClickPosition = Vector3.zero;

        private List<ISystem> m_systems = new List<ISystem>();
        private BotBehaviour m_Bot;
        #endregion

        #region Unity_Callbacks
        private void Start()
        {
            mainCamera = Camera.main;
            m_Bot = GameObject.Find("Target").GetComponent<BotBehaviour>();
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                    m_ClickPosition = hit.point;

                m_Bot.SetPath(new List<Vector3>() { transform.position, m_ClickPosition });
            }
        }
        private void OnDisable()
        {
            foreach (var system in m_systems)
                system.OnRelease();
        }

        private void OnGUI()
        {
            GUILayout.Label("Position: " + m_ClickPosition.ToString());
        }
        #endregion

        #region Public_API
        public void RegisterSystem(ISystem system)
        {

        }
        #endregion
    }
}