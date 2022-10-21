using CustomData.Wrappers;
using CustomData.Xml;
using Kwytto.Utils;
using System.Reflection;
using UnityEngine;

namespace CustomData.Overrides
{
    public class VehicleAIOVerrides : Redirector, IRedirectable
    {
        #region Mod
        public static bool PreGetColor(ushort vehicleID, ref Vehicle data, InfoManager.InfoMode infoMode, ref Color __result)
        {
            if (infoMode == InfoManager.InfoMode.None && data.m_sourceBuilding > 0 && CDStorage.Instance.GetBuildingSettings(data.m_sourceBuilding, true) is BuildingDW dw && dw.VehiclesColor is Color clr)
            {
                __result = clr;
                return false;
            }
            return true;
        }
        public static bool PreGetColorLinedVehicle(ushort vehicleID, ref Vehicle data, InfoManager.InfoMode infoMode, ref Color __result)
        {
            if (infoMode == InfoManager.InfoMode.None && data.m_sourceBuilding > 0 && CDStorage.Instance.GetBuildingSettings(data.m_sourceBuilding, true) is BuildingDW dw && dw.OverrideLineColor && dw.VehiclesColor is Color clr)
            {
                __result = clr;
                return false;
            }
            return true;
        }
        #endregion
        #region Hooking
        public void Awake()
        {
            LogUtils.DoLog("Loading VehicleAI Overrides");
            #region VehicleAI Hooks
            MethodInfo preRename = typeof(VehicleAIOVerrides).GetMethod("PreGetColor", RedirectorUtils.allFlags);
            MethodInfo GetNameMethod = typeof(VehicleAI).GetMethod("GetColor", RedirectorUtils.allFlags, null, new[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InfoManager.InfoMode) }, null);
            LogUtils.DoLog($"Overriding GetName ({GetNameMethod} => {preRename})");
            AddRedirect(GetNameMethod, preRename);
            MethodInfo preGetColorLine = typeof(VehicleAIOVerrides).GetMethod("PreGetColorLinedVehicle", RedirectorUtils.allFlags);
            foreach (var clazz in new[]
            {
                typeof(BusAI),
                typeof(CableCarAI),
                typeof(PassengerBlimpAI),
                typeof(PassengerFerryAI),
                typeof(PassengerHelicopterAI),
                typeof(PassengerPlaneAI),
                typeof(PassengerShipAI),
                typeof(PassengerTrainAI),
                typeof(TaxiAI),
                typeof(TramAI),
                typeof(TrolleybusAI),
            })
            {
                AddRedirect(clazz.GetMethod("GetColor", RedirectorUtils.allFlags, null, new[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InfoManager.InfoMode) }, null), preGetColorLine);
            }
            #endregion
        }
        #endregion
    }
}
