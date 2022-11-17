using ColossalFramework;
using CustomData.Utils;
using CustomData.Wrappers;
using CustomData.Xml;
using ICities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomData.Overrides
{
    public class CDFacade : ILoadingExtension
    {
        public static CDFacade Instance { get; private set; } = new CDFacade();

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

        #region ITM
        public event Action<ushort> EventOnTransportLineLogoChanged;
        internal void CallEventOnTransportLineLogoChanged(ushort transportLineId) => TransportManager.instance.StartCoroutine(CallEventOnTransportLineLogoChanged_impl(transportLineId));
        private IEnumerator CallEventOnTransportLineLogoChanged_impl(ushort transportLineId)
        {
            yield return 0;
            EventOnTransportLineLogoChanged?.Invoke(transportLineId);
        }
        public Texture2D GetLineIcon(ushort transportLineId) => CDStorage.Instance.GetTransportLineInstance(transportLineId, false)?.LineIcon;
        public Texture2D SetLineIcon(ushort transportLineId, Texture2D newIcon) => CDStorage.Instance.GetTransportLineInstance(transportLineId, true).LineIcon = newIcon;
        public string GetVehicleIdentifier(ushort vehicleId) => CDStorage.Instance.GetVehicleSettings(vehicleId, true).GetVehicleIdentifier();

        public void OnCreated(ILoading loading) { }

        public void OnReleased() { Instance = new CDFacade(); }

        public void OnLevelLoaded(LoadMode mode) { }

        public void OnLevelUnloading() => Instance = new CDFacade();
        #endregion

    }
}
