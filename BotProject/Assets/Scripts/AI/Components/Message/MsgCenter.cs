namespace GameAI.Component
{
    using System;
    using System.Collections.Generic;

    using GameRuntime;

    public class MsgCenter : IComponent
    {
        #region Properties
        private int m_Priority;

        private List<IComponent> m_Components;
        private List<Type> m_Types;
        #endregion

        #region IComponent_API
        public IBot Owner { get; set; }
        public int Priority
        {
            get { return m_Priority; }
            set { m_Priority = value; }
        }
        public MsgCenter Msgcenter
        {
            get { return this; }
            set { int i = 1; }
        }

        public void OnInit()
        {
            m_Components = new List<IComponent>();
            m_Types = new List<Type>();
        }
        public void OnUpdate()
        {

        }
        public void OnRelease()
        {

        }
        public void SetOwner(BotBehaviour owner)
        {
            Owner = owner;
        }
        public void OnNotify(int msgID)
        {

        }
        #endregion

        #region Public_API
        public void Register<ComponentType>(ComponentType component)
                                           where ComponentType : IComponent
        {
            Type type = typeof(ComponentType);
            if (m_Types.Contains(type)) return;

            component.Msgcenter = this;

            m_Types.Add(type);
            m_Components.Add(component);
        }
        public void DeRegister<ComponentType>(ComponentType component)
                                             where ComponentType : IComponent
        {
            Type type = typeof(ComponentType);
            if (!m_Types.Contains(type)) return;

            component.Msgcenter = null;

            m_Types.Remove(type);
            m_Components.Remove(component);
        }

        public void BoardCast(int msgID)
        {
            for (int i = 0; i < m_Components.Count; i++)
                m_Components[i].OnNotify(msgID);
        }
        public void SendMsgTo<ComponentType>(int msgID) 
                                             where ComponentType : IComponent
        {
            Type type = typeof(ComponentType);
            int index = m_Types.IndexOf(type);
            if (index != -1)
                m_Components[index].OnNotify(msgID);
        }
        #endregion
    }
}