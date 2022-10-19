using ColossalFramework.UI;
using CustomData.Localization;
using CustomData.Xml;
using Kwytto.LiteUI;
using Kwytto.Utils;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CustomData.UI
{

    public class CDAddressWindow : GUIOpacityChanging
    {
        protected override bool showOverModals => false;
        protected override bool requireModal => false;
        protected override bool ShowCloseButton => false;
        private const float minHeight = 125;
        public static CDAddressWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObjectUtils.CreateElement<CDAddressWindow>(UIView.GetAView().transform);
                    instance.Init(ModInstance.Instance.GeneralName, new Rect(256, 256, 100, minHeight), resizable: false, minSize: new Vector2(100, minHeight), hasTitlebar: true);
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

        private GUIStyle m_redButton;
        public GUIStyle RedButton
        {
            get
            {
                if (m_redButton is null)
                {
                    m_redButton = new GUIStyle(Skin.button)
                    {
                        normal = new GUIStyleState()
                        {
                            background = GUIKwyttoCommons.darkRedTexture,
                            textColor = Color.white
                        },
                        hover = new GUIStyleState()
                        {
                            background = GUIKwyttoCommons.redTexture,
                            textColor = Color.white
                        },
                    };
                }
                return m_redButton;
            }
        }
        protected override void DrawWindow(Vector2 size)
        {
            if (m_noBreakLabel is null)
            {
                m_noBreakLabel = new GUIStyle(GUI.skin.label)
                {
                    wordWrap = false,
                    alignment = TextAnchor.MiddleLeft,
                };
            }
            using (new GUILayout.AreaScope(new Rect(new Vector2(4, 4), size)))
            {
                using (new GUILayout.VerticalScope())
                {
                    var maxLineWidth = m_noBreakLabel.CalcSize(new GUIContent(Title)).x + 20 + (16 * EffectiveFontSizeMultiplier);
                    if (addressesLines != null)
                    {
                        foreach (var line in addressesLines)
                        {
                            maxLineWidth = Mathf.Max(maxLineWidth, m_noBreakLabel.CalcSize(new GUIContent(line)).x + 8);
                            GUILayout.Label(line, m_noBreakLabel);
                        }
                    }
                    else
                    {
                        var dimensions = m_noBreakLabel.CalcSize(new GUIContent(Str.cd_addressWindow_noAddressAvailable));
                        maxLineWidth = Mathf.Max(maxLineWidth, dimensions.x + 8);
                        GUILayout.Label(Str.cd_addressWindow_noAddressAvailable, m_noBreakLabel, GUILayout.Height(3 * dimensions.y));
                    }
                    if (CDStorage.Instance.GetBuildingSettings(m_lastBuildingId).Logo is Texture2D tex)
                    {
                        AddLogoPickerButton();
                        if (GUILayout.Button(Str.cd_addressWindow_removeLogo, RedButton))
                        {
                            CDStorage.Instance.GetBuildingSettings(m_lastBuildingId).Logo = null;
                        }

                        maxLineWidth = Mathf.Max(maxLineWidth, 256 + 8);
                        GUILayout.Space(256);
                        var lastRect = GUILayoutUtility.GetLastRect();
                        windowRect.height = 256 + minHeight + 30;
                        GUI.DrawTexture(new Rect(0, lastRect.y + 2, 256, 256), tex, ScaleMode.ScaleToFit, true);
                    }
                    else
                    {
                        AddLogoPickerButton();
                        windowRect.height = minHeight;
                    }
                    windowRect.width = maxLineWidth;
                }
            }
        }

        private void AddLogoPickerButton()
        {
            if (GUILayout.Button(Str.cd_addressWindow_addLogoImage))
            {
                CDFilePicker.PickAFile(string.Format(Str.cd_addressWindow_pickALogoFor, Title), (x) =>
                {
                    if (x != null)
                    {
                        var result = TextureAtlasUtils.LoadTextureFromFile(x);
                        if (result.width != 256 || result.height != 256)
                        {
                            ModInstance.Controller.StartCoroutine(ShowErrorModal());
                            Destroy(result);
                        }
                        else
                        {
                            CDStorage.Instance.GetBuildingSettings(m_lastBuildingId).Logo = result;
                        }
                    }
                });
            }
        }

        private ushort m_lastBuildingId = 0;
        private string[] addressesLines;

        public IEnumerator ShowErrorModal()
        {
            yield return 0;
            yield return 0;
            KwyttoDialog.ShowModal(new KwyttoDialog.BindProperties
            {
                title = Str.cd_addressWindow_invalidTexture,
                message = Str.cd_addressWindow_invalidTextureContent,
                messageAlign = TextAnchor.MiddleCenter,
                messageTextSizeMultiplier = 1.5f,
                buttons = KwyttoDialog.basicOkButtonBar
            });
        }

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
                    CDStorage.Instance.GetOwnCitySettings().GetAddressLines(building.CalculateSidewalkPosition(), building.m_position, out addressesLines);
                    Visible = true;
                    Title = BuildingManager.instance.GetBuildingName(m_lastBuildingId, default);
                }
            }
            else
            {
                Visible = false;
                m_lastBuildingId = 0;
            }
        }
        protected override void OnWindowDestroyed()
        {
            instance = null;
        }

    }

}
