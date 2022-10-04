using CustomData.Wrappers;
using Kwytto.Localization;
using System;
using System.Linq;
namespace CustomData.Localization
{
    internal static class EnumI18nExtensions
    {
        public static string ValueToI18n(this Enum variable)
        {
            switch (variable)
            {
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
            }
            return variable.ValueToI18nKwytto();
        }

        public static string[] GetAllValuesI18n<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<Enum>().Select(x => x.ValueToI18n()).ToArray();
    }
}
