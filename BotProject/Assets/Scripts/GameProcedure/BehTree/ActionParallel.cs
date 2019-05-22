namespace GameProcedure.BehTree
{
    using System.Collections.Generic;

    public enum ParallelPattern { AND, OR }

    public class ActionParallel : Action
    {
        #region Properties
        private ParallelPattern m_EvaluatePattern;
        private ParallelPattern m_RunningPattern;
        #endregion

        public ActionParallel() : base(-1)
        {
            m_EvaluatePattern = ParallelPattern.AND;
            m_RunningPattern = ParallelPattern.OR;
        }
        public ActionParallel SetEvaluatePattern(ParallelPattern pattern)
        {
            m_EvaluatePattern = pattern;
            return this;
        }
        public ActionParallel SetRunningPattern(ParallelPattern pattern)
        {
            m_RunningPattern = pattern;
            return this;
        }

        protected class ActionParallelContext : ActionContext
        {
            public List<bool> evaluateStatus;
            public List<RunningStatus> runningStatus;

            public ActionParallelContext()
            {
                runningStatus = new List<RunningStatus>();
                evaluateStatus = new List<bool>();
            }
        }

        protected override bool OnEvaluate(WorkData data)
        {
            var context = GetContext<ActionParallelContext>(data);
            ResetList(context.evaluateStatus, false);
            int len = ChildCount;
            bool finalResult = false;

            for (int i = 0; i < len; i++)
            {
                var node = GetChild<Action>(i);
                bool result = node.Evaluate(data);
                if (m_EvaluatePattern == ParallelPattern.AND && !result)
                {
                    finalResult = false;
                    break;
                }
                if(result)
                    finalResult = true;
                context.evaluateStatus[i] = result;
            }

            return finalResult;
        }
        protected override RunningStatus OnUpdate(WorkData data)
        {
            var context = GetContext<ActionParallelContext>(data);
            int len = ChildCount;
            bool hasFinished = false;
            bool isExecuting = false;

            if (context.runningStatus.Count != len)
                ResetList(context.runningStatus, RunningStatus.Running);

            for (int i = 0; i < len; i++)
            {
                if (!context.evaluateStatus[i]) continue;

                if(context.runningStatus[i] == RunningStatus.Finished)
                {
                    hasFinished = true;
                    continue;
                }

                var node = GetChild<Action>(i);
                RunningStatus status = node.Update(data);
                if(status == RunningStatus.Finished) 
                    hasFinished = true; 
                else 
                    isExecuting = true;
                context.runningStatus[i] = status;
            }
            if ((m_RunningPattern == ParallelPattern.AND && !isExecuting) ||
                (m_RunningPattern == ParallelPattern.OR && hasFinished))
            {
                ResetList(context.runningStatus, RunningStatus.Running);
                return RunningStatus.Finished;
            }

            return RunningStatus.Running;
        }
        protected override void OnTransition(WorkData data)
        {
            var context = GetContext<ActionParallelContext>(data);

            for(int i = 0; i < ChildCount; i++)
            {
                var node = GetChild<Action>(i);
                node.Transition(data);
            }

            ResetList(context.runningStatus, RunningStatus.Running);
        }

        private void ResetList<T>(List<T> list, T value)
        {
            int len = ChildCount;
            if (list.Count != ChildCount)
            {
                list.Clear();
                for (int i = 0; i < len; i++)
                    list.Add(value);
            }
            else
                for (int i = 0; i < len; i++)
                    list[i] = value;
        }
    }
}