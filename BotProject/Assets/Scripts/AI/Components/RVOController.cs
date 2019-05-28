namespace GameAI.Component
{
    using GameRuntime;

    public class RVOController : IComponent
    {


        #region IComponent
        public IBot Owner { get; set; }
        public int Priority { get; set; }
        public MsgCenter Msgcenter { get; set; }

        public void OnInit()
        {

        }
        public void OnNotify(int msgID)
        {

        }
        public void OnRelease()
        {

        }
        public void OnUpdate()
        {

        }

        public void SetOwner(BotBehaviour owner)
        {
            Owner = owner;
        }
        #endregion


    }
}