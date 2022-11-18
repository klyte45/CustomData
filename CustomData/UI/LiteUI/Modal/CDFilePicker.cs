using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using HarmonyLib;
using Kwytto.LiteUI;
using Kwytto.Utils;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CustomData.UI
{
    public class CDFilePicker : GUIRootWindowBase
    {
        protected override void OnWindowDestroyed()
        {
            Destroy(tempTexture);
        }

        internal static void PickAFile(string title, Action<string> onReturn)
        {
            var parent = UIView.GetAView();
            var container = new GameObject();
            container.transform.SetParent(parent.transform);
            var window = container.AddComponent<CDFilePicker>();
            window.Init(title ?? ModInstance.Instance.GeneralName, new Rect(new Vector2((Screen.width / 2) - 400, (Screen.height / 2) - 250) / UIScaler.UIScale, new Vector2(800, 500) / UIScaler.UIScale), true, true, new Vector2(500, 500));
            window.onReturn = onReturn;
            window.RestartFilterCoroutine();
        }
        protected override void OnCloseButtonPress()
        {
            base.OnCloseButtonPress();
            Destroy(gameObject);
        }

        protected override bool showOverModals => true;
        protected override bool requireModal => true;
        protected override bool ShowCloseButton => false;

        private Action<string> onReturn;
        private Vector2 m_leftPanelScroll;
        private Vector2 m_rightPanelScroll;
        private static string m_currentDirectory = ModInstance.Instance.ModRootFolder;
        private string SearchText = "";
        private readonly string extension = "*.png";
        private Texture2D tempTexture;
        private string loadedTexturePath;
        public string SelectedValue => m_searchResult.Value?.ElementAtOrDefault(HoverIdx);
        protected override void DrawWindow(Vector2 size)
        {
            var topHeight = DrawTop(size);
            using (new GUILayout.HorizontalScope())
            {
                using (var scroll = new GUILayout.ScrollViewScope(m_leftPanelScroll, false, true, GUILayout.Width(size.x / 2), GUILayout.Height(size.y - topHeight - 30)))
                {
                    DrawLeftPanel(size);
                    m_leftPanelScroll = scroll.scrollPosition;
                };
                using (new GUILayout.VerticalScope(GUILayout.Width(size.x / 2 - 20), GUILayout.Height(size.y - topHeight - 30)))
                {
                    GUILayout.Label(m_currentDirectory, new GUIStyle(GUI.skin.label) { normal = new GUIStyleState() { textColor = Color.yellow } });
                    GUILayout.Space(8);
                    using (var scroll = new GUILayout.ScrollViewScope(m_rightPanelScroll))
                    {
                        DrawRightPanel(new Vector2(size.x / 2 - 20, size.y - topHeight - 50));
                        m_rightPanelScroll = scroll.scrollPosition;
                    }
                };
            };
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(Locale.Get("CANCEL"), GUILayout.Width(size.x / 2)))
                {
                    OnClose(null);
                }
                if (SelectedValue != null)
                {
                    string selection = Path.Combine(m_currentDirectory, SelectedValue);
                    if (File.Exists(selection) && GUILayout.Button("OK"))
                    {
                        OnClose(selection);
                    }
                }
            };
        }
        private void OnClose(string v)
        {
            onReturn(v);
            Visible = false;
            Destroy(gameObject);
        }

        internal readonly Wrapper<string[]> m_searchResult = new Wrapper<string[]>();
        private Coroutine m_searchCoroutine;
        internal void RestartFilterCoroutine()
        {
            if (m_searchCoroutine != null)
            {
                ModInstance.Controller.StopCoroutine(m_searchCoroutine);
            }
            m_searchCoroutine = ModInstance.Controller.StartCoroutine(OnFilterParam(null));
        }
        public int HoverIdx = -1;
        public float DrawTop(Vector2 areaRect)
        {
            bool dirtyInput;
            using (new GUILayout.HorizontalScope(GUILayout.Width(areaRect.x)))
            {
                var newInput = GUILayout.TextField(SearchText, GUILayout.Height(30));
                dirtyInput = newInput != SearchText;
                if (dirtyInput)
                {
                    SearchText = newInput;
                }
            };

            if (dirtyInput)
            {
                RestartFilterCoroutine();
            }
            return 50;
        }
        public void DrawLeftPanel(Vector2 areaRect)
        {
            var selectItem = GUILayout.SelectionGrid(HoverIdx, m_searchResult.Value, 1, new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft
            }, GUILayout.Width((areaRect.x / 2) - 25));
            if (selectItem >= 0)
            {
                OnSelectItem(selectItem);
            }
        }
        public void DrawRightPanel(Vector2 areaRect)
        {
            using (new GUILayout.AreaScope(new Rect(default, areaRect)))
            {
                using (var scroll = new GUILayout.ScrollViewScope(scrollImageRightPanel))
                {
                    if (HoverIdx > 0 && Path.Combine(m_currentDirectory, SelectedValue) is string filePath && loadedTexturePath != filePath && File.Exists(filePath))
                    {
                        if (!(tempTexture is null))
                        {
                            Destroy(tempTexture);
                        }
                        loadedTexturePath = filePath;
                        tempTexture = TextureAtlasUtils.LoadTextureFromFile(filePath);
                    }
                    if (tempTexture != null)
                    {
                        GUI.DrawTexture(new Rect(0, 0, tempTexture.width, tempTexture.height), tempTexture, ScaleMode.ScaleToFit, true);
                    }
                    scrollImageRightPanel = scroll.scrollPosition;
                }
            }
        }
        static Vector2 scrollImageRightPanel;
        public void OnSelectItem(int selectLayout)
        {
            if (selectLayout == HoverIdx)
            {
                if (selectLayout == 0 && !IsRoot())
                {
                    m_searchResult.Value = new string[0];
                    m_currentDirectory = Directory.GetParent(m_currentDirectory)?.FullName ?? "";
                    RestartFilterCoroutine();
                }
                else
                {
                    var targetPath = Path.Combine(m_currentDirectory, SelectedValue);
                    if (Directory.Exists(targetPath))
                    {
                        m_currentDirectory = Path.Combine(m_currentDirectory, SelectedValue);
                        RestartFilterCoroutine();
                    }
                }
            }
            else
            {
                HoverIdx = selectLayout;
            }

        }

        private IEnumerator OnFilterParam(string autoselect)
        {
            HoverIdx = -1;
            Destroy(tempTexture);
            SearchText = "";
            m_searchResult.Value = new string[0];
            yield return 0;
            yield return m_searchResult.Value = IsRoot() ? OnFilterParam() : new[] { "<color=#FFFF00><<</color>" }.AddRangeToArray(OnFilterParam()?.Select(x => x.IsNullOrWhiteSpace() ? GUIKwyttoCommons.v_empty : x).ToArray() ?? new string[0]);
            if (autoselect != null)
            {
                var autoSelectVal = Array.IndexOf(m_searchResult.Value, autoselect);
                if (autoSelectVal > 0)
                {
                    OnHoverVar(autoSelectVal);
                }
            }
        }

        private bool IsRoot()
        {
            return m_currentDirectory.Length == 0;
        }

        public string[] OnFilterParam()
        {
            if (IsRoot())
            {
                return Directory.GetLogicalDrives();
            }
            else
            {
                try
                {
                    return Directory.GetDirectories(m_currentDirectory).Select(x => x.Replace(m_currentDirectory + Path.DirectorySeparatorChar, "")).Concat(Directory.GetFiles(m_currentDirectory, extension).Select(x => x.Replace(m_currentDirectory + Path.DirectorySeparatorChar, ""))).ToArray();
                }
                catch
                {
                    return new string[0];
                }
            }
        }

        public void OnHoverVar(int autoSelectVal)
        {
        }
    }
}
