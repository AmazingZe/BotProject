namespace GameAI.Component
{
    using GameRuntime;

    public interface IComponent
    {
        int Priority { get; }
        MsgCenter Msgcenter { get; set; }

        void OnInit();
        void OnUpdate();
        void OnRelease();

        void SetOwner(BotBehaviour owner);

        void OnNotify(int msgID);
    }
}