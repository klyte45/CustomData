using CustomData.Utils;
using CustomData.Wrappers;
using Kwytto.Data;
using Kwytto.Utils;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEngine;
using static CustomData.Utils.SegmentUtils;

namespace CustomData.Xml
{

    public class CDStorage : DataExtensionBase<CDStorage>
    {
        public override string SaveId => "K45_CD_CDStorage";

        [XmlElement("InstanceExtraData")]
        public NonSequentialList<InstanceDataExtensionXml> InstanceExtraData { get; set; } = new NonSequentialList<InstanceDataExtensionXml>();
        [XmlAttribute("nextRegionCityId")]
        public ushort nextRegionCityId = 1;

        public DistrictDW GetDistrictData(byte districtId) => new DistrictDW(SafeGet(new InstanceID { District = districtId }));

        private InstanceDataExtensionXml SafeGet(InstanceID targetIdx)
        {
            if (!InstanceExtraData.TryGetValue(targetIdx.RawData, out var selectedDistrictData))
            {
                selectedDistrictData = CDStorage.Instance.InstanceExtraData[targetIdx.RawData] = new InstanceDataExtensionXml { Id = targetIdx };
            }

            return selectedDistrictData;
        }

        #region Region cities
        private RegionCitiesDW[] m_cachedCitiesSorted;
        public RegionCitiesDW[] CachedCitiesRegion
        {
            get
            {
                RefreshSortCache();
                return m_cachedCitiesSorted;
            }
        }

        public RegionCitiesDW GetCityAtAngle(float angle)
        {
            RefreshSortCache();
            return m_cachedCitiesSorted.Length == 0
                ? null
                : m_cachedCitiesSorted.Length == 1
                    ? m_cachedCitiesSorted[0]
                    : angle < m_cachedCitiesSorted[0].Azimuth
                        ? m_cachedCitiesSorted[m_cachedCitiesSorted.Length - 1]
                        : m_cachedCitiesSorted.Where(x => x.Azimuth <= angle).Last();
        }

        private void RefreshSortCache()
        {
            if (m_cachedCitiesSorted is null)
            {
                m_cachedCitiesSorted = InstanceExtraData
                    .Values
                    .Where(x => x.Id.Type == (InstanceType)InstanceIdUtils.TYPE_CD_REGIONCITIES && x.Id.Index > 0 && x.Id.Index < 0xFFFFFF)
                    .Select(x =>
                    {
                        var obj = new RegionCitiesDW(x);
                        obj.OnAzimuthChanged += () => m_cachedCitiesSorted = null;
                        return obj;
                    })
                    .OrderBy(x => x.Azimuth)
                    .ToArray();

                SegmentUtils.UpdateSegmentNamesView();
            }
        }
        public RegionCitiesDW CreateRegionCity()
        {
            var instanceObj = new InstanceDataExtensionXml { Id = new InstanceID { Type = (InstanceType)InstanceIdUtils.TYPE_CD_REGIONCITIES, Index = nextRegionCityId++ } };
            InstanceExtraData[instanceObj.Id.RawData] = instanceObj;
            m_cachedCitiesSorted = null;
            return new RegionCitiesDW(instanceObj);
        }
        private static readonly InstanceID COMMON_REGIONCITY_CONFIG_ID = new InstanceID { Type = (InstanceType)InstanceIdUtils.TYPE_CD_REGIONCITIES };
        public RegionCitiesCommonDW GetCommonCityConfig() => new RegionCitiesCommonDW(SafeGet(COMMON_REGIONCITY_CONFIG_ID));

        internal void RemoveCity(InstanceID id)
        {
            InstanceExtraData.Remove(id.RawData);
            m_cachedCitiesSorted = null;
        }

        internal void ResetCachedCitiesRegion() => m_cachedCitiesSorted = null;

        #endregion
        public CitizenGeneralDW GetCitizenData() => new CitizenGeneralDW(SafeGet(new InstanceID { Citizen = 0 }));

        public OwnCitySettingsDW GetOwnCitySettings() => new OwnCitySettingsDW(SafeGet(new InstanceID { Index = 0xFFFFFF, Type = (InstanceType)InstanceIdUtils.TYPE_CD_REGIONCITIES }));

