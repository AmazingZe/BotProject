namespace GameUtils.Singleton
{
    using System;
    using System.Reflection;

    public static class SingletonCreator
    {
        public static T Create<T>() where T : class, ISingleton
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

            if (ctor == null)
                throw new Exception("Non-Public Constructor() not found! in " + typeof(T));

            T retMe = ctor.Invoke(null) as T;
            retMe.OnInit();
            return retMe;
        }
    }
}