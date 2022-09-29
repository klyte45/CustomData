using ColossalFramework.UI;
using Kwytto.LiteUI;
using Kwytto.UI;
using Kwytto.Utils;
using UnityEngine;

namespace CustomData.UI
{

    internal class CDMainWindow : GUIOpacityChanging
    {
        public static CDMainWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObjectUtils.CreateElement<CDMainWindow>(UIView.GetAView().transform);
                    instance.Init(ModInstance.Instance.GeneralName, new Rect(128, 128, 680, 420), resizable: true, minSize: new Vector2(440, 260));
                    var tabs = new IGUIVerticalITab[] {
                    };
                    instance.m_tabsContainer = new GUIVerticalTabsContainer(tabs);
                    instance.Visible = false;
                }
                return instance;
            }
        }
        protected override bool showOverModals => false;

        protected override bool requireModal => false;

        private GUIVerticalTabsContainer m_tabsContainer;
        private static CDMainWindow instance;


        protected override void DrawWindow(Vector2 size)
        {
            m_tabsContainer.DrawListTabs(new Rect(default, size), 200);
        }
        protected override void OnWindowOpened()
        {
            base.OnWindowOpened();
            m_tabsContainer?.Reset();
        }

        protected override void OnWindowDestroyed()
        {
            instance = null;
        }
    }
}
