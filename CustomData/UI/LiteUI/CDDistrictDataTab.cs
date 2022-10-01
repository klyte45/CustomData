using CustomData.Localization;
using CustomData.Utils;
using CustomData.Wrappers;
using CustomData.Xml;
using Kwytto.LiteUI;
using Kwytto.UI;
using Kwytto.Utils;
using MonoMod.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomData.UI
{
    public class CDDistrictDataTab : IGUIVerticalITab
    {
        public string TabDisplayName => Str.cd_districtTab_title;
        private readonly CDMainWindow root;
        private uint m_lastFrameCalculation = 0;
        private readonly Dictionary<string, byte> m_cachedDistricts = new Dictionary<string, byte>();
        private string[] m_cachedDistrictOptions;
        private Texture2D m_overlayDistrictTexture;

        private int m_selectedIdxDistrict;

        public CDDistrictDataTab(CDMainWindow root)
        {
            this.root = root;

        }

        private DistrictDW m_parsedData = null;

        public void DrawArea(Vector2 tabAreaSize)
        {
            if (SimulationManager.instance.m_currentTickIndex - m_lastFrameCalculation > 40f)
            {
                ReloadDistrictOptions();
                root.ProcessResourceTexture();
            }
            m_lastFrameCalculation = SimulationManager.instance.m_currentTickIndex;
            using (new GUILayout.AreaScope(new Rect(default, tabAreaSize)))
            {
                using (new GUILayout.VerticalScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUIKwyttoCommons.AddComboBox(tabAreaSize.x - 30, Str.cd_districtTab_districtToChange, ref m_selectedIdxDistrict, m_cachedDistrictOptions, root);
                        if (GUILayout.Button(root.m_reloadButton, root.m_reloadBtnStyle))
                        {
                            ReloadDistrictOptions();
                        }
                    }

                    var selectedDistrictId = m_cachedDistricts[m_cachedDistrictOptions[m_selectedIdxDistrict]];
                    if (m_parsedData?.Id.District != selectedDistrictId)
                    {
                        m_parsedData = CDStorage.Instance.GetDistrictData(selectedDistrictId);
                        root.ProcessResourceTexture();

                        if (m_overlayDistrictTexture is null)
                        {
                            m_overlayDistrictTexture = new Texture2D(900, 900, TextureFormat.RGBA32, false);
                        }
                        var cells = DistrictManager.instance.m_districtGrid;
                        int offsets = 0;
                        int rowWidth = 900;
                        if (cells.Length == 512 * 512)
                        {
                            offsets = 194;
                            rowWidth = 512;
                        }
                        m_overlayDistrictTexture.SetPixels(m_overlayDistrictTexture.GetPixels().Select((x, i) =>
                        {
                            var coordX = (i % 900) - offsets;
                            var coordY = (i / 900) - offsets;
                            if (coordX < 0 || coordX >= rowWidth || coordY < 0 || coordY >= rowWidth)
                            {
                                return new Color(0, 0, 0, .95f);
                            }
                            else
                            {
                                var cell = cells[coordX + (coordY * rowWidth)];
                                return cell.m_district1 == selectedDistrictId ? Color.clear : new Color(0, 0, 0, .85f);
                            }
                        }).ToArray());

                        m_overlayDistrictTexture.Apply();

                    }
                    GUILayout.Space(10);
                    root.ComboBoxWithButtons(tabAreaSize, selectedDistrictId == 0 ? Str.cd_districtTab_roadNamingsFileDefault : Str.cd_districtTab_roadNamingsFile, m_parsedData.RoadNamesFile, CDController.LoadedGeneralNamesIdx, ApplyNameFile, CDController.GeneralNamesPath, CDController.LoadGeneralNames);
                    root.ComboBoxWithButtons(tabAreaSize, selectedDistrictId == 0 ? Str.cd_districtTab_roadFormattingFileDefault : Str.cd_districtTab_roadFormattingFile, m_parsedData.RoadQualifierFile, CDController.LoadedRoadPatternsIdx, ApplyQualifierFile, CDController.RoadPatternPath, CDController.LoadRoadPatternFiles);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUIKwyttoCommons.AddIntField(tabAreaSize.x - 20, selectedDistrictId == 0 ? Str.cd_districtTab_postalCodeDigitsFallback : Str.cd_districtTab_postalCodeDigits, m_parsedData.DigitsPostalCode ?? selectedDistrictId, (x) => m_parsedData.DigitsPostalCode = x, true, 0, 999, "000");
                        if (GUILayout.Button(root.m_clearButton, root.m_reloadBtnStyle))
                        {
                            m_parsedData.DigitsPostalCode = null;
                        }
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUIKwyttoCommons.AddColorPicker(selectedDistrictId == 0 ? Str.cd_districtTab_cityColor : Str.cd_districtTab_districtColor, root.m_colorPicker, m_parsedData.Color, (x) => m_parsedData.Color = x);
                        if (GUILayout.Button(root.m_clearButton, root.m_reloadBtnStyle))
                        {
                            m_parsedData.Color = null;
                        }
                    }
                    var mapRect = GUILayoutUtility.GetRect(256, 512, 256, 512);
                    GUI.DrawTexture(mapRect, root.CityTexture, ScaleMode.ScaleToFit, true, 1);
                    GUI.DrawTexture(mapRect, m_overlayDistrictTexture, ScaleMode.ScaleToFit, true, 1);
                }
            }
        }

        private void ApplyQualifierFile(string x)
        {
            m_parsedData.RoadQualifierFile = x;
            SegmentUtils.UpdateSegmentNamesView();
        }

        private void ApplyNameFile(string x)
        {
            m_parsedData.RoadNamesFile = x;
            SegmentUtils.UpdateSegmentNamesView();
        }

        private void ReloadDistrictOptions()
        {
            m_cachedDistricts.Clear();
            m_cachedDistricts.AddRange(DistrictUtils.GetValidDistricts());
            m_cachedDistrictOptions = m_cachedDistricts.OrderBy(x => x.Value == 0 ? 0 : 1).ThenBy(x => x.Key).Select(x => x.Key).ToArray();
            m_selectedIdxDistrict = 0;
        }

        public void Reset()
        {
        }
    }
}
