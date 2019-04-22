namespace GameUtils
{
    public class Pool<T> : Singleton<Pool<T>>
    {
        #region Properties
        private T[] m_Pool;

        #endregion

        #region API
        public static void Recycle(T item)
        {
            if(item == null) 
        }
        public static T Allocate()
        {

        }
        #endregion

        
    }
}