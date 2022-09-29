using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public class RegionCitiesDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_CD_REGIONCITIES;
        protected override bool ExclusiveToIndex => false;
        protected override bool AnyButIndex => true;
        protected override int RefIndex { get; } = 0;
        public RegionCitiesDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public float Azimuth { get => xml.generalFloatValue ?? 0; set => xml.generalFloatValue = value; }
        public string Name { get => xml.givenStringId; set => xml.givenStringId = value; }
    }
}
