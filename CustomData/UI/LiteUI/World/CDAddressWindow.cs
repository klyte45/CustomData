using ColossalFramework.UI;
using CustomData.Localization;
using CustomData.Xml;
using Kwytto.LiteUI;
using Kwytto.UI;
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
        protected override bool ShowMinimizeButton => true;
        private const float minHeight = 325;
        private GUIColorPicker picker;
        private Texture2D m_clearButton;
        private Texture2D m_helpButton;
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
            m_clearButton = KResourceLoader.LoadTextureKwytto(CommonsSpriteNames.K45_Delete);
            m_helpButton = KResourceLoader.LoadTextureKwytto(CommonsSpriteNames.K45_QuestionMark);
            picker = GameObjectUtils.CreateElement<GUIColorPicker>(transform).Init();
            picker.Visible = false;
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

            if (m_inlineBtnStyle is null)
            {
                m_inlineBtnStyle = new GUIStyle(GUI.skin.button)
                {
                    fixedHeight = 20 * ResolutionMultiplier,
                    fixedWidth = 20 * ResolutionMultiplier,
                    padding = new RectOffset(0, 0, 0, 0),
                };
            }
            var offset = new Vector2(4, 4);
            var effArea = size - offset - offset;
            using (new GUILayout.AreaScope(new Rect(offset, effArea)))
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

                    var buildingSettings = CDStorage.Instance.GetBuildingSettings(m_lastBuildingId, false);

                    GUILayout.Space(5);
                    maxLineWidth = Mathf.Max(maxLineWidth, m_noBreakLabel.CalcSize(new GUIContent(Str.cd_addressWindow_vehicleIdentifierPattern)).x + 8);
                    GUILayout.Label(Str.cd_addressWindow_vehicleIdentifierPattern, m_noBreakLabel);
                    using (new GUILayout.HorizontalScope())
                    {
                        var newText = GUILayout.TextField(buildingSettings?.OwnVehiclesIdFormatter ?? "", GUILayout.Height(20));
                        if (buildingSettings?.PreferredSkin != newText)
                        {
                            if (buildingSettings is null)
                            {
                                buildingSettings = CDStorage.Instance.GetBuildingSettings(m_lastBuildingId, true);
                            }
                            buildingSettings.OwnVehiclesIdFormatter = newText;
                        }
                        if (GUILayout.Button(m_helpButton, m_inlineBtnStyle))
                        {
                            KwyttoDialog.ShowModal(new KwyttoDialog.BindProperties
                            {
                                buttons = KwyttoDialog.basicOkButtonBar,
                                title = Str.cd_addressWindow_vehicleIdentifierPattern,
                                message = Str.cd_addressWindow_vehicleIdentifierPattern_helpHeader,
                                scrollText = Str.cd_addressWindow_vehicleIdentifierPattern_helpContent,
                            });
                        }
                    }
                    {
                        GUILayout.Space(5);
                        maxLineWidth = Mathf.Max(maxLineWidth, m_noBreakLabel.CalcSize(new GUIContent(Str.cd_addressWindow_preferredSkin)).x + 8);
                        GUILayout.Label(Str.cd_addressWindow_preferredSkin, m_noBreakLabel);
                        var newText = GUILayout.TextField(buildingSettings?.PreferredSkin ?? "", GUILayout.Height(20));
                        if (buildingSettings?.PreferredSkin != newText)
                        {
                            if (buildingSettings is null)
                            {
                                buildingSettings = CDStorage.Instance.GetBuildingSettings(m_lastBuildingId, true);
                            }
                            buildingSettings.PreferredSkin = newText;
                        };
                    }
                    ColorVehiclesEditor(ref maxLineWidth, ref buildingSettings);

                    GUILayout.Space(5);

                    if (buildingSettings?.Logo is Texture2D tex)
                    {
                        AddLogoPickerButton();
                        if (GUILayout.Button(Str.cd_addressWindow_removeLogo, RedButton))
                        {
                            if (buildingSettings is null)
                            {
                                buildingSettings = CDStorage.Instance.GetBuildingSettings(m_lastBuildingId, true);
                            }
                            buildingSettings.Logo = null;
                        }

                        maxLineWidth = Mathf.Max(maxLineWidth, 256 + 8);
                        GUILayout.Space(256);
                        var lastRect = GUILayoutUtility.GetLastRect();
                        windowRect.height = 256 + minHeight + 30;
                        GUI.DrawTexture(new Rect(0, lastRect.y + 2, effArea.x, 256), tex, ScaleMode.ScaleToFit, true);
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

        private void ColorVehiclesEditor(ref float maxLineWidth, ref Wrappers.BuildingDW buildingSettings)
        {
            GUILayout.Space(5);
            maxLineWidth = Mathf.Max(maxLineWidth, 310, m_noBreakLabel.CalcSize(new GUIContent(Str.cd_addressWindow_ownVehicleColors)).x + 8);
            GUILayout.Label(Str.cd_addressWindow_ownVehicleColors, m_noBreakLabel);
            var oldVal = buildingSettings?.VehiclesColor;
            Color? newVal;
            using (new GUILayout.HorizontalScope())
            {
                newVal = picker.PresentColor("CD_BUILDINGCLR", oldVal, true);
                if (GUILayout.Button(m_clearButton, m_inlineBtnStyle))
                {
                    newVal = null;
                }
                GUILayout.Space(3);
            }
            var changed = newVal != oldVal;
            if (changed)
            {
                if (buildingSettings is null)
                {
                    buildingSettings = CDStorage.Instance.GetBuildingSettings(m_lastBuildingId, true);
                }
                buildingSettings.VehiclesColor = newVal;
            }
            maxLineWidth = Mathf.Max(maxLineWidth, m_noBreakLabel.CalcSize(new GUIContent(Str.cd_addressWindow_overrideLineColor)).x + 16);
            GUIKwyttoCommons.AddToggle(Str.cd_addressWindow_overrideLineColor, buildingSettings?.OverrideLineColor ?? false, (x) => CDStorage.Instance.GetBuildingSettings(m_lastBuildingId, true).OverrideLineColor = x, true);
        }

        private void AddLogoPickerButton()
        {
            if (GUILayout.Button(Str.cd_addressWindow_addLogoImage))
            {
                CDFilePicker.PickAFile(string.Format(Str.cd_addressWindow_pickALogoFor, Title), (x) =>
                {
                    if (x != null)
                    {
                        var result = TextureAtlasUtils.LoadTextureFromFile(x, linear: false);
                        if (result.width != 256 || result.height != 256)
                        {
                            ModInstance.Controller.StartCoroutine(ShowErrorModal());
                            Destroy(result);
                        }
                        else
                        {
                            CDStorage.Instance.GetBuildingSettings(m_lastBuildingId, true).Logo = result;
                        }
                    }
                });
            }
        }

        private ushort m_lastBuildingId = 0;
        private string[] addressesLines;
        private GUIStyle m_inlineBtnStyle;

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
