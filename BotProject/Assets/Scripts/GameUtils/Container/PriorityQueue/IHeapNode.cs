namespace GameUtils
{
    public interface IHeapNode
    {
        float Priority { get; set; }
        int HeapIndex { get; set; }
    }
}