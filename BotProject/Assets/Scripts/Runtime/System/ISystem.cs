namespace GameRuntime
{
    public interface ISystem
    {
        int Priority { get; }

        void OnUpdate();
        void OnRelease();
    }
}