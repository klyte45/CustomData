using ColossalFramework;
using CustomData.Localization;
using CustomData.Utils;
using CustomData.Wrappers;
using CustomData.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomData.Overrides
{
    public class CDFacade
    {
        public static CDFacade Instance => ModInstance.Controller.Facade;

        public event Action EventOnBuildingNameGenStrategyChanged;
        public event Action<ushort> EventOnBuildingLogoChanged;
        internal void CallBuildingNameGenStrategyChangedEvent() => BuildingManager.instance.StartCoroutine(CallBuildRenamedEvent_impl());
        internal void CallEventOnBuildingLogoChanged(ushort buildingId) => BuildingManager.instance.StartCoroutine(CallEventOnBuildingLogoChanged_impl(buildingId));
        private IEnumerator CallBuildRenamedEvent_impl()
        {
            yield return 0;
            EventOnBuildingNameGenStrategyChanged?.Invoke();
        }
        private IEnumerator CallEventOnBuildingLogoChanged_impl(ushort buildingId)
        {
            yield return 0;
            EventOnBuildingLogoChanged?.Invoke(buildingId);
        }
        public bool GetStreetAndNumber(Vector3 sidewalk, Vector3 midPosBuilding, out int number, out string streetName)
            => OwnCitySettingsDW.GetStreetAndNumber(sidewalk, midPosBuilding, out streetName, out number);
        public Color GetDistrictColor(byte districtId) => CDStorage.Instance.GetDistrictData(districtId).Color ?? CDStorage.Instance.GetDistrictData(0).Color ?? Color.black;
        public string GetStreetFull(ushort segmentId)
        {
            string result = "";
            var usedQueue = new List<ushort>();
            NetManagerOverrides.GenerateSegmentNameInternal(segmentId, ref result, ref usedQueue, false);
            return result;
        }
        public string GetStreetQualifier(ushort segmentId)
        {
            string result = "";
            var usedQueue = new List<ushort>();
            NetManagerOverrides.GenerateSegmentNameInternal(segmentId, ref result, ref usedQueue, true);
            return result.IsNullOrWhiteSpace() ? GetStreetFull(segmentId).Trim() : GetStreetFull(segmentId).Replace(result, "").Trim();
        }
        public int GetStreetDirection(ushort segmentId)
        {
            string result = "";
            var usedQueue = new List<ushort>();
            NetManagerOverrides.GenerateSegmentNameInternal(segmentId, ref result, ref usedQueue, true);
            return SegmentUtils.GetCardinalDirectionSegment(segmentId, CDStorage.Instance.GetHighwayInstance(NetManager.instance.m_segments.m_buffer[segmentId].m_nameSeed).HighwayAxis).GetCardinalAngle();
        }
    }
}
