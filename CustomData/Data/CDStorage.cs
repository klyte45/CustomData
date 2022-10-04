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

      
    }
}
