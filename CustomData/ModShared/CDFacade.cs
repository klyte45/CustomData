﻿using ColossalFramework;
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
        internal void CallBuildingNameGenStrategyChangedEvent() => BuildingManager.instance.StartCoroutine(CallBuildRenamedEvent_impl());
        private IEnumerator CallBuildRenamedEvent_impl()
        {
            yield return 0;
            EventOnBuildingNameGenStrategyChanged?.Invoke();
        }


        public event Action<ushort> EventOnBuildingLogoChanged;
        internal void CallEventOnBuildingLogoChanged(ushort buildingId) => BuildingManager.instance.StartCoroutine(CallEventOnBuildingLogoChanged_impl(buildingId));
        private IEnumerator CallEventOnBuildingLogoChanged_impl(ushort buildingId)
        {
            yield return 0;
            EventOnBuildingLogoChanged?.Invoke(buildingId);
        }

        public event Action<ushort> EventOnBuildingVehicleSkinChanged;
        internal void CallEventOnBuildingVehicleSkinChanged(ushort buildingId) => BuildingManager.instance.StartCoroutine(CallEventOnBuildingVehicleSkinChanged_impl(buildingId));
        private IEnumerator CallEventOnBuildingVehicleSkinChanged_impl(ushort buildingId)
        {
            yield return 0;
            EventOnBuildingVehicleSkinChanged?.Invoke(buildingId);
        }


        public event Action<ushort> EventOnBuildingVehicleIdPatternChanged;
        internal void CallEventOnBuildingVehicleIdPatternChanged(ushort buildingId) => BuildingManager.instance.StartCoroutine(CallEventOnBuildingVehicleIdPatternChanged_impl(buildingId));
        private IEnumerator CallEventOnBuildingVehicleIdPatternChanged_impl(ushort buildingId)
        {
            yield return 0;
            EventOnBuildingVehicleIdPatternChanged?.Invoke(buildingId);
        }

        public event Action EventPostalCodeParamChanged;
        internal void CallEventPostalCodeParamChanged() => BuildingManager.instance.StartCoroutine(CallEventPostalCodeParamChanged_impl());
        private IEnumerator CallEventPostalCodeParamChanged_impl()
        {
            yield return 0;
            EventPostalCodeParamChanged?.Invoke();
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

        public string GetPreferredVehiclesSkinForBuilding(ushort buildingId) => buildingId == 0 ? null : CDStorage.Instance.GetBuildingSettings(buildingId, false)?.PreferredSkin;
    }
}
