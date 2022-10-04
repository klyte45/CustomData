using CustomData.Utils;
using CustomData.Xml;
using static CustomData.Xml.InstanceDataExtensionXml;

namespace CustomData.Wrappers
{
    public class OwnCitySettingsDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_CD_REGIONCITIES;
        protected override bool ExclusiveToIndex => true;
        protected override bool AnyButIndex => false;
        protected override int RefIndex { get; } = 0xff;
        public OwnCitySettingsDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public ReferenceData GetDistrictGeneratorFile(DistrictAreaType type) => xml.SafeGetReference((long)type) ?? (xml.references[(long)type] = new ReferenceData());
        public int PostalCodeDigits { get => (xml.genericId ?? 0) % 1000; set => xml.genericId = value % 1000; }
        public string PostalCodeFormat { get => xml.givenStringId; set => xml.givenStringId = value; }
    }

}

