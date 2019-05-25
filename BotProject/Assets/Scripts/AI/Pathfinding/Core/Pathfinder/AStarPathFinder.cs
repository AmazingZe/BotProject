namespace GameAI.Pathfinding.Core
{
    using System.Collections.Generic;

    using GameUtils;

    public class AStarPathFinder<NodeType> : Singleton<AStarPathFinder<NodeType>>, IPathfinder 
                                             where NodeType : NavNode
    {
        #region Properties
        private long m_MaxTick = 10000;
        private List<NavNode> neighbors = new List<NavNode>();
        #endregion

        #region ISingleton
        private AStarPathFinder() { }
        public override void OnInit()
        {
            
        }
        public override void OnRelease()
        {

            base.OnRelease();
        }
        #endregion

        #region IPathFinder
        public void CalculateStep(Path path)
        {
            PathNode curNode = path.CurNode;
            PathHandler handler = curNode.Handler;
            PathNodeHeap heap = handler.Heap;

            curNode = path.CurNode;
            NavNode node = curNode.Node;

            node.GetNeighbor(neighbors);
            for (int i = 0; i < neighbors.Count; i++)
            {
                NavNode tmpNode = neighbors[i];
                if (tmpNode == null) continue;

                PathNode tmpPNode = handler.GetPathNode(tmpNode.NodeIndex);
                int tmpCost = node.GetNeighborCost(i);
                tmpPNode.Parent = curNode;
                tmpPNode.G = curNode.F + tmpCost;
                //tmpPNode.H
                heap.Enqueue(tmpPNode, tmpPNode.F);
            }

            path.CurNode = heap.Dequeue();
        }
        #endregion
    }
}