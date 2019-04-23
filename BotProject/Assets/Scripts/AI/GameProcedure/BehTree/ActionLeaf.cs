namespace GameProcedure.BehTree
{
    public class ActionLeaf : Action
    {
        private static int ACTION_READY = 0;
        private static int ACTION_RUNNING = 1;
        private static int ACTION_FINISHED = 2;

        protected class ActionLeafContext : ActionContext
        {
            public bool needExit;
            public int status;
            private object userData;

            public ActionLeafContext()
            {
                needExit = false;
                userData = null;

                status = ACTION_READY;
            }

            public T GetUserData<T>() where T : class, new()
            {
                if (ReferenceEquals(userData, null))
                    return new T();
                else
                    return (T)userData;
            }
        }

        public ActionLeaf() : base(0) { }

        protected T GetContextUserData<T>(WorkData data) where T : class, new()
        {
            return GetContext<ActionLeafContext>(data).GetUserData<T>();
        }

        protected sealed override RunningStatus OnUpdate(WorkData data)
        {
            var runningStatus = RunningStatus.Finished;

            ActionLeafContext context = GetContext<ActionLeafContext>(data);
            if(context.status == ACTION_READY)
            {
                OnEnter(data);
                context.status = ACTION_RUNNING;
                context.needExit = true;
            }
            if(context.status == ACTION_RUNNING)
            {
                runningStatus = OnExecute(data);
                if (runningStatus == RunningStatus.Finished)
                    context.status = ACTION_FINISHED;
            }
            if(context.status == ACTION_FINISHED)
            {
                if (context.needExit)
                    OnExit(data, runningStatus);
                context.needExit = false;
                context.status = ACTION_READY;
            }

            return runningStatus;
        }
        protected sealed override void OnTransition(WorkData data)
        {
            ActionLeafContext context = GetContext<ActionLeafContext>(data);

            if (context.needExit)
                OnExit(data, RunningStatus.Transition);
            context.needExit = false;
            context.status = ACTION_READY;
        }

        #region User-Defined
        protected virtual void OnEnter(WorkData data) { }
        protected virtual RunningStatus OnExecute(WorkData data)
        {
            return RunningStatus.Finished;
        }
        protected virtual void OnExit(WorkData data, RunningStatus status) { }
        #endregion
    }
}