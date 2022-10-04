using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public class RegionCitiesCommonDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_CD_REGIONCITIES;
        protected override bool ExclusiveToIndex => true;
        protected override bool AnyButIndex => false;
        protected override int RefIndex { get; } = 0;
        public RegionCitiesCommonDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public string NameGenerator
        {
            get => xml.SafeGetReference(0).mainReference; set
            {
                xml.SafeGetReference(0).mainReference = value;
                CDStorage.Instance.ResetCachedCitiesRegion();
            }
        }
    }
}
