using CustomData.Localization;
using CustomData.Wrappers;
using CustomData.Xml;
using Kwytto.LiteUI;
using Kwytto.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CustomData.Wrappers.BuildingGeneralDW;

namespace CustomData.UI
{
    public class CDAutoNameBuildingTab : IGUIVerticalITab
    {
        public string TabDisplayName => Str.cd_autoNamingBuildingTab_title;
        private readonly CDMainWindow root;
        public CDAutoNameBuildingTab(CDMainWindow root)
        {
            this.root = root;

        }

        private BuildingGeneralDW m_parsedData = null;
        private Vector2 m_scrollPosition;

        private IEnumerable<UseGeneratedNameCategories> passengerCategories;
        private IEnumerable<UseGeneratedNameCategories> cargoCategories;
        private IEnumerable<UseGeneratedNameCategories> ricoCategories;

        public void DrawArea(Vector2 tabAreaSize)
        {
            if (m_parsedData is null)
            {
                m_parsedData = CDStorage.Instance.GetBuildingGeneralSettings();
            }
            if (passengerCategories is null)
            {
                passengerCategories = Enum.GetValues(typeof(UseGeneratedNameCategories)).Cast<UseGeneratedNameCategories>().Where(x => x < UseGeneratedNameCategories.ShipCargo);
            }
            if (cargoCategories is null)
            {
                cargoCategories = Enum.GetValues(typeof(UseGeneratedNameCategories)).Cast<UseGeneratedNameCategories>().Where(x => x >= UseGeneratedNameCategories.ShipCargo && x < UseGeneratedNameCategories.ResidentialZone);
            }
            if (ricoCategories is null)
            {
                ricoCategories = Enum.GetValues(typeof(UseGeneratedNameCategories)).Cast<UseGeneratedNameCategories>().Where(x => x >= UseGeneratedNameCategories.ResidentialZone);
            }
            using (new GUILayout.AreaScope(new Rect(default, tabAreaSize)))
            {
                using (new GUILayout.VerticalScope())
                {
                    using (var scroll = new GUILayout.ScrollViewScope(m_scrollPosition))
                    {
                        GUILayout.Label(Str.cd_autoNamingBuildingTab_passengerStationsNamingHeader);
                        GenerateToggles(passengerCategories);
                        GUILayout.Space(8);
                        GUILayout.Label(Str.cd_autoNamingBuildingTab_cargoStationsNamingHeader);
                        GenerateToggles(cargoCategories);
                        GUILayout.Space(8);
                        GUILayout.Label(Str.cd_autoNamingBuildingTab_ricoNamingHeader);
                        GenerateToggles(ricoCategories);
                        GUILayout.Space(8);
                        m_scrollPosition = scroll.scrollPosition;
                    }
                }
            }
        }

        private void GenerateToggles(IEnumerable<UseGeneratedNameCategories> categories)
        {
            foreach (var category in categories)
            {
                if (GUIKwyttoCommons.AddToggle(category.ValueToI18n(), m_parsedData.GetIsAutogen(category), out var newVal))
                {
                    if (newVal)
                    {
                        m_parsedData.SetAutogen(category);
                    }
                    else
                    {
                        m_parsedData.UnsetAutogen(category);
                    }
                }
            }
        }


        public void Reset()
        {
        }
    }
}
