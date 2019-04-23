namespace GameProcedure.BehTree
{
    public class ActionSequence : Action
    {
        #region Properties
        private bool m_ContinueWhenError;
        #endregion

        public ActionSequence(int num, bool continueWhenError) : base(num)
        {
            m_ContinueWhenError = continueWhenError;
        }

        protected class ActionSequenceContext : ActionContext
        {
            public int currentIndex;

            public ActionSequenceContext() { currentIndex = -1; }
        }

        protected override bool OnEvaluate(WorkData data)
        {
            var context = GetContext<ActionSequenceContext>(data);
            var index = context.currentIndex;
            var currentIndex = 0;

            if (IsIndexVaild(index)) currentIndex = index;

            if (IsIndexVaild(currentIndex))
            {
                var node = GetChild<Action>(currentIndex);
                if (node.Evaluate(data))
                {
                    context.currentIndex = currentIndex;
                    return true;
                }
            }

            return false;
        }
        protected override RunningStatus OnUpdate(WorkData data)
        {
            var context = GetContext<ActionSequenceContext>(data);
            var index = context.currentIndex;
            RunningStatus status = RunningStatus.Finished;

            var node = GetChild<Action>(index);
            status = node.Update(data);

            //Todo:ErrorCheck
            if (!m_ContinueWhenError)
            {
                context.currentIndex = -1;
                return status;
            }

            if(status == RunningStatus.Finished)
            {
                context.currentIndex++;
                if (IsIndexVaild(context.currentIndex))
                    status = RunningStatus.Running;
                else
                    context.currentIndex = -1;
            }

            return status;
        }
        protected override void OnTransition(WorkData data)
        {
            var context = GetContext<ActionSequenceContext>(data);
            var node = GetChild<Action>(context.currentIndex);
            if (node != null)
                node.Transition(data);
            context.currentIndex = -1;
        }
    }
}