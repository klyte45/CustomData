using ColossalFramework.UI;
using CustomData.Xml;
using Kwytto.LiteUI;
using Kwytto.Utils;
using System.Linq;
using UnityEngine;

namespace CustomData.UI
{

    public class CDAddressWindow : GUIOpacityChanging
    {
        protected override bool showOverModals => false;
        protected override bool requireModal => false;
        protected override bool ShowCloseButton => false;
        public static CDAddressWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObjectUtils.CreateElement<CDAddressWindow>(UIView.GetAView().transform);
                    instance.Init(ModInstance.Instance.GeneralName, new Rect(256, 256, 100, 105), resizable: false, minSize: new Vector2(100, 105), hasTitlebar: true);
                    instance.Visible = false;
                }
                return instance;
            }
        }

        private static CDAddressWindow instance;
        private Tuple<UIComponent, BuildingWorldInfoPanel>[] currentBWIP;

        public override void Awake()
        {
            base.Awake();
        }
        GUIStyle m_noBreakLabel;

        protected override void DrawWindow(Vector2 size)
        {
            if (addressesLines is null)
            {
                return;
            }
            if (m_noBreakLabel is null)
            {
                m_noBreakLabel = new GUIStyle(GUI.skin.label)
                {
                    wordWrap = false
                };
            }
            using (new GUILayout.AreaScope(new Rect(new Vector2(4, 4), size)))
            {
                using (new GUILayout.VerticalScope())
                {
                    var maxLineWidth = m_noBreakLabel.CalcSize(new GUIContent(Title)).x + 20 + (16 * EffectiveFontSizeMultiplier);
                    foreach (var line in addressesLines)
                    {
                        maxLineWidth = Mathf.Max(maxLineWidth, m_noBreakLabel.CalcSize(new GUIContent(line)).x + 8);
                        GUILayout.Label(line, m_noBreakLabel);
                    }
                    windowRect.width = maxLineWidth;
                }
            }
        }

        private ushort m_lastBuildingId = 0;
        private string[] addressesLines;

        private void FixedUpdate()
        {
            if (currentBWIP is null)
            {
                var BWIPs = UIView.GetAView().GetComponentsInChildren<BuildingWorldInfoPanel>();
                if (BWIPs is null || BWIPs.Length == 0)
                {
                    return;
                }
                currentBWIP = BWIPs.Select(x => Tuple.New(x.GetComponent<UIComponent>(), x)).ToArray();
            }
            if (currentBWIP.FirstOrDefault(x => x.First.isVisible) is Tuple<UIComponent, BuildingWorldInfoPanel> window)
            {
                if (m_lastBuildingId != WorldInfoPanel.GetCurrentInstanceID().Building)
                {
                    m_lastBuildingId = WorldInfoPanel.GetCurrentInstanceID().Building;
                    ref Building building = ref BuildingManager.instance.m_buildings.m_buffer[m_lastBuildingId];
                    CDStorage.Instance.GetAddressLines(building.CalculateSidewalkPosition(), building.m_position, out addressesLines);
                    Visible = addressesLines != null;
                }

            }
            else
            {
                Visible = false;
                m_lastBuildingId = 0;
            }
            if (Visible)
            {
                Title = BuildingManager.instance.GetBuildingName(m_lastBuildingId, default);
            }
        }
        protected override void OnWindowDestroyed()
        {
            instance = null;
        }

    }

}
