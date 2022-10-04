using ColossalFramework.UI;
using Kwytto.LiteUI;
using Kwytto.UI;
using Kwytto.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace CustomData.UI
{

    public class CDMainWindow : GUIOpacityChanging
    {
        public static CDMainWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObjectUtils.CreateElement<CDMainWindow>(UIView.GetAView().transform);
                    instance.Init(ModInstance.Instance.GeneralName, new Rect(128, 128, 680, 420), resizable: true, minSize: new Vector2(600, 420));
                    instance.Visible = false;
                }
                return instance;
            }
        }
        protected override bool showOverModals => false;
        internal Texture2D m_reloadButton;
        internal Texture2D m_excludeButton;
        internal Texture2D m_clearButton;
        internal Texture2D m_folderButton;
        internal Texture2D m_addButton;
        internal GUIStyle m_reloadBtnStyle;

        internal GUIColorPicker m_colorPicker;

        private Color bgSubgroup;
        public Texture2D BgTextureSubgroup { get; private set; }
        protected override bool requireModal => false;

        public Texture2D CityTexture { get; private set; }

        private GUIVerticalTabsContainer m_tabsContainer;
        private static CDMainWindow instance;

        public override void Awake()
        {
            base.Awake();
            bgSubgroup = ModInstance.Instance.ModColor.SetBrightness(.20f);

            BgTextureSubgroup = new Texture2D(1, 1);
            BgTextureSubgroup.SetPixel(0, 0, new Color(bgSubgroup.r, bgSubgroup.g, bgSubgroup.b, ModInstance.Instance.UIOpacity));
            BgTextureSubgroup.Apply();

            m_reloadButton = KResourceLoader.LoadTextureKwytto(CommonsSpriteNames.K45_Reload);
            m_clearButton = KResourceLoader.LoadTextureKwytto(CommonsSpriteNames.K45_Delete);
            m_folderButton = KResourceLoader.LoadTextureKwytto(CommonsSpriteNames.K45_Load);
            m_excludeButton = KResourceLoader.LoadTextureKwytto(CommonsSpriteNames.K45_X);
            m_addButton = KResourceLoader.LoadTextureKwytto(CommonsSpriteNames.K45_Plus);
            m_colorPicker = GameObjectUtils.CreateElement<GUIColorPicker>(transform).Init();
            m_colorPicker.Visible = false;

            var tabs = new IGUIVerticalITab[] {
                        new CDDistrictDataTab(this),
                        new CDNeighborDataTab(this),
                        new CDCitizenNamingTab(this)
                    };
            m_tabsContainer = new GUIVerticalTabsContainer(tabs)
            {
                m_listBgTexture = BgTextureSubgroup
            };

        }

        protected override void OnOpacityChanged(float newVal)
        {
            base.OnOpacityChanged(newVal);
            if (BgTextureSubgroup != null)
            {
                BgTextureSubgroup.SetPixel(0, 0, new Color(bgSubgroup.r, bgSubgroup.g, bgSubgroup.b, newVal));
                BgTextureSubgroup.Apply();
            }
        }

        protected override void DrawWindow(Vector2 size)
        {
            if (m_reloadBtnStyle is null)
            {
                m_reloadBtnStyle = new GUIStyle(GUI.skin.button)
                {
                    fixedHeight = 20 * ResolutionMultiplier,
                    fixedWidth = 20 * ResolutionMultiplier,
                    padding = new RectOffset(0, 0, 0, 0),
                };
            }
            m_tabsContainer.DrawListTabs(new Rect(default, size), 200 * ResolutionMultiplier);
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


        internal void ComboBoxWithButtons(Vector2 tabAreaSize, string lbl, string value, string[] array, Action<string> onSet, string targetFolder, Action onReload)
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUIKwyttoCommons.AddComboBox(tabAreaSize.x - 60 * ResolutionMultiplier, lbl, Array.IndexOf(array, value), array, out var newValue, this))
                {
                    onSet(array[newValue]);
                };
                if (GUILayout.Button(m_clearButton, m_reloadBtnStyle))
                {
                    onSet(null);
                }
                if (GUILayout.Button(m_folderButton, m_reloadBtnStyle))
                {
                    ColossalFramework.Utils.OpenInFileBrowser(targetFolder);
                }
                if (GUILayout.Button(m_reloadButton, m_reloadBtnStyle))
                {
                    onReload();
                }
            }
        }

        internal void ProcessResourceTexture()
        {
            var reference = NaturalResourceManager.instance.m_resourceTexture;
            Texture2D result = new Texture2D(reference.width, reference.height);
            result.SetPixels(reference.GetPixels().Select(x =>
                    x.b > 0.55f ? WATER
                            : x.b < 0.45f ? FOREST
                            : x.g > 0.55f ? SAND
                            : GROUND).ToArray());
            result.Apply();
            CityTexture = result;
        }

        private static readonly Color SAND = ColorExtensions.FromRGB("fff8c2").SetBrightness(.8f);
        private static readonly Color WATER = ColorExtensions.FromRGB("68a0f3").SetBrightness(.6f);
        private static readonly Color FOREST = ColorExtensions.FromRGB("38b73e").SetBrightness(.6f);
        private static readonly Color GROUND = ColorExtensions.FromRGB("cdff8d").SetBrightness(.7f);
    }

}
