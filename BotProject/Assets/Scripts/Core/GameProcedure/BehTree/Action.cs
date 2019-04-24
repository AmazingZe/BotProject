namespace GameProcedure.BehTree
{
    public abstract class Action : Node
    {
        #region Properties
        private static int sUniqueIndex = 0;
        private static int GetUniqueIndex()
        {
            return ++sUniqueIndex;
        }

        protected int UniqueIndex;
        protected Condition Precondition;
        #endregion

        public Action(int maxChildNum) : base(maxChildNum)
        {
            UniqueIndex = GetUniqueIndex();
        }

        #region API
        public Action SetPrediction(Condition condition)
        {
            Precondition = condition;
            return this;
        }
        public bool Evaluate(WorkData data)
        {
            return (Precondition == null || Precondition.IsTrue(data)) && 
                   OnEvaluate(data);
        }
        public RunningStatus Update(WorkData data)
        {
            return OnUpdate(data);
        }
        public void Transition(WorkData data)
        {
            OnTransition(data);
        }

        public override int GetHashCode()
        {
            return UniqueIndex;
        }
        protected T GetContext<T>(WorkData data) where T : ActionContext, new()
        {
            int uniqueIndex = GetHashCode();
            T retMe;
            if (data.Context.ContainsKey(uniqueIndex))
                retMe = (T)data.Context[uniqueIndex];
            else
            {
                retMe = new T();
                data.Context.Add(uniqueIndex, retMe);
            }
            return retMe;
        }
        #endregion

        #region Implementation
        protected virtual bool OnEvaluate(WorkData data)
        {
            return true;
        }
        protected virtual RunningStatus OnUpdate(WorkData data)
        {
            return RunningStatus.Finished;
        }
        protected virtual void OnTransition(WorkData data)
        {

        }
        #endregion
    }
}