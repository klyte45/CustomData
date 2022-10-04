using CustomData.Localization;
using CustomData.Wrappers;
using CustomData.Xml;
using Kwytto.LiteUI;
using Kwytto.UI;
using System.Collections.Generic;
using UnityEngine;

namespace CustomData.UI
{
    public class CDCitizenNamingTab : IGUIVerticalITab
    {
        public string TabDisplayName => Str.cd_cimNamingTab_title;
        private readonly CDMainWindow root;
        public CDCitizenNamingTab(CDMainWindow root)
        {
            this.root = root;

        }

        private CitizenGeneralDW m_parsedData = null;

        public void DrawArea(Vector2 tabAreaSize)
        {
            if (m_parsedData is null)
            {
                m_parsedData = CDStorage.Instance.GetCitizenData();
            }
            using (new GUILayout.AreaScope(new Rect(default, tabAreaSize)))
            {
                using (new GUILayout.VerticalScope())
                {
                    root.ComboBoxWithButtons(tabAreaSize, Str.cd_cimNamingTab_femaleNamesTitle, m_parsedData.FemalesFile, CDController.LoadedGeneralNamesIdx, (x) => m_parsedData.FemalesFile = x, CDController.GeneralNamesPath, CDController.LoadGeneralNames);
                    root.ComboBoxWithButtons(tabAreaSize, Str.cd_cimNamingTab_maleNamesTitle, m_parsedData.MalesFile, CDController.LoadedGeneralNamesIdx, (x) => m_parsedData.MalesFile = x, CDController.GeneralNamesPath, CDController.LoadGeneralNames);
                    root.ComboBoxWithButtons(tabAreaSize, Str.cd_cimNamingTab_surnamesTitle, m_parsedData.SurnamesFile, CDController.LoadedGeneralNamesIdx, (x) => m_parsedData.SurnamesFile = x, CDController.GeneralNamesPath, CDController.LoadGeneralNames);
                    GUIKwyttoCommons.AddToggle(Str.cd_cimNamingTab_surnameBeforeMainname, m_parsedData.SurnameFirst, (x) => m_parsedData.SurnameFirst = x);
                }
            }
        }


        public void Reset()
        {
        }
    }
}
