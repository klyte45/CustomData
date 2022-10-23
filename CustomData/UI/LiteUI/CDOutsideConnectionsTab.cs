using ColossalFramework.Math;
using CustomData.Localization;
using Kwytto.LiteUI;
using Kwytto.UI;
using Kwytto.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace CustomData.UI
{
    public class CDOutsideConnectionsTab : IGUIVerticalITab
    {
        public string TabDisplayName => Str.cd_outsideConnections_title;
        private Vector2 scrollPos;
        private readonly List<OutsideConnectionEntry> outsideConnectionEntries = new List<OutsideConnectionEntry>();

        private Texture2D roadBg;
        private Texture2D trainBg;
        private Texture2D shipBg;
        private Texture2D airplaneBg;

        private GUIStyle roadStyle;
        private GUIStyle trainStyle;
        private GUIStyle shipStyle;
        private GUIStyle airplaneStyle;

        private bool goToOnSelect;


        private static readonly Color roadColor = ColorExtensions.FromRGB("1166AA");
        private static readonly Color trainColor = ColorExtensions.FromRGB("AA4411");
        private static readonly Color shipColor = ColorExtensions.FromRGB("999922");
        private static readonly Color airplaneColor = ColorExtensions.FromRGB("992277");

        public void DrawArea(Vector2 tabAreaSize)
        {
            if (roadBg is null)
            {
                roadBg = TextureUtils.NewSingleColorForUI(roadColor);
                trainBg = TextureUtils.NewSingleColorForUI(trainColor);
                shipBg = TextureUtils.NewSingleColorForUI(shipColor);
                airplaneBg = TextureUtils.NewSingleColorForUI(airplaneColor);

                roadStyle = new GUIStyle(GUI.skin.button) { normal = { background = roadBg, textColor = roadColor.ContrastColor() } };
                trainStyle = new GUIStyle(GUI.skin.button) { normal = { background = trainBg, textColor = trainColor.ContrastColor() } };
                shipStyle = new GUIStyle(GUI.skin.button) { normal = { background = shipBg, textColor = shipColor.ContrastColor() } };
                airplaneStyle = new GUIStyle(GUI.skin.button) { normal = { background = airplaneBg, textColor = airplaneColor.ContrastColor() } };
            }
            using (new GUILayout.AreaScope(new Rect(default, tabAreaSize)))
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label(Str.cd_outsideConnections_header);
                    GUILayout.Space(4);

                    GUIKwyttoCommons.AddToggle(Str.cd_outsideConnections_goToWhenSelect, ref goToOnSelect);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(Str.cd_outsideConnections_reloadOutsideConnections))
                        {
                            ReloadOutsideConnections();
                        }
                    }
                    GUILayout.Space(8);
                    using (var scroll = new GUILayout.ScrollViewScope(scrollPos))
                    {
                        foreach (var entry in outsideConnectionEntries)
                        {
                            if (GUILayout.Button(entry.ToString(), entry.GetStyleBg(this)))
                            {
                                var iid = new InstanceID { Building = entry.buildingId };
                                WorldInfoPanel.Show<CityServiceWorldInfoPanel>(entry.building.m_position, iid);
                                if (goToOnSelect)
                                {
                                    ToolsModifierControl.cameraController.SetTarget(iid, entry.building.m_position, false);
                                }
                            }
                        }
                        scrollPos = scroll.scrollPosition;
                    }
                }
            }
        }

        private void ReloadOutsideConnections()
        {
            var buffer = BuildingManager.instance.m_buildings.m_buffer;
            outsideConnectionEntries.Clear();
            for (int i = 1; i < buffer.Length; i++)
            {
                if (buffer[i].Info?.m_buildingAI is OutsideConnectionAI)
                {
                    outsideConnectionEntries.Add(new OutsideConnectionEntry(ref buffer[i], (ushort)i));
                }
            }
            outsideConnectionEntries.Sort((a, b) => a.GetAngleAzimuth().CompareTo(b.GetAngleAzimuth()));
        }

        public void Reset()
        {
            scrollPos = default;
        }

        private class OutsideConnectionEntry
        {
            public readonly Building building;
            public readonly ushort buildingId;
            public NetNode Node { get; private set; }

            public OutsideConnectionEntry(ref Building building, ushort buildingId)
            {
                this.building = building;
                this.buildingId = buildingId;
                Node = NetManager.instance.m_nodes.m_buffer[building.m_netNode];
            }

            public GUIStyle GetStyleBg(CDOutsideConnectionsTab tab)
            {
                var ai = building.Info.m_buildingAI as OutsideConnectionAI;
                switch (ai.m_transportInfo?.m_transportType)
                {
                    case TransportInfo.TransportType.Airplane: return tab.airplaneStyle;
                    case TransportInfo.TransportType.Ship: return tab.shipStyle;
                    case TransportInfo.TransportType.Train: return tab.trainStyle;
                    default: return tab.roadStyle;
                }
            }

            public override string ToString()
            {
                var ai = building.Info.m_buildingAI as OutsideConnectionAI;
                var angle = GetAngleAzimuth();
                return $"[ID:{buildingId}] {angle:0°} ({CardinalPoint.GetCardinalPoint16(angle)})\n TL: {ai.m_transportInfo?.ToString() ?? "N/A"} @ {VectorUtils.XZ(building.m_position)} ";
            }

            internal float GetAngleAzimuth() => (building.m_position.GetAngleXZ() + 360) % 360;
        }
    }
}
