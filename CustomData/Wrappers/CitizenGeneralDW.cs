using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public class CitizenGeneralDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_CITIZEN;
        protected override bool ExclusiveToIndex => true;
        protected override bool AnyButIndex => false;
        protected override int RefIndex { get; } = 0;
        public CitizenGeneralDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public string MalesFile { get => xml.mainReference; set => xml.mainReference = value; }
        public string FemalesFile { get => xml.shortReference; set => xml.shortReference = value; }
        public string SurnamesFile { get => xml.qualifiedReference; set => xml.qualifiedReference = value; }
        public bool OrientalNaming { get => xml.HasAnyFlag(0x1); set => _ = value ? xml.AddFlag(0x1) : xml.RemoveFlag(0x1); }
    }
}
