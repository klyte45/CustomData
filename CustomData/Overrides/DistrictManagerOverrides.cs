using ColossalFramework.Globalization;
using ColossalFramework.Math;
using CustomData.Enums;
using CustomData.Xml;
using Kwytto.Utils;
using System.Reflection;

namespace CustomData.Overrides
{
    public class DistrictManagerOverrides : Redirector, IRedirectable
    {
        public Redirector RedirectorInstance => this;
        #region Mod
#pragma warning disable IDE0051 // Remover membros privados não utilizados
        private static bool GenerateName(int district, DistrictManager __instance, ref string __result)
        {
            if (!LoadingManager.instance.m_loadingComplete)
            {
                return true;
            }

            Randomizer randomizer = new Randomizer(__instance.m_districts.m_buffer[district].m_randomSeed);
            var settings = CDStorage.Instance.GetOwnCitySettings().GetDistrictGeneratorFile(DistrictAreaType.Regular);
            DoNameGen(out __result, randomizer, settings);
            return false;
        }
        private static bool GenerateParkName(int park, DistrictManager __instance, ref string __result)
        {
            if (!LoadingManager.instance.m_loadingComplete)
            {
                return true;
            }
            ref DistrictPark parkRef = ref __instance.m_parks.m_buffer[park];
            Randomizer randomizer = new Randomizer(parkRef.m_randomSeed);
            DistrictAreaType type;
            switch (parkRef.m_parkType)
            {
                case DistrictPark.ParkType.AmusementPark:
                    type = DistrictAreaType.Parks_Amusement;
                    break;
                case DistrictPark.ParkType.Zoo:
                    type = DistrictAreaType.Parks_Zoo;
                    break;
                case DistrictPark.ParkType.NatureReserve:
                    type = DistrictAreaType.Parks_Natural;
                    break;
                case DistrictPark.ParkType.Farming:
                    type = DistrictAreaType.Industry_Farming;
                    break;
                case DistrictPark.ParkType.Forestry:
                    type = DistrictAreaType.Industry_Forest;
                    break;
                case DistrictPark.ParkType.Ore:
                    type = DistrictAreaType.Industry_Mining;
                    break;
                case DistrictPark.ParkType.Oil:
                    type = DistrictAreaType.Industry_Oil;
                    break;
                case DistrictPark.ParkType.TradeSchool:
                    type = DistrictAreaType.Campus_Trade;
                    break;
                case DistrictPark.ParkType.LiberalArts:
                    type = DistrictAreaType.Campus_Liberal;
                    break;
                case DistrictPark.ParkType.University:
                    type = DistrictAreaType.Campus_Regular;
                    break;
                case DistrictPark.ParkType.Airport:
                    type = DistrictAreaType.Airports;
                    break;
                case DistrictPark.ParkType.PedestrianZone:
                    type = DistrictAreaType.Pedestrian;
                    break;
                case DistrictPark.ParkType.None:
                case DistrictPark.ParkType.Generic:
                case DistrictPark.ParkType.Industry:
                case DistrictPark.ParkType.GenericCampus:
                default:
                    return true;
            }
            var settings = CDStorage.Instance.GetOwnCitySettings().GetDistrictGeneratorFile(type);
            DoNameGen(out __result, randomizer, settings);
            return false;
        }

        private static void DoNameGen(out string __result, Randomizer randomizer, InstanceDataExtensionXml.ReferenceData settings)
        {
            string format, arg;
            string filenamePrefix = settings.qualifiedReference ?? "";
            string filenameName = settings.mainReference ?? "";

            if (CDController.LoadedSimplePatterns.TryGetValue(filenamePrefix, out var formatArr))
            {
                int arrLen = formatArr.Length;
                format = formatArr[randomizer.Int32((uint)arrLen)];
            }
            else
            {
                format = Locale.Get("DISTRICT_PATTERN", randomizer.Int32(Locale.Count("DISTRICT_PATTERN")));
            }

            if (CDController.LoadedGeneralNames.TryGetValue(filenameName, out var nameArr))
            {
                int arrLen = nameArr.Length;
                arg = nameArr[randomizer.Int32((uint)arrLen)];
            }
            else
            {
                arg = Locale.Get("DISTRICT_NAME", randomizer.Int32(Locale.Count("DISTRICT_NAME")));
            }

            __result = StringUtils.SafeFormat(format, arg);
        }
#pragma warning restore IDE0051 // Remover membros privados não utilizados

        #endregion

        #region Hooking
        public void Awake()
        {
            LogUtils.DoLog("Loading District Overrides");
            #region District Hooks
            MethodInfo preRename = typeof(DistrictManagerOverrides).GetMethod("GenerateName", RedirectorUtils.allFlags);
            MethodInfo GetNameMethod = typeof(DistrictManager).GetMethod("GenerateName", RedirectorUtils.allFlags);
            LogUtils.DoLog($"Overriding GetName ({GetNameMethod} => {preRename})");
            RedirectorInstance.AddRedirect(GetNameMethod, preRename);
            MethodInfo preRenamePk = typeof(DistrictManagerOverrides).GetMethod("GenerateParkName", RedirectorUtils.allFlags);
            MethodInfo GetNameMethodPk = typeof(DistrictManager).GetMethod("GenerateParkName", RedirectorUtils.allFlags);
            LogUtils.DoLog($"Overriding GetParkName ({GetNameMethod} => {preRename})");
            RedirectorInstance.AddRedirect(GetNameMethodPk, preRenamePk);


            #endregion
        }
        #endregion        

        /*
         * private string GenerateName(int district)
    {
        Randomizer randomizer = new Randomizer(this.m_districts.m_buffer[district].m_randomSeed);
        string format = Locale.Get("DISTRICT_PATTERN", randomizer.Int32(Locale.Count("DISTRICT_PATTERN")));
        string arg = Locale.Get("DISTRICT_NAME", randomizer.Int32(Locale.Count("DISTRICT_NAME")));
        return StringUtils.SafeFormat(format, arg);
    }
         */

    }

}
