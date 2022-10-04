using ColossalFramework;
using CustomData.Utils;
using CustomData.Xml;
using Kwytto.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CustomData.Overrides
{
    public class BuildingManagerOverrides : Redirector, IRedirectable
    {
        #region Mod


#pragma warning disable IDE0051 // Remover membros privados não utilizados
        private static bool GetNameStation(ref string __result, ushort buildingID)
        {
            var info = BuildingManager.instance.m_buildings.m_buffer[buildingID].Info;

            if (!CDStorage.Instance.GetBuildingGeneralSettings().IsRenameEnabled(info.m_buildingAI))
            {
                return true;
            }
            if (info.m_placementMode == BuildingInfo.PlacementMode.Roadside)
            {
                Vector3 sidewalk = BuildingManager.instance.m_buildings.m_buffer[buildingID].CalculateSidewalkPosition();
                SegmentUtils.GetNearestSegment(sidewalk, out _, out _, out ushort targetSegmentId);
                List<ushort> crossingSegmentList = SegmentUtils.GetCrossingPath(targetSegmentId);
                crossingSegmentList.Add(targetSegmentId);
                foreach (ushort segId in crossingSegmentList)
                {
                    if ((NetManager.instance.m_segments.m_buffer[segId].m_flags & NetSegment.Flags.CustomName) != NetSegment.Flags.None)
                    {
                        InstanceID id = default;
                        id.NetSegment = segId;
                        __result = Singleton<InstanceManager>.instance.GetName(id);
                        if (__result != string.Empty)
                        {
                            return false;
                        }
                    }

                    if (NetManagerOverrides.GetStreetNameForStation(segId, ref __result))
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                foreach (ushort segId in crossingSegmentList)
                {
                    __result = NetManager.instance.GetSegmentName(segId) ?? string.Empty;

                    if (__result == string.Empty)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                var parkId = DistrictManager.instance.GetPark(BuildingManager.instance.m_buildings.m_buffer[buildingID].m_position);
                if (parkId != 0)
                {
                    __result = DistrictManager.instance.GetParkName(parkId);
                    if (__result != string.Empty)
                    {
                        return false;
                    }
                }
                var districtId = DistrictManager.instance.GetDistrict(BuildingManager.instance.m_buildings.m_buffer[buildingID].m_position);
                if (districtId != 0)
                {
                    __result = DistrictManager.instance.GetDistrictName(districtId);
                    if (__result != string.Empty)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        private static bool GetNameRico(ref string __result, ushort buildingID)
        {
            var info = BuildingManager.instance.m_buildings.m_buffer[buildingID].Info;
            if (CDStorage.Instance.GetBuildingGeneralSettings().IsRenameEnabled(info.m_buildingAI))
            {
                return true;
            }

            Vector3 sidewalk = BuildingManager.instance.m_buildings.m_buffer[buildingID].CalculateSidewalkPosition();
            CDStorage.Instance.GetOwnCitySettings().GetAddressLines(sidewalk, BuildingManager.instance.m_buildings.m_buffer[buildingID].m_position, out string[] addressLines);
            if (addressLines.Length == 0)
            {
                return true;
            }
            __result = addressLines[0];
            return false;
        }

#pragma warning restore IDE0051 // Remover membros privados não utilizados
        #endregion

        #region Hooking
        private static readonly MethodInfo m_getNameMethod = typeof(BuildingAI).GetMethod("GenerateName", RedirectorUtils.allFlags);
        private static readonly MethodInfo m_getNameMethodRes = typeof(ResidentialBuildingAI).GetMethod("GenerateName", RedirectorUtils.allFlags);
        private static readonly MethodInfo m_getNameMethodInd = typeof(IndustrialBuildingAI).GetMethod("GenerateName", RedirectorUtils.allFlags);
        private static readonly MethodInfo m_getNameMethodCom = typeof(CommercialBuildingAI).GetMethod("GenerateName", RedirectorUtils.allFlags);
        private static readonly MethodInfo m_getNameMethodOff = typeof(OfficeBuildingAI).GetMethod("GenerateName", RedirectorUtils.allFlags);

        public Redirector RedirectorInstance => this;
        #endregion


        #region Hooking

        public void Awake()
        {
            LogUtils.DoLog("Loading Building Manager Overrides");
            #region Building Hooks
            MethodInfo preRename = typeof(BuildingManagerOverrides).GetMethod("GetNameStation", RedirectorUtils.allFlags);
            MethodInfo preRenameRico = typeof(BuildingManagerOverrides).GetMethod("GetNameRico", RedirectorUtils.allFlags);
            LogUtils.DoLog($"Overriding GetName ({m_getNameMethod} => {preRename})");
            RedirectorInstance.AddRedirect(m_getNameMethod, preRename);
            RedirectorInstance.AddRedirect(m_getNameMethodRes, preRenameRico);
            RedirectorInstance.AddRedirect(m_getNameMethodInd, preRenameRico);
            RedirectorInstance.AddRedirect(m_getNameMethodCom, preRenameRico);
            RedirectorInstance.AddRedirect(m_getNameMethodOff, preRenameRico);
            #endregion

        }
        #endregion

    }
}
