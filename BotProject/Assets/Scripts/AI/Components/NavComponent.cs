namespace GameAI.Component
{
    using GameRuntime;
    using GameAI.Pathfinding.Core;

    public class NavComponent : IComponent
    {
        #region Properties
        private readonly OnPathComplete m_OnPathComplete;
        private readonly OnPathComplete m_OnPathPartialComplete;

        private OnPathComplete m_TmpPathCallback;
        private Path m_CurPath;
        private int m_LastPathID;
        #endregion

        #region Constructor
        public static NavComponent Create()
        {
            NavComponent retMe = new NavComponent();

            return retMe;
        }
        private NavComponent()
        {
            m_OnPathComplete = OnPathComplete;
            m_OnPathPartialComplete = OnPartialPathComplete;
        }
        #endregion

        #region IComponent
        public IBot Owner { get; set; }
        public int Priority { get; set; }
        public MsgCenter Msgcenter { get; set; }

        public void OnInit()
        {

        }
        public void OnNotify(int msgID)
        {

        }
        public void OnRelease()
        {

        }
        public void OnUpdate()
        {

        }

        public void SetOwner(BotBehaviour owner)
        {
            Owner = owner;
        }
        #endregion

        private void OnPathComplete(Path path) { _OnPathComplete(path, true, true); }
        private void OnPartialPathComplete(Path path) { _OnPathComplete(path, true, false); }
        private void _OnPathComplete(Path path, bool runModifier, bool sendCallback)
        {
            //Todo: Post-Processing Modifier;

        }

        private void _StartPath(Path path, OnPathComplete callback)
        {
            path.OnComplete += callback;
            m_CurPath = path;
            m_TmpPathCallback = callback;

            m_LastPathID = m_CurPath.PathID;

            _RunModify(m_CurPath);

            //Todo: Send to NavSystem
        }

        private void _RunModify(Path path)
        {

        }
    }
}