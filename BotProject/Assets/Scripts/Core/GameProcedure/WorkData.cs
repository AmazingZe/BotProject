namespace GameProcedure
{
    using System.Collections.Generic;

    public class WorkData
    {
        #region Properties
        internal Dictionary<int, ActionContext> Context;
        #endregion

        public WorkData()
        {
            Context = new Dictionary<int, ActionContext>();
        }
    }

    public class ActionContext { }
}