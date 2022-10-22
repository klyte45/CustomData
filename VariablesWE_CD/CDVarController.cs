extern alias WE;

using CustomData;
using CustomData.Enums;
using CustomData.Overrides;
using CustomData.Xml;
using Kwytto.Interfaces;
using Kwytto.Utils;
using UnityEngine;
using WE::WriteEverywhere.Font.Utility;
using WE::WriteEverywhere.Xml;

namespace VariablesWE_CD
{
    internal class CDVarController : MonoBehaviour
    {
        static CDVarController instance;
        public static CDVarController Instance
        {
            get
            {
                if (instance is null && ModInstance.Controller is BaseController control)
                {
                    instance = control.gameObject.AddComponent<CDVarController>();
                }
                return instance;
            }
        }

        public SimpleNonSequentialList<BasicRenderInformation> CachedBuildingImages { get; } = new SimpleNonSequentialList<BasicRenderInformation>();
        public SimpleNonSequentialList<string> CachedBuildingPostalCode { get; } = new SimpleNonSequentialList<string>();

        private readonly NonSequentialList<VehicleItemCache> m_cacheVehicles = new NonSequentialList<VehicleItemCache>();

        public VehicleItemCache SafeGetVehicle(ushort vehicleId)
        {
            if (!m_cacheVehicles.TryGetValue(vehicleId, out var cachedVehicle))
            {
                cachedVehicle = m_cacheVehicles[vehicleId] = new VehicleItemCache { vehicleId = vehicleId };
            }
            return cachedVehicle;
        }
        public string GetBuildingPostalCode(ushort buildingId)
        {
            if (!CachedBuildingPostalCode.TryGetValue(buildingId, out var cached))
            {
                ref Building b = ref BuildingManager.instance.m_buildings.m_buffer[buildingId];
                cached = CachedBuildingPostalCode[buildingId] = CDStorage.Instance.GetOwnCitySettings().PostalCodeAtPosition(b.CalculateSidewalkPosition());
            }
            return cached;
        }

        private void Start()
        {
            CDFacade.Instance.EventOnBuildingLogoChanged += (x) => CachedBuildingImages.Remove(x);
            CDFacade.Instance.EventPostalCodeParamChanged += () => CachedBuildingPostalCode.Clear();
            CDFacade.Instance.EventOnBuildingVehicleIdPatternChanged += (x) => m_cacheVehicles.Clear();
        }
    }

    internal static class VarEnumExtensions
    {
        public static TextContent GetContentType(this VariableBuildingSubType type)
        {
            switch (type)
            {
                case VariableBuildingSubType.ImageLogo:
                    return TextContent.ParameterizedSpriteSingle;
                default:
                    return TextContent.ParameterizedText;
            }
        }
    }
}
