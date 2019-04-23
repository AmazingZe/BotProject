namespace GameProcedure.BehTree
{
    public class ActionLoop : Action
    {
        #region Properties
        private const int INFINITY = -1;

        private int m_LoopCount;
        #endregion

        protected class ActionLoopContext : ActionContext
        {
            public int currentLoop;

            public ActionLoopContext() { currentLoop = 0; }
        }

        public ActionLoop() : base(1) { }

        public ActionLoop SetLoopCount(int count)
        {
            m_LoopCount = count;
            return this;
        }

        protected override bool OnEvaluate(WorkData data)
        {
            var context = GetContext<ActionLoopContext>(data);

            var needLoop = (m_LoopCount == INFINITY) || (context.currentLoop < m_LoopCount);
            if (!needLoop) return false;
            if (IsIndexVaild(0))
            {
                var node = GetChild<Action>(0);
                return node.Evaluate(data);
            }
            return false;
        }
        // Todo:Recycle?
        protected override RunningStatus OnUpdate(WorkData data)
        {
            var context = GetContext<ActionLoopContext>(data);
            RunningStatus status = RunningStatus.Finished;

            if (IsIndexVaild(0))
            {
                var node = GetChild<Action>(0);
                status = node.Update(data);
                if(status == RunningStatus.Finished)
                {
                    context.currentLoop++;
                    if (context.currentLoop < m_LoopCount || m_LoopCount == INFINITY)
                        status = RunningStatus.Running;
                }
            }
            return status;
        }
        protected override void OnTransition(WorkData data)
        {
            var context = GetContext<ActionLoopContext>(data);

            if (IsIndexVaild(0))
            {
                var node = GetChild<Action>(0);
                node.Transition(data);
            }
            context.currentLoop = 0;
        }
    }
}