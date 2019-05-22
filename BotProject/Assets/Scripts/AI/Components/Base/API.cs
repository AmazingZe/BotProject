namespace GameAI.Component
{
    public interface IComponent
    {
        int Priority { get; }

        #region Lifespan
        void OnInit();
        void OnUpdate();
        void OnRest();
        #endregion
    }
}