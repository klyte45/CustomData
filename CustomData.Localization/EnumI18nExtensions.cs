
using CustomData.Enums;
using Kwytto.Localization;
using System;
using System.Linq;

namespace CustomData.Localization
{
    public static class EnumI18nExtensions
    {
        public static string ValueToI18n(this Enum variable)
        {
            switch (variable)
            {
                case CDVars v:
                    switch (v)
                    {
                        case CDVars.CurrentBuilding:
                            return Str.cd_Enum__CDVars_CurrentBuilding;
                        case CDVars.SourceBuilding:
                            return Str.cd_Enum__CDVars_SourceBuilding;
                    }
                    break;

                case DistrictAreaType.Regular: return Str.cd_Enum__DistrictAreaType_Regular;
                case DistrictAreaType.Parks_Amusement: return Str.cd_Enum__DistrictAreaType_Parks_Amusement;
                case DistrictAreaType.Parks_CityPark: return Str.cd_Enum__DistrictAreaType_Parks_CityPark;
                case DistrictAreaType.Parks_Natural: return Str.cd_Enum__DistrictAreaType_Parks_Natural;
                case DistrictAreaType.Parks_Zoo: return Str.cd_Enum__DistrictAreaType_Parks_Zoo;
                case DistrictAreaType.Industry_Oil: return Str.cd_Enum__DistrictAreaType_Industry_Oil;
                case DistrictAreaType.Industry_Mining: return Str.cd_Enum__DistrictAreaType_Industry_Mining;
                case DistrictAreaType.Industry_Farming: return Str.cd_Enum__DistrictAreaType_Industry_Farming;
                case DistrictAreaType.Industry_Forest: return Str.cd_Enum__DistrictAreaType_Industry_Forest;
                case DistrictAreaType.Campus_Trade: return Str.cd_Enum__DistrictAreaType_Campus_Trade;
                case DistrictAreaType.Campus_Liberal: return Str.cd_Enum__DistrictAreaType_Campus_Liberal;
                case DistrictAreaType.Campus_Regular: return Str.cd_Enum__DistrictAreaType_Campus_Regular;
                case DistrictAreaType.Airports: return Str.cd_Enum__DistrictAreaType_Airports;
                case DistrictAreaType.Pedestrian: return Str.cd_Enum__DistrictAreaType_Pedestrian;
                case UseGeneratedNameCategories.Bus: return Str.cd_Enum__UseGeneratedNameCategories_Bus;
                case UseGeneratedNameCategories.IntercityBus: return Str.cd_Enum__UseGeneratedNameCategories_IntercityBus;
                case UseGeneratedNameCategories.Trolleybus: return Str.cd_Enum__UseGeneratedNameCategories_Trolleybus;
                case UseGeneratedNameCategories.Tram: return Str.cd_Enum__UseGeneratedNameCategories_Tram;
                case UseGeneratedNameCategories.TrainPassenger: return Str.cd_Enum__UseGeneratedNameCategories_TrainPassenger;
                case UseGeneratedNameCategories.Monorail: return Str.cd_Enum__UseGeneratedNameCategories_Monorail;
                case UseGeneratedNameCategories.Metro: return Str.cd_Enum__UseGeneratedNameCategories_Metro;
                case UseGeneratedNameCategories.CableCar: return Str.cd_Enum__UseGeneratedNameCategories_CableCar;
                case UseGeneratedNameCategories.Ferry: return Str.cd_Enum__UseGeneratedNameCategories_Ferry;
                case UseGeneratedNameCategories.ShipPassenger: return Str.cd_Enum__UseGeneratedNameCategories_ShipPassenger;
                case UseGeneratedNameCategories.Helicopters: return Str.cd_Enum__UseGeneratedNameCategories_Helicopters;
                case UseGeneratedNameCategories.Blimps: return Str.cd_Enum__UseGeneratedNameCategories_Blimps;
                case UseGeneratedNameCategories.AirplanePassenger: return Str.cd_Enum__UseGeneratedNameCategories_AirplanePassenger;
                case UseGeneratedNameCategories.ShipCargo: return Str.cd_Enum__UseGeneratedNameCategories_ShipCargo;
                case UseGeneratedNameCategories.TrainCargo: return Str.cd_Enum__UseGeneratedNameCategories_TrainCargo;
                case UseGeneratedNameCategories.AirplaneCargo: return Str.cd_Enum__UseGeneratedNameCategories_AirplaneCargo;
                case UseGeneratedNameCategories.ResidentialZone: return Str.cd_Enum__UseGeneratedNameCategories_ResidentialZone;
                case UseGeneratedNameCategories.CommercialZone: return Str.cd_Enum__UseGeneratedNameCategories_CommercialZone;
                case UseGeneratedNameCategories.IndustrialZone: return Str.cd_Enum__UseGeneratedNameCategories_IndustrialZone;
                case UseGeneratedNameCategories.OfficeZone: return Str.cd_Enum__UseGeneratedNameCategories_OfficeZone;
                case UseGeneratedNameCategories.Taxi: return Str.cd_Enum__UseGeneratedNameCategories_Taxi;

                case VariableBuildingSubType x:
                    switch (x)
                    {
                        case VariableBuildingSubType.ImageLogo: return Str.cd_Enum__VariableBuildingSubType_ImageLogo;
                        case VariableBuildingSubType.PostalCode: return Str.cd_Enum__VariableBuildingSubType_PostalCode;
                    }
                    break;
                case VariableVehicleSubType x:
                    switch (x)
                    {
                        case VariableVehicleSubType.GeneratedId: return Str.cd_Enum__VariableVehicleSubType_GeneratedId;
                    }
                    break;

            }
            return variable.ValueToI18nKwytto();
        }

        public static string[] GetAllValuesI18n<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<Enum>().Select(x => x.ValueToI18n()).ToArray();
    }
}
