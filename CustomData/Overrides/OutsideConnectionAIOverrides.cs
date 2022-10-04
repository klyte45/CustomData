using ColossalFramework.Math;
using CustomData.Xml;
using Kwytto.Utils;
using System.Reflection;
using UnityEngine;

namespace CustomData.Overrides
{
    public class OutsideConnectionAIOverrides : Redirector, IRedirectable
    {
        #region Mod

#pragma warning disable IDE0051 // Remover membros privados não utilizados
        private static bool GenerateNameOverride(OutsideConnectionAI __instance, ushort buildingID, ref string __result)
        {
            var angle = Vector2.zero.GetAngleToPoint(VectorUtils.XZ(BuildingManager.instance.m_buildings.m_buffer[buildingID].m_position));
            LogUtils.DoLog($"[buildingID {buildingID}] angle => {angle}, pos => {BuildingManager.instance.m_buildings.m_buffer[buildingID].m_position} ");

            return GetNameBasedInAngle(__instance, out __result, angle);
        }
#pragma warning restore IDE0051 // Remover membros privados não utilizados

        private static bool GetNameBasedInAngle(OutsideConnectionAI __instance, out string __result, float angle)
        {

            __result = CDStorage.Instance.GetCityAtAngle(angle)?.SafeName;
            return __result == null;
        }



        #endregion

        #region Hooking
        public static readonly MethodInfo GenerateNameMethod = typeof(OutsideConnectionAI).GetMethod("GenerateName", RedirectorUtils.allFlags);
        private static readonly OutsideConnectionAI m_defaultAI = new OutsideConnectionAI
        {
            m_useCloseNames = true,
            m_useFarNames = true,
            m_useMediumNames = true
        };

        public Redirector RedirectorInstance => this;

        public void Awake()
        {
            LogUtils.DoLog("Loading OutsideConnectionAI Overrides");
            #region OutsideConnectionAIOverrides Hooks
            MethodInfo preRename = typeof(OutsideConnectionAIOverrides).GetMethod("GenerateNameOverride", RedirectorUtils.allFlags);

            RedirectorInstance.AddRedirect(GenerateNameMethod, preRename);
            #endregion
        }
        #endregion

    }

}
