using CustomData.Localization;
using Kwytto.LiteUI;
using Kwytto.UI;
using Kwytto.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomData.UI
{
    public class CDStadiumsTab : IGUIVerticalITab
    {
        public string TabDisplayName => Str.cd_stadiumsTab_title;
        private readonly CDMainWindow root;
        private uint m_lastFrameCalculation = 0;
        public CDStadiumsTab(CDMainWindow root)
        {
            this.root = root;

        }

        private Vector2 m_scrollPosition;


        private string[] m_stadiumsDisplayNames;
        private ushort[] m_stadiumsIdx;
        private int m_selectedIdx = -1;

        public void DrawArea(Vector2 tabAreaSize)
        {

            if (SimulationManager.instance.m_currentTickIndex - m_lastFrameCalculation > 40f)
            {
                ReloadStadiumsOptions();
                root.ProcessResourceTexture();
            }
            m_lastFrameCalculation = SimulationManager.instance.m_currentTickIndex;
            using (new GUILayout.AreaScope(new Rect(default, tabAreaSize)))
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label(Str.cd_stadiumsTab_selectStadiumLabel);
                    using (new GUILayout.HorizontalScope())
                    {
                        var newVal = GUIComboBox.Box(m_selectedIdx, m_stadiumsDisplayNames, "stadiums", root, tabAreaSize.x);
                        if (newVal != m_selectedIdx)
                        {
                            m_selectedIdx = newVal;
                        }
                    }
                    if (m_selectedIdx >= 0)
                    {
                        using (var scroll = new GUILayout.ScrollViewScope(m_scrollPosition))
                        {


                            m_scrollPosition = scroll.scrollPosition;
                        }
                    }
                }
            }
        }

        private const EventManager.EventType matchTypes = EventManager.EventType.Football | EventManager.EventType.VarsitySportsMatch;

        private void ReloadStadiumsOptions()
        {
            Building[] buffer = BuildingManager.instance.m_buildings.m_buffer;
            var results = new List<Tuple<ushort, string>>();
            for (ushort i = 1; i < buffer.Length; i++)
            {
                if (buffer[i].Info.m_buildingAI is MonumentAI ai && (ai.m_supportEvents & matchTypes) != 0)
                {
                    results.Add(Tuple.New(i, BuildingManager.instance.GetBuildingName(i, default)));
                }
            }
            results = results.OrderBy(x => x.Second).ToList();
            m_stadiumsDisplayNames = results.Select(x => x.Second).ToArray();
            m_stadiumsIdx = results.Select(x => x.First).ToArray();
            m_selectedIdx = -1;
        }

        public void Reset()
        {
        }
    }
}
