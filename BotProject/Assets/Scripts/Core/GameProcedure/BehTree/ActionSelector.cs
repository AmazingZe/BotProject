namespace GameProcedure.BehTree
{
    public class ActionSelector : Action
    {
        #region Properties

        #endregion

        protected class ActionSelectorContext : ActionContext
        {
            public int lastIndex;
            public int currentIndex;

            public ActionSelectorContext() { lastIndex = currentIndex = -1; }
        }

        public ActionSelector() : base(-1) { }

        protected override bool OnEvaluate(WorkData data)
        {
            var context = GetContext<ActionSelectorContext>(data);
            context.currentIndex = -1;
            for (int i = 0; i < ChildCount; i++)
            {
                var node = GetChild<Action>(i);
                if (node.Evaluate(data))
                {
                    context.currentIndex = i;
                    return true;
                }
            }

            return false;
        }
        protected override RunningStatus OnUpdate(WorkData data)
        {
            var context = GetContext<ActionSelectorContext>(data);
            int lastIndex = context.lastIndex;
            int currentIndex = context.currentIndex;
            RunningStatus status = RunningStatus.Finished;
            
            if (lastIndex != currentIndex)
            {
                if (IsIndexVaild(lastIndex))
                {
                    var node = GetChild<Action>(lastIndex);
                    node.Transition(data);
                }
                context.lastIndex = context.currentIndex;
            }

            if (IsIndexVaild(currentIndex))
            {
                var node = GetChild<Action>(currentIndex);
                status = node.Update(data);
                if (status == RunningStatus.Finished)
                    context.lastIndex = -1;
            }

            return status;
        }
        protected override void OnTransition(WorkData data)
        {
            var context = GetContext<ActionSelectorContext>(data);
            var node = GetChild<Action>(context.lastIndex);
            if (node != null) node.Transition(data);
        }
    }
}