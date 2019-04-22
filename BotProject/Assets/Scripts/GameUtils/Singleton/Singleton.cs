namespace GameUtils
{
    using GameUtils.Singleton;

    public interface ISingleton
    {
        void OnInit();
    }

    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        #region Properties
        private static T _Instance = null;
        protected static object Locker = new object();

        public static T Instance
        {
            get {
                if(_Instance == null)
                {
                    lock (Locker)
                    {
                        if(_Instance == null) 
                            _Instance = SingletonCreator.Create<T>(); 
                    }
                }
                return _Instance;
            }
        }
        #endregion

        public abstract void OnInit();
        public virtual void OnRelease() { }
    }
}