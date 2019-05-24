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
            var heap = new PathNodeHeap(10);
            for (int i = 10; i > 0; i--)
                heap.Enqueue(new PathNode(), i);

            for (int i = 0; i < 10; i++)
            {
                var node = heap.Dequeue();
                Debug.Log(node.Priority + "\n");
            }
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