        public BuildingGeneralDW GetBuildingGeneralSettings() => new BuildingGeneralDW(SafeGet(new InstanceID { Building = 0 }));

        #region Addresses

        public void GetAddressLines(Vector3 sidewalk, Vector3 midPosBuilding, out string[] addressLines)
        {
            addressLines = null;
            var buildingSettings = GetBuildingGeneralSettings();
            string line1 = buildingSettings.AddressLine1;
            string line2 = buildingSettings.AddressLine2;
            string line3 = buildingSettings.AddressLine3;
            string format = (line1 + "≠" + line2 + "≠" + line3);

            Regex regexEscape = new Regex(@"\\([ABCDEF[\]\\()])", RegexOptions.Compiled);
            foreach (Match escapeMatch in regexEscape.Matches(format))
            {
                format = format.Replace($"\\{escapeMatch.Groups[1].Value}", char.ConvertFromUtf32(0xFF00 + escapeMatch.Groups[1].Value[0]));
            }


            string a = "";//street
            int b = 0;//number
            string c = SimulationManager.instance.m_metaData.m_CityName;//city
            string d = "";//district
            string e = "";//zipcode
            string f = "";//area name

            if (format.ToCharArray().Intersect("AB".ToCharArray()).Count() > 0)
            {
                if (!GetStreetAndNumber(sidewalk, midPosBuilding, out a, out b))
                {
                    return;
                }
            }

            if (format.ToCharArray().Intersect("D[]".ToCharArray()).Count() > 0)
            {
                int districtId = DistrictManager.instance.GetDistrict(midPosBuilding);
                if (districtId > 0)
                {
                    d = DistrictManager.instance.GetDistrictName(districtId);
                    format = Regex.Replace(format, @"\]|\[", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }
                else
                {
                    format = Regex.Replace(format, @"\[[^]]*\]", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }

            }

            if (format.Contains("E"))
            {
                e = GetOwnCitySettings().TokenToPostalCode(sidewalk);
            }
            if (format.Intersect("F()".ToCharArray()).Count() > 0)
            {
                int parkId = DistrictManager.instance.GetPark(midPosBuilding);
                if (parkId > 0)
                {
                    f = DistrictManager.instance.GetParkName(parkId);
                    format = Regex.Replace(format, @"\)|\(", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }
                else
                {
                    format = Regex.Replace(format, @"\([^\)]*\)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }
            }
            ParseToFormatableString(ref format, 6);

            addressLines = new string(string.Format(format, a, b, c, d, e, f).Select(x => x > 0xff00 && x < 0xffff ? (char)(x - 0xFF00) : x).ToArray()).Split("≠".ToCharArray());
        }

        internal static bool GetStreetAndNumber(Vector3 sidewalk, Vector3 midPosBuilding, out string streetName, out int number)
        {
            SegmentUtils.GetNearestSegment(sidewalk, out Vector3 targetPosition, out float targetLength, out ushort targetSegmentId);
            if (targetSegmentId == 0)
            {
                streetName = string.Empty;
                number = 0;
                return false;
            }
            var seed = NetManager.instance.m_segments.m_buffer[targetSegmentId].m_nameSeed;
            var startSrc = MileageStartSource.DEFAULT;
            var offsetMeters = 0;
            //if (AdrNameSeedDataXml.Instance.NameSeedConfigs.TryGetValue(seed, out AdrNameSeedConfig seedConf))
            //{
            //    startSrc = seedConf.MileageStartSrc;
            //    offsetMeters = (int)seedConf.MileageOffset;
            //}

            return SegmentUtils.GetAddressStreetAndNumber(targetPosition, targetSegmentId, targetLength, midPosBuilding, startSrc, offsetMeters, out number, out streetName);
        }

        private static void ParseToFormatableString(ref string input, byte count)
        {
            for (int i = 0; i < count; i++)
            {
                var targetChar = ((char)('A' + i)).ToString();
                input = input.Replace($"\\{targetChar}", "☆").Replace(targetChar, $"{{{i}}}").Replace("☆", targetChar);
            }
        }
        #endregion
    }
}
