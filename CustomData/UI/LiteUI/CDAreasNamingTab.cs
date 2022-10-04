using CustomData.Localization;
using CustomData.Wrappers;
using CustomData.Xml;
using Kwytto.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomData.UI
{
    public class CDAreasNamingTab : IGUIVerticalITab
    {
        public string TabDisplayName => Str.cd_areasNamingTab_title;
        private readonly CDMainWindow root;
        public CDAreasNamingTab(CDMainWindow root)
        {
            this.root = root;

        }

        private OwnCitySettingsDW m_parsedData = null;
        private Vector2 m_scrollPosition;

        private IEnumerable<DistrictAreaType> m_areaTypesCache;

        public void DrawArea(Vector2 tabAreaSize)
        {
            if (m_parsedData is null)
            {
                m_parsedData = CDStorage.Instance.GetOwnCitySettings();
            }
            using (new GUILayout.AreaScope(new Rect(default, tabAreaSize)))
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label(Str.cd_areasNamingTab_simplePatternNote);
                    using (var scroll = new GUILayout.ScrollViewScope(m_scrollPosition))
                    {
                        foreach (var areaType in m_areaTypesCache ?? (m_areaTypesCache = Enum.GetValues(typeof(DistrictAreaType)).Cast<DistrictAreaType>()))
                        {
                            if (areaType.IsAvailable())
                            {
                                var data = m_parsedData.GetDistrictGeneratorFile(areaType);
                                GUILayout.Label(areaType.ValueToI18n());
                                root.ComboBoxWithButtons(tabAreaSize, Str.cd_areasNamingTab_namesFile, data.mainReference, CDController.LoadedGeneralNamesIdx, (x) => OnNameChanged(x, areaType), CDController.GeneralNamesPath, CDController.LoadGeneralNames, areaType.ToString());
                                root.ComboBoxWithButtons(tabAreaSize, Str.cd_areasNamingTab_simplePatternFile, data.qualifiedReference, CDController.LoadedSimplePatternsIdx, (x) => OnPatternChanged(x, areaType), CDController.SimplePatternPath, CDController.LoadSimplePatterns, areaType.ToString());
                                GUILayout.Space(8);
                            }
                        }


                        m_scrollPosition = scroll.scrollPosition;
                    }
                }
            }
        }

        private void OnPatternChanged(string newVal, DistrictAreaType areaType)
        {
            m_parsedData.GetDistrictGeneratorFile(areaType).qualifiedReference = newVal;

            if (areaType == DistrictAreaType.Regular)
            {
                DistrictManager.instance.NamesModified();
            }
            else
            {
                DistrictManager.instance.ParkNamesModified();
            }
        }

        private void OnNameChanged(string newVal, DistrictAreaType areaType)
        {
            m_parsedData.GetDistrictGeneratorFile(areaType).mainReference = newVal;
            if (areaType == DistrictAreaType.Regular)
            {
                DistrictManager.instance.NamesModified();
            }
            else
            {
                DistrictManager.instance.ParkNamesModified();
            }
        }

        public void Reset()
        {
        }
    }
}
