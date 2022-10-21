using CustomData.Enums;
using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public class BuildingGeneralDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_BUILDING;
        protected override bool ExclusiveToIndex => true;
        protected override bool AnyButIndex => false;
        protected override int RefIndex { get; } = 0;
        public BuildingGeneralDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public bool GetIsAutogen(UseGeneratedNameCategories cat) => xml.HasAnyFlag(1ul << (int)cat);
        public void SetAutogen(UseGeneratedNameCategories cat)
        {
            xml.AddFlag(1ul << (int)cat);
            ModInstance.Controller.Facade.CallBuildingNameGenStrategyChangedEvent();
        }

        public void UnsetAutogen(UseGeneratedNameCategories cat)
        {
            xml.RemoveFlag(1ul << (int)cat);
            ModInstance.Controller.Facade.CallBuildingNameGenStrategyChangedEvent();
        }

        internal bool IsRenameEnabled(BuildingAI buildingAi)
        {
            TransportInfo.TransportType transportType;
            VehicleInfo.VehicleType vehicleType;
            switch (buildingAi)
            {
                case TransportStationAI ai:
                    transportType = ai.m_transportInfo.m_transportType;
                    vehicleType = ai.GetVehicleType();
                    switch (transportType)
                    {
                        case TransportInfo.TransportType.Bus:
                            return buildingAi.m_info.m_class.m_level == ItemClass.Level.Level3
                                ? GetIsAutogen(UseGeneratedNameCategories.IntercityBus)
                                : GetIsAutogen(UseGeneratedNameCategories.Bus);
                        case TransportInfo.TransportType.Metro:
                            return GetIsAutogen(UseGeneratedNameCategories.Metro);
                        case TransportInfo.TransportType.Train:
                            return GetIsAutogen(UseGeneratedNameCategories.TrainPassenger);
                        case TransportInfo.TransportType.Ship:
                            return GetIsAutogen(UseGeneratedNameCategories.ShipPassenger);
                        case TransportInfo.TransportType.Airplane:
                            switch (vehicleType)
                            {
                                case VehicleInfo.VehicleType.Plane:
                                    return GetIsAutogen(UseGeneratedNameCategories.AirplanePassenger);
                                case VehicleInfo.VehicleType.Blimp:
                                    return GetIsAutogen(UseGeneratedNameCategories.Blimps);
                                default:
                                    return false;
                            }
                        case TransportInfo.TransportType.Tram:
                            return GetIsAutogen(UseGeneratedNameCategories.Tram);
                        case TransportInfo.TransportType.Monorail:
                            return GetIsAutogen(UseGeneratedNameCategories.Monorail);
                        case TransportInfo.TransportType.CableCar:
                            return GetIsAutogen(UseGeneratedNameCategories.CableCar);
                        case TransportInfo.TransportType.Trolleybus:
                            return GetIsAutogen(UseGeneratedNameCategories.Trolleybus);
                        case TransportInfo.TransportType.Helicopter:
                            return GetIsAutogen(UseGeneratedNameCategories.Helicopters);
                        default:
                            return false;
                    }
                case CargoStationAI ai:
                    transportType = ai.m_transportInfo.m_transportType;
                    switch (transportType)
                    {
                        case TransportInfo.TransportType.Train:
                            return GetIsAutogen(UseGeneratedNameCategories.TrainCargo);
                        case TransportInfo.TransportType.Ship:
                            return GetIsAutogen(UseGeneratedNameCategories.ShipCargo);
                        case TransportInfo.TransportType.Airplane:
                            return GetIsAutogen(UseGeneratedNameCategories.AirplaneCargo);
                        default:
                            return false;
                    }
                case TaxiStandAI _:
                    return GetIsAutogen(UseGeneratedNameCategories.Taxi);
                case ResidentialBuildingAI _:
                    return GetIsAutogen(UseGeneratedNameCategories.ResidentialZone);
                case OfficeBuildingAI _:
                    return GetIsAutogen(UseGeneratedNameCategories.OfficeZone);
                case CommercialBuildingAI _:
                    return GetIsAutogen(UseGeneratedNameCategories.CommercialZone);
                case IndustrialBuildingAI _:
                    return GetIsAutogen(UseGeneratedNameCategories.IndustrialZone);
                default:
                    return false;
            }
        }




    }
}
