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
    public class CDNeighborDataTab : IGUIVerticalITab
    {
        public string TabDisplayName => Str.cd_neighborTab_title;
        private readonly CDMainWindow root;
        private Texture2D m_baseNeighborTexture;
        private bool m_neighborTextureDirty;
        private bool m_neighborTextureUpdateInProgress;
        private Vector2 m_scrollCities;
        private GUIStyle m_nowrapLabel;

        private const float IMG_TEXTURE_REGION_SIDE = 600;
        private const float RATIO_NEIGHBOR_CITYMAP_SIZE = 512f / 600f;
        private const float IMG_PIXEL_MULTIPLIER = .25f;

        public CDNeighborDataTab(CDMainWindow root)
        {
            this.root = root;
        }

        public void DrawArea(Vector2 tabAreaSize)
        {
            var cdStorage = CDStorage.Instance;
            if (m_baseNeighborTexture is null)
            {
                m_baseNeighborTexture = TextureUtils.New((int)(IMG_TEXTURE_REGION_SIDE * IMG_PIXEL_MULTIPLIER), (int)(IMG_TEXTURE_REGION_SIDE * IMG_PIXEL_MULTIPLIER));
                m_neighborTextureDirty = true;
            }
            if (m_neighborTextureDirty && !m_neighborTextureUpdateInProgress)
            {
                root.StartCoroutine(RecreateNeigborsTexture(cdStorage));
                m_neighborTextureUpdateInProgress = true;
            }
            if (m_nowrapLabel is null)
            {
                m_nowrapLabel = new GUIStyle(GUI.skin.label)
                {
                    wordWrap = false,
                };
            }
            using (new GUILayout.HorizontalScope())
            {
                var mapWidth = Mathf.Min((tabAreaSize.x / 2) - 3, IMG_TEXTURE_REGION_SIDE);
                var mapRect = GUILayoutUtility.GetRect(mapWidth, tabAreaSize.y);
                GUI.DrawTexture(mapRect, m_baseNeighborTexture, ScaleMode.ScaleToFit, true, 1);
                var sideSize = Mathf.Min(mapRect.width, mapRect.height);
                GUI.DrawTexture(new Rect(new Vector2((mapRect.width - (sideSize * RATIO_NEIGHBOR_CITYMAP_SIZE)) * .5f, 0), new Vector2(sideSize * RATIO_NEIGHBOR_CITYMAP_SIZE, mapRect.height)), root.CityTexture, ScaleMode.ScaleToFit, true, 1);
                GUILayout.Space(6);
                using (new GUILayout.VerticalScope())
                {
                    root.ComboBoxWithButtons(tabAreaSize / 2, Str.cd_neighborTab_neighborNamesFile, cdStorage.GetCommonCityConfig().NameGenerator, CDController.LoadedGeneralNamesIdx, OnChangeCityNameGenerator, CDController.GeneralNamesPath, CDController.LoadGeneralNames);
                    GUILayout.Space(4);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("#", m_nowrapLabel, GUILayout.Width(30 * GUIWindow.ResolutionMultiplier));
                        GUILayout.Label(Str.cd_neighborTab_azimuthDirectionShort, m_nowrapLabel, GUILayout.Width(80 * GUIWindow.ResolutionMultiplier));
                        GUILayout.Label(Str.cd_neighborTab_cityName, m_nowrapLabel);
                        GUILayout.Space(20 * GUIWindow.ResolutionMultiplier);
                        if (GUILayout.Button(root.m_addButton, root.m_inlineBtnStyle))
                        {
                            cdStorage.CreateRegionCity();
                            m_neighborTextureDirty = true;
                        }
                    }
                    using (var scroll = new GUILayout.ScrollViewScope(m_scrollCities))
                    {
                        foreach (var cityPair in cdStorage.CachedCitiesRegion.Select((x, i) => Tuple.New(x, cdStorage.CachedCitiesRegion[(i + 1) % cdStorage.CachedCitiesRegion.Length])).OrderBy(x => x.First.Id.RawData))
                        {
                            var city = cityPair.First;
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Box("A", GUILayout.Width(30 * GUIWindow.ResolutionMultiplier));
                                var rectBg = GUILayoutUtility.GetLastRect();
                                GUI.DrawTexture(rectBg, city.CachedColorTexture, ScaleMode.StretchToFill, true, 1);
                                GUI.Label(rectBg, city.Id.Index.ToString("X3"), new GUIStyle(GUI.skin.label)
                                {
                                    alignment = TextAnchor.MiddleCenter,
                                    normal = new GUIStyleState
                                    {
                                        textColor = city.CityColor.ContrastColor()
                                    }
                                });
                                var oldVal = (int)city.Azimuth;
                                if (GUIIntField.IntField($"azm.{city.Id.RawData}", oldVal, -360, 360, 35f) is int newVal && newVal != oldVal)
                                {
                                    city.Azimuth = newVal;
                                    m_neighborTextureDirty = true;
                                }
                                GUILayout.Label(CardinalPoint.GetCardinalPoint((city.Azimuth + cityPair.Second.Azimuth + (cityPair.Second.Azimuth < city.Azimuth ? 360 : 0)) / 2).GetCardinal16().ValueToI18n(), GUILayout.Width(45 * GUIWindow.ResolutionMultiplier));
                                var newValStr = GUILayout.TextField(city.SafeName);
                                if (newValStr != city.SafeName)
                                {
                                    city.Name = newValStr;
                                    m_neighborTextureDirty = true;
                                }
                                if (GUILayout.Button(root.m_reloadButton, root.m_inlineBtnStyle))
                                {
                                    city.Name = null;
                                    city.Seed = Random.Range(int.MinValue, int.MaxValue);
                                }
                                if (GUILayout.Button(root.m_excludeButton, root.m_inlineBtnStyle))
                                {
                                    cdStorage.RemoveCity(city.Id);
                                    m_neighborTextureDirty = true;
                                }
                            }
                        }
                        m_scrollCities = scroll.scrollPosition;
                    }

                }
            }

        }

        private static void OnChangeCityNameGenerator(string x)
        {
            CDStorage.Instance.GetCommonCityConfig().NameGenerator = x;
        }

        private IEnumerator RecreateNeigborsTexture(CDStorage cdStorage)
        {
            yield return 0;
            var imgCenter = new Vector2((IMG_TEXTURE_REGION_SIDE * IMG_PIXEL_MULTIPLIER * .5f) - .5f, (IMG_TEXTURE_REGION_SIDE * IMG_PIXEL_MULTIPLIER * .5f) - .5f);
            var newPixels = m_baseNeighborTexture.GetPixels();
        preFor:
            m_neighborTextureDirty = false;
            for (int i = 0; i < newPixels.Length; i++)
            {
                if (i % 10000 == 0)
                {
                    yield return 0;
                    if (m_neighborTextureDirty)
                    {
                        goto preFor;
                    }
                }
                var coord = new Vector2(i % (IMG_TEXTURE_REGION_SIDE * IMG_PIXEL_MULTIPLIER), Mathf.Floor(i / (IMG_TEXTURE_REGION_SIDE * IMG_PIXEL_MULTIPLIER)));
                var angle = imgCenter.GetAngleToPoint(coord);
                newPixels[i] = cdStorage.GetCityAtAngle(angle)?.CityColor ?? Color.clear;
            }

            m_baseNeighborTexture.SetPixels(newPixels);
            m_baseNeighborTexture.Apply();
            m_neighborTextureUpdateInProgress = false;
        }

        public void Reset()
        {
        }
    }
}
