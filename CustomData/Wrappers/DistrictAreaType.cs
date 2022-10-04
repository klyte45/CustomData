using ICities;

namespace CustomData.Wrappers
{
    public enum DistrictAreaType : long
    {
        Regular,
        Parks_Amusement,
        Parks_CityPark,
        Parks_Natural,
        Parks_Zoo,
        Industry_Oil,
        Industry_Mining,
        Industry_Farming,
        Industry_Forest,
        Campus_Trade,
        Campus_Liberal,
        Campus_Regular,
        Airports,
        Pedestrian
    }

    public static class DistrictAreaTypeExtensions
    {
        public static bool IsAvailable(this DistrictAreaType areaType)
        {
            switch (areaType)
            {
                case DistrictAreaType.Regular:
                    return true;
                case DistrictAreaType.Parks_Amusement:
                case DistrictAreaType.Parks_CityPark:
                case DistrictAreaType.Parks_Natural:
                case DistrictAreaType.Parks_Zoo:
                    return LoadingManager.instance.SupportsExpansion(Expansion.Parks);
                case DistrictAreaType.Industry_Oil:
                case DistrictAreaType.Industry_Mining:
                case DistrictAreaType.Industry_Farming:
                case DistrictAreaType.Industry_Forest:
                    return LoadingManager.instance.SupportsExpansion(Expansion.Industry);
                case DistrictAreaType.Campus_Trade:
                case DistrictAreaType.Campus_Liberal:
                case DistrictAreaType.Campus_Regular:
                    return LoadingManager.instance.SupportsExpansion(Expansion.Campuses);
                case DistrictAreaType.Airports:
                    return LoadingManager.instance.SupportsExpansion(Expansion.Airport);
                case DistrictAreaType.Pedestrian:
                    return LoadingManager.instance.SupportsExpansion(Expansion.PlazasAndPromenades);
                default:
                    return false;
            }
        }
    }
}
