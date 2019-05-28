namespace GameRuntime
{
    using UnityEngine;

    using System.Collections.Generic;

    using GameAI.Component;
    using GameAI.Utils;

    public class BotBehaviour : MonoBehaviour, IBot
    {
        #region IBot_API
        private MoveContoller Movement;
        #endregion

        #region Unity_Callback
        private void Start()
        {
            Movement = MoveContoller.Create();
            Movement.SetOwner(this);
        }
        private void Update()
        {
            Movement.OnUpdate();
        }
        #endregion

        #region Move

        #endregion
    }
}