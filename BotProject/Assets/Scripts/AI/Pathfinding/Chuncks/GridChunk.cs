namespace GameAI.Pathfinding
{
    using UnityEngine;

    using GameUtils;

    public class GridChunk<T> : Singleton<GridChunk<T>> where T : INavGraph
    {
        #region Properties
        private Chunck<T>[] m_Chuncks;
        private int m_ChunkNum;
        private Vector3[] m_Pivots;
        private Rect m_Bound;
        
        private int m_Dimention = 3;
        public int Dimention
        {
            get { return m_Dimention; }
            set { m_Dimention = value; }
        }

        private int m_ChunkSize = 200;
        public int ChunkSize
        {
            get { return m_ChunkSize; }
            set { m_ChunkSize = value; }
        }

        private int m_GridSize = 1;
        public int GridSize
        {
            get { return m_GridSize; }
            set { m_GridSize = value; }
        }
        #endregion

        #region Singleton
        private GridChunk() { }
        public override void OnInit() { }
        #endregion

        #region Public_API
        public static GridChunk<T> Initialize(Vector3 center)
        {
            return Instance._Initialize(center);
        }
        #endregion

        #region Update_With_Dir
        public void UpdateUp()
        {

        }
        public void UpdateRight()
        {

        }
        public void UpdateDown()
        {

        }
        public void UpdateLeft()
        {

        }
        #endregion

        private GridChunk<T> _Initialize(Vector3 center)
        {
            m_ChunkNum = m_Dimention * m_Dimention;
            int offset = m_GridSize * m_ChunkSize;

            m_Chuncks = new Chunck<T>[m_ChunkNum];
            for (int i = 0; i < m_ChunkNum; i++)
                m_Chuncks[i] = new Chunck<T>();

            m_Pivots = new Vector3[4];
            m_Pivots[0] = center + new Vector3(-offset / 2, 0, offset / 2);
            m_Pivots[0] = center + new Vector3(offset / 2, 0, offset / 2);
            m_Pivots[0] = center + new Vector3(offset / 2, 0, -offset / 2);
            m_Pivots[0] = center + new Vector3(-offset / 2, 0, -offset / 2);

            m_Bound = new Rect(new Vector2(center.x - offset * m_Dimention / 2, center.z - offset * m_Dimention / 2),
                               new Vector2(offset * m_Dimention, offset * m_Dimention));
            return this;
        }
    }
}