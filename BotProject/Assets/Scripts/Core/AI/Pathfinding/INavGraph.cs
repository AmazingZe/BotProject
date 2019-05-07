namespace GameAI.Pathfinding
{
    using System.Collections;

    public interface INavGraph
    {
        void OnDestroy();
        IEnumerable AsyncScan();

    }
}