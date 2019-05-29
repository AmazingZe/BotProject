namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameAI.Component;

    public class BotBehaviour : MonoBehaviour, IBot
    {
        #region IBot_API
        public MoveContoller Movement;
        #endregion

        #region Unity_Callback
        private void Start()
        {
            Movement = MoveContoller.Create(this);
        }
        private void Update()
        {
            Movement.OnUpdate();
        }
        #endregion

        #region Move
        public void SetPath(List<Vector3> path)
        {
            Movement.SetPath(path);
        }
        #endregion
    }
